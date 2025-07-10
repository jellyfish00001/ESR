using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Application.Contracts.DTO.Currency;
using ERS.Domain.IDomainServices;
using ERS.Entities;
using Volo.Abp.Application.Services;
namespace ERS.Application.Services
{
    public class CurrencyService : ApplicationService, ICurrencyService
    {
        private ICurrencyDomainService _currencyDomainService;
        public CurrencyService(ICurrencyDomainService currencyDomainService)
        {
            _currencyDomainService = currencyDomainService;
        }
        public async Task<List<CurrencyDto>> GetQueryCurrency(string userId)
        {
            // List<CurrencyDto> currencyDtos = new List<CurrencyDto>();
            // List<string> cashCurrencies = await _currencyDomainService.GetQueryCurrency(userId);
            // cashCurrencies.ForEach(i => currencyDtos.Add(new CurrencyDto { currency = i, currency_desc = i }));
            return await _currencyDomainService.GetQueryCurrency(userId);
        }
        public async Task<decimal> queryRate(string ccurfrom, string ccurto)
        {
            SAPExchRate sAPExch = await _currencyDomainService.queryRate(ccurfrom, ccurto);
            if (sAPExch != null)
            {
                return sAPExch.ccurrate * sAPExch.ratioto;
            }
            if(ccurfrom == ccurto)
            {
                return 1;
            }
            return 0;
        }
        public async Task<decimal> queryRate2(string ccurfrom, string ccurto)
        {
            SAPExchRate sAPExch = await _currencyDomainService.queryRate(ccurfrom, ccurto);
            if (sAPExch != null)
            {
                return sAPExch.ccurrate * sAPExch.ratioto;
            }
            if (ccurfrom == ccurto)
            {
                return 1;
            }
            return 0;
        }
    }
}