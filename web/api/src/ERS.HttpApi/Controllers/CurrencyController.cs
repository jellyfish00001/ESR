using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Application.Contracts.DTO.Currency;
using ERS.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace ERS.HttpApi.Controllers
{
#if DEBUG
    [AllowAnonymous]
#endif
    public class CurrencyController: BaseController
    {
        private ICurrencyService _currencyService;
        public CurrencyController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }
        [HttpGet("currencys")]
        public async Task<List<CurrencyDto>> GetQueryCurrency(){
            return await _currencyService.GetQueryCurrency(this.userId);
        }
        //查询本位币
        [HttpGet("currency/rate")]
        public async Task<decimal> queryRate(string ccurfrom, string ccurto)
        {
            return await _currencyService.queryRate(ccurfrom, ccurto);
        }
        //查询本位币
        [HttpGet("currency/rate2")]
        public async Task<decimal> queryRate2(string ccurfrom, string ccurto)
        {
            return await _currencyService.queryRate2(ccurfrom, ccurto);
        }
    }
}
