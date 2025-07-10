using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.BaseData;
using ERS.DTO.BDCash;
using ERS.Entities;
using ERS.IRepositories;
using ERS.Localization;
using Microsoft.Extensions.Configuration;
using Volo.Abp.Application.Services;
using Volo.Abp.ObjectMapping;
namespace ERS.Services.BaseData
{
    public class CashReturnService : ApplicationService, ICashReturnService
    {
        private IBDCashReturnRepository _bdcashreturnRepository;
        private ICashAmountRepository _cashamountRepository;
        private IConfiguration _configuration;
        private IObjectMapper _objectMapper;
        public CashReturnService(IBDCashReturnRepository bDCashReturnRepository,
        IObjectMapper ObjectMapper,
        ICashAmountRepository cashAmountRepository,
        IConfiguration configuration)
        {
            LocalizationResource = typeof(ERSResource);
            _bdcashreturnRepository = bDCashReturnRepository;
            _cashamountRepository = cashAmountRepository;
            _objectMapper = ObjectMapper;
            _configuration = configuration;
        }
        //查询api（公司必填）
        public async Task<Result<List<BDCashDto>>> QueryBDCash(Request<BaseDataDto> parameters)
        {
            List<BDCashDto> bDCashData = new List<BDCashDto>();
            Result<List<BDCashDto>> result = new Result<List<BDCashDto>>();
            if (parameters != null)
            {
                List<BDCashReturn> bDCashReturnQuery = new List<BDCashReturn>();
                bDCashReturnQuery = (await _bdcashreturnRepository.WithDetailsAsync())
                                    .Where(b => parameters.data.company.Contains(b.company))
                                    .WhereIf(parameters.data.rno.Count > 0, b => parameters.data.rno.Contains(b.rno))
                                    .OrderByDescending(x => x.cdate)
                                    .ToList();
                bDCashData = _objectMapper.Map<List<BDCashReturn>, List<BDCashDto>>(bDCashReturnQuery);
                int pageIndex = parameters.pageIndex;
                int pageSize = parameters.pageSize;
                if (pageIndex < 1 || pageSize < 0)
                {
                    pageIndex = 1;
                    pageSize = 10;
                }
                int count = bDCashData.Count();
                bDCashData = bDCashData.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                result.data = bDCashData;
                result.total = count;
            }
            return result;
        }
        //新增BDCash
        public async Task<Result<BDCashDto>> AddBDCash(BDCashDto parameters)
        {
            Result<BDCashDto> result = new Result<BDCashDto>();
            if (parameters != null)
            {
                //判断此单号是否已有记录
                List<decimal> bDCashReturnQuery = (await _bdcashreturnRepository.WithDetailsAsync()).Where(b => b.rno == parameters.rno).AsNoTracking().Select(i => i.amount).ToList();
                decimal amount = (await _cashamountRepository.WithDetailsAsync()).Where(x => x.rno == parameters.rno).AsNoTracking().Select(x => x.actamt).FirstOrDefault();
                if(parameters.amount > (amount - (bDCashReturnQuery.Count == 0 ? 0 : bDCashReturnQuery.Sum(i => i))))
                {
                    result.status = 2;
                    result.message = L["AddFail"] + "，" + L["amounterror"];
                    return result;
                }
                BDCashReturn bDCashReturn = new BDCashReturn();//比较金额 查Cashamount actamt >= amount(输入)
                bDCashReturn.company = parameters.company;
                bDCashReturn.rno = parameters.rno;
                bDCashReturn.amount = parameters.amount;
                bDCashReturn.cuser = parameters.cuser;
                bDCashReturn.cdate = DateTime.Now;
                await _bdcashreturnRepository.InsertAsync(bDCashReturn);
                BDCashDto bDCashDto = new BDCashDto();
                bDCashDto = _objectMapper.Map<BDCashReturn, BDCashDto>(bDCashReturn);
                result.data = bDCashDto;
                result.status = 1;
                result.message = L["AddSuccess"];
            }
            else
            {
                result.status = 2;
                result.message = L["AddFail"];
            }
            return result;
        }
        //修改BDCash
        public async Task<Result<BDCashDto>> UpdateBDCash(BDCashDto parameters)
        {
            Result<BDCashDto> result = new Result<BDCashDto>();
            BDCashReturn bDCashReturn = (await _bdcashreturnRepository.WithDetailsAsync()).Where(b => b.Id == parameters.Id).FirstOrDefault();
            if (bDCashReturn != null)
            {
                List<BDCashReturn> bDCashReturnList = (await _bdcashreturnRepository.WithDetailsAsync()).Where(b => b.rno == bDCashReturn.rno).AsNoTracking().ToList().Where(i => i.Id != bDCashReturn.Id).ToList();
                decimal amount = (await _cashamountRepository.WithDetailsAsync()).Where(x => x.rno == parameters.rno).AsNoTracking().Select(x => x.actamt).FirstOrDefault();
                if (amount - bDCashReturnList.Sum(i => i.amount) < parameters.amount)
                {
                    result.status = 2;
                    result.message = L["AddFail"] + "，" + L["amounterror"];
                    return result;
                }
                bDCashReturn.amount = parameters.amount;
                bDCashReturn.mdate = DateTime.Now;
                bDCashReturn.muser = parameters.muser;
                await _bdcashreturnRepository.UpdateAsync(bDCashReturn);
                result.status = 1;
                result.message = L["UpdateSuccess"];
                BDCashDto bDCashDto = new BDCashDto();
                bDCashDto = _objectMapper.Map<BDCashReturn, BDCashDto>(bDCashReturn);
                result.data = bDCashDto;
            }
            else
            {
                result.status = 2;
                result.message = L["docNotFound"];
            }
            return result;
        }
        //删除BDCash
        public async Task<Result<string>> DeleteBDCash(BDCashDto request)
        {
            Result<string> result = new Result<string>();
            BDCashReturn bDCashReturn = (await _bdcashreturnRepository.WithDetailsAsync()).Where(b => b.Id == request.Id).FirstOrDefault();
            if (bDCashReturn != null)
            {
                await _bdcashreturnRepository.DeleteAsync(bDCashReturn);
                result.status = 1;
                result.message = L["DeleteSuccess"];
            }
            else
            {
                result.status = 2;
                result.message = L["docNotFound"];
            }
            return result;
        }
    }
}