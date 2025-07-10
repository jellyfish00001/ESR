

using System.Threading.Tasks;
using ERS.Application.Contracts.DTO.Currency;
using Xunit;

namespace ERS.Application.Tests.Services
{
    public class CurrencyServiceTest: ERSApplicationTestBase
    {
        
        private ICurrencyService _currencyService;

        public CurrencyServiceTest()
        {
            _currencyService = GetRequiredService<ICurrencyService>();
        }

        [Theory(DisplayName = "查询币别")]
        [InlineData()]
        public async Task get_currency()
        {
            string userId = "Z19081961";
            //Act
            var result = await _currencyService.GetQueryCurrency(userId);
            //Assert
            Assert.NotNull(result);
        }
        [Theory(DisplayName = "获取本位币汇率")]
        [InlineData("RMB", "JPY")]
        public async Task get_currRate(string ccurfrom, string ccurto)
        {
            //Act
            var result = await _currencyService.queryRate(ccurfrom, ccurto);

            //Assert
            Assert.NotNull(result);
        }

        [Theory(DisplayName = "获取汇率币别相同")]
        [InlineData("RMB", "RMB")]
        public async Task get_ccurRate_samecurr(string ccurfrom, string ccurto)
        {
            var result = await _currencyService.queryRate(ccurfrom, ccurto);
            Assert.NotNull(result);
        }
    }
}
