using System.Threading.Tasks;
using ERS.IDomainServices;
using Xunit;

namespace ERS.DomainServices
{
    public class AccountDomainServiceTest : ERSDomainTestBase
    {
        private IAccountDomainService _AccountDomainService;
        public AccountDomainServiceTest()
        {
            _AccountDomainService = GetRequiredService<IAccountDomainService>();
        }

        [Theory(DisplayName = "根據公司別獲取銀行")]
        [InlineData("WZS")]
        public async Task GetBankByCompany(string company)
        {
            var result = await _AccountDomainService.QueryBanks(company);
            Assert.True(result.data.Count > 0);
        }
    }
}
