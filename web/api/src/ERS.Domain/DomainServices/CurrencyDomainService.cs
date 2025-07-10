using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERS.Application.Contracts.DTO.Currency;
using ERS.Domain.IDomainServices;
using ERS.Entities;
using ERS.IRepositories;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories;
namespace ERS.Domain.DomainServices
{
    public class CurrencyDomainService : ICurrencyDomainService
    {
        private IRepository<CashCurrency, Guid> _cashCurrencyRepository;
        private IRepository<SAPExchRate, Guid> _sAPExchRateRepository;
        private ICompanyRepository _companyRepository;
        private IEmployeeRepository _employeeRepository;
        public CurrencyDomainService(
            IRepository<CashCurrency, Guid> cashCurrencyRepository,
            IRepository<SAPExchRate, Guid> sAPExchRateRepository,
            ICompanyRepository companyRepository,
            IEmployeeRepository employeeRepository)
        {
            _cashCurrencyRepository = cashCurrencyRepository;
            _sAPExchRateRepository = sAPExchRateRepository;
            _companyRepository = companyRepository;
            _employeeRepository = employeeRepository;
        }
        //只取能从转换为本位币的币别
        public async Task<List<CurrencyDto>> GetQueryCurrency(string userId)
        {
            //return (await _cashCurrencyRepository.WithDetailsAsync());
            List<CurrencyDto> result = new List<CurrencyDto>();
            string companyCode = (await _employeeRepository.WithDetailsAsync()).FirstOrDefault(w => w.emplid == userId)?.company;
            string basecurr = (await _companyRepository.WithDetailsAsync()).FirstOrDefault(w => w.CompanyCategory == companyCode)?.BaseCurrency;
            if (string.IsNullOrEmpty(basecurr))
            {
                basecurr = "USD";
            }
            result.Add(new CurrencyDto()
            {
                basecurr = basecurr,
                currency = basecurr,
                rate = 1,
                currency_desc = basecurr
            });
            var ccurList = await (await _sAPExchRateRepository.WithDetailsAsync()).Where(w => w.ccurto == basecurr).AsNoTracking().ToListAsync();
            ccurList.Select(s => s.ccurfrom).Distinct().ToList().ForEach(w => result.Add(new CurrencyDto()
            {
                basecurr = basecurr,
                currency = w,
                currency_desc = w,
                rate = queryRate(w, basecurr, ccurList).ccurrate
            }));
            return result;
        }
        public async Task<SAPExchRate> queryRate(string ccurfrom, string ccurto)
        {
            //DateTime current = DateTime.Now.AddDays(-Convert.ToInt32(DateTime.Now.Date.Day));
            // DateTime current = DateTime.Now.Date;
            // return (await _sAPExchRateRepository.WithDetailsAsync()).FirstOrDefault(b => b.ccurfrom == ccurfrom && b.ccurto == ccurto && current >= b.date_fm && current <= b.date_to);
     
            // 从数据库中获取汇率列表并查询符合条件的汇率
            var result = (await _sAPExchRateRepository.WithDetailsAsync())
                .Where(w => w.ccurfrom == ccurfrom && w.ccurto == ccurto)
                .OrderByDescending(w => w.date_fm)
                .FirstOrDefault();
            // 如果没有找到符合条件的汇率，返回一个默认的汇率对象
            if (result == null)
            {
                result = new SAPExchRate()
                {
                    ccurrate = 0,
                    ratioto=1,
                };
            }
            return result;
        }
        public SAPExchRate queryRate(string ccurfrom, string ccurto, List<SAPExchRate> ratelist)
        {
            var result = new SAPExchRate();
            //DateTime current = DateTime.Now.AddDays(-Convert.ToInt32(DateTime.Now.Date.Day));
            //result = ratelist.Where(b => b.ccurfrom == ccurfrom && b.ccurto == ccurto && b.ccurdate > current).OrderBy(b => b.ccurdate).FirstOrDefault();
            DateTime current = DateTime.Now.Date;
            // result = ratelist.FirstOrDefault(w => w.ccurfrom == ccurfrom && w.ccurto == ccurto && current >= w.date_fm && current <= w.date_to);
            result = ratelist
            .Where(w => w.ccurfrom == ccurfrom && w.ccurto == ccurto)
            .OrderByDescending(w => w.date_fm)
            .FirstOrDefault();
            if (result == null)
            {
                result = new SAPExchRate()
                {
                    ccurrate = 0
                };
            }
            return result;
        }
    }
}
