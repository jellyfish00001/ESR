using ERS.DTO.BDExp;
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

namespace ERS.Repositories
{
    public class BDExpRepository : EfCoreRepository<ERSDbContext, BdExp, Guid>, IBDExpRepository
    {

        private IRedisRepository _RedisRepository;
        public BDExpRepository(IDbContextProvider<ERSDbContext> dbContextProvider,IRedisRepository RedisRepository) : base(dbContextProvider)
        {
          _RedisRepository = RedisRepository;
        }
        public async Task<List<BdExp>> GetBdExps(string company,IList<string> expcodes, IList<string> acctcodes)
        {
            
            return (await GetDbSetAsync()).Where(b => b.company == company && expcodes.Any(c => c.Equals(b.expcode)) && acctcodes.Any(d => d.Equals(b.acctcode))).ToList();
        }

        public async Task<IList<EXPAddSign>> GetAddSignBefore(IList<string> expcodes, string company, int category = 1)
        {
            IList<EXPAddSign> data = await (await GetDbSetAsync()).Where(i => expcodes.Contains(i.expcode) && i.company == company && i.category == category && i.addsignstep != "BG").AsNoTracking().Select(i => new EXPAddSign() { addsign = i.addsign, addsignstep = i.addsignstep }).Distinct().ToListAsync();
            return data;
        }
        public async Task<IList<EXPAddSign>> GetAddSignAfter(IList<string> expcodes, string company, int category = 1)
        {
            IList<EXPAddSign> data = await (await GetDbSetAsync()).Where(i => expcodes.Contains(i.expcode) && i.company == company && i.category == category && i.addsignstep == "BG").AsNoTracking().Select(i => new EXPAddSign() { expcode = i.expcode, addsign = i.addsign, addsignstep = i.addsignstep }).Distinct().ToListAsync();
            return data;
        }
        public async Task<List<string>> GetAcctcodeByCompany(string company)
        {
            var result = (await GetDbSetAsync()).Where(w => w.company == company).AsNoTracking().Select(w => w.acctcode).Distinct().ToListAsync();
            return await result;
        }
        public async Task<BdExp> GetBDExpByCode(string expcode, string company)
        {
            return (await GetDbSetAsync()).Where(w => w.company == company && w.expcode == expcode).FirstOrDefault();
        }
        public async Task<BdExp> GetBDExpById(Guid? Id)
        {
            return await (await GetDbSetAsync()).Where(w => w.Id == Id).FirstOrDefaultAsync();
        }
    }
}
