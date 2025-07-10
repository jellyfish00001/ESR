using ERS.DTO;
using ERS.DTO.BDExp;
using ERS.DTO.BDExpenseSenario;
using ERS.Entities;
using ERS.EntityFrameworkCore;
using ERS.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.ObjectMapping;

namespace ERS.Repositories
{
    public class BDExpenseSenarioRepository : EfCoreRepository<ERSDbContext, BDExpenseSenario, Guid>, IBDExpenseSenarioRepository
    {
        private readonly IObjectMapper _ObjectMapper;
        private IRedisRepository _RedisRepository;
        public BDExpenseSenarioRepository(IDbContextProvider<ERSDbContext> dbContextProvider,IRedisRepository RedisRepository, IObjectMapper iObjectMapper) : base(dbContextProvider)
        {
          _RedisRepository = RedisRepository;
          _ObjectMapper = iObjectMapper;
        }
        public async Task<List<BDExpenseSenario>> GetBdExps(string companycategory, IList<string> expcodes, IList<string> acctcodes)
        {
            
            return (await GetDbSetAsync()).Where(b => b.companycategory == companycategory && expcodes.Any(c => c.Equals(b.expensecode)) && acctcodes.Any(d => d.Equals(b.accountcode))).ToList();
        }

        //public async Task<IList<EXPAddSign>> GetAddSignBefore(IList<string> expcodes, string companycategory, string category)
        //{
        //    IList<EXPAddSign> data = await (await GetDbSetAsync()).Where(i => expcodes.Contains(i.expensecode) && i.companycategory == companycategory && i.category == category 
        //    //&& i.addsignstep != "BG"
        //    ).AsNoTracking().Select(i => new EXPAddSign() 
        //    {
        //       addsign = i.addsign, addsignstep = i.addsignstep 
        //    }).Distinct().ToListAsync();
        //    return data;
        //}

        //public async Task<IList<EXPAddSign>> GetAddSignAfter(IList<string> expcodes, string companycategory, string category)
        //{
        //    IList<EXPAddSign> data = await (await GetDbSetAsync()).Where(i => expcodes.Contains(i.expensecode) && i.companycategory == companycategory && i.category == category
        //    //&& i.addsignstep == "BG"
        //    ).AsNoTracking().Select(i => new EXPAddSign() 
        //    {
        //        expcode = i.expensecode,
        //        addsign = i.addsign,
        //        addsignstep = i.addsignstep
        //    }).Distinct().ToListAsync();
        //    return data;
        //}

        public async Task<List<string>> GetAcctcodeByCompany(string companycategory)
        {
            var result = (await GetDbSetAsync()).Where(w => w.companycategory == companycategory).AsNoTracking().Select(w => w.accountcode).Distinct().ToListAsync();
            return await result;
        }
        public async Task<BDExpenseSenario> GetBDExpByCode(string expcode, string companycategory)
        {
            return (await GetDbSetAsync()).Where(w => w.companycategory == companycategory && w.expensecode == expcode).FirstOrDefault();
        }
        public async Task<BDExpenseSenario> GetBDExpById(Guid? Id)
        {
            return await (await GetDbSetAsync()).Where(w => w.Id == Id).FirstOrDefaultAsync();
        }

        public async Task<BDSenarioDto> GetBDSenarioById(Guid Id)
        {
            DbSet<ExtraSteps> extraStepsDbSet = (await GetDbContextAsync()).Set<ExtraSteps>();

            BDExpenseSenario bDSenario = await (await GetDbSetAsync())
                        .Where(bs => bs.Id == Id).FirstOrDefaultAsync();

            List<ExtraSteps> extraSteps = await extraStepsDbSet
                .Where(es => bDSenario.Id == es.SenarioId)
                .ToListAsync();

            BDSenarioDto bDSenarioDto = _ObjectMapper.Map<BDExpenseSenario, BDSenarioDto>(bDSenario);
            bDSenarioDto.extraSteps = _ObjectMapper.Map<List<ExtraSteps>, List<ExtraStepsDto>>(extraSteps);

            return bDSenarioDto;
        }

        public async Task<List<BDExpenseSenario>> SearchBDSenariosByKeywordAsync(string companyCategory, string keyword)
        {
            return await (await GetDbSetAsync())
                        .Where(bs => (companyCategory == null || bs.companycategory == companyCategory) &&
                            (keyword == null || (bs.keyword != null && bs.keyword.Contains(keyword)) ||
                            (bs.senarioname != null && bs.senarioname.Contains(keyword))))
                        .Take(20)
                        .ToListAsync();
        }

        public async Task<PagedResultDto<BDSenarioDto>> GetBDSenarios(Request<BDSenarioFilterDto> request)
        {
            DbSet<ExtraSteps> extraStepsDbSet = (await GetDbContextAsync()).Set<ExtraSteps>();
            BDSenarioFilterDto filter = request.data;

            List<BDExpenseSenario> bDSenarios = await (await GetDbSetAsync())
                .Where(bs => (filter == null ||
                    (filter.CompanyCategory == null || bs.companycategory == filter.CompanyCategory) &&
                        (filter.ExpenseCode == null || bs.expensecode == filter.ExpenseCode) &&
                        (filter.SenarioName == null || bs.senarioname == filter.SenarioName)))
                .Skip((request.pageIndex - 1) * request.pageSize)
                .Take(request.pageSize)
                .ToListAsync();

            List<Guid> senarioIds = bDSenarios.Select(bs => bs.Id).ToList();
            List<ExtraSteps> extraSteps = await extraStepsDbSet
                .Where(es => senarioIds.Contains(es.SenarioId))
                .ToListAsync();

            List<BDSenarioDto> bDSenarioDtos = bDSenarios.Select(bs =>
            {
                BDSenarioDto bsd = _ObjectMapper.Map<BDExpenseSenario, BDSenarioDto>(bs);

                bsd.extraSteps = _ObjectMapper.Map<List<ExtraSteps>, List<ExtraStepsDto>>(
                    extraSteps.Where(es => es.SenarioId == bs.Id).ToList()
                );

                return bsd;
            }).ToList();

            int totalCount = await (await GetDbSetAsync()).CountAsync();

            PagedResultDto<BDSenarioDto> pagedResultDto = new()
            {
                Data = bDSenarioDtos,
                TotalCount = totalCount,
                PageIndx = request.pageIndex,
                PageSize = request.pageSize
            };

            return pagedResultDto;
        }
    }
}
