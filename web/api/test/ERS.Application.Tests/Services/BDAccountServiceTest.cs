using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.BDAccount;
using Xunit;

namespace ERS.Services
{
    public class BDAccountServiceTest : ERSApplicationTestBase
    {
        private IBDAccountService _BDAccountService;
        public BDAccountServiceTest()
        {
            _BDAccountService = GetRequiredService<IBDAccountService>();
        }

        [Fact(DisplayName = "会计科目分页查询（有数据）")]
        public async Task QueryBDAccountSuccess()
        {
            Request<BDAccountParamDto> request = new()
            {
                pageIndex = 1,
                pageSize = 10,
                data = new()
                {
                    acctcode = "71412000",
                    acctname = "",
                    companyList = new()
                    {
                        "WZS"
                    }
                }
            };
            var result = await _BDAccountService.GetPageBDAcct(request);
            Assert.True(result.data.Count > 0);
        }

        [Fact(DisplayName = "会计科目分页查询（无数据）")]
        public async Task QueryBDAccountFail()
        {
            Request<BDAccountParamDto> request = new()
            {
                pageIndex = 1,
                pageSize = 10,
                data = new()
                {
                    acctcode = "71412000",
                    acctname = "",
                    companyList = new()
                    {
                        "WZ"
                    }
                }
            };
            var result = await _BDAccountService.GetPageBDAcct(request);
            Assert.True(result.data.Count == 0);
        }

        [Fact(DisplayName = "會計科目添加（成功）")]
        public async Task AddBDAccountSuccess()
        {
            BDAccountParamDto request = new()
            {
                acctcode = "71710000",
                acctname = "加工費用",
                company = "WZS"
            };
            var result = await _BDAccountService.AddBDAccount(request, "Z22070025");
            Assert.True(result.status == 1);
        }

        [Fact(DisplayName = "會計科目添加（失敗）")]
        public async Task AddBDAccountFail()
        {
            BDAccountParamDto request = new()
            {
                acctcode = "",
                acctname = "加工費用",
                company = "WZS"
            };
            var result = await _BDAccountService.AddBDAccount(request, "Z22070025");
            Assert.True(result.status == 2);
        }

        [Fact(DisplayName = "會計科目刪除（成功）")]
        public async Task DeleteBDAccountSuccess()
        {
            BDAccountParamDto request = new()
            {
                acctcode = "71412000",
                acctname = "交際費-其他",
                company = "WZS"
            };
            var result = await _BDAccountService.DeleteBDAccount(request);
            Assert.True(result.status == 1);
        }

        [Fact(DisplayName = "會計科目刪除（失敗）")]
        public async Task DeleteBDAccountFail()
        {
            BDAccountParamDto request = new()
            {
                acctcode = "123",
                acctname = "交際費-其他",
                company = "WZS"
            };
            var result = await _BDAccountService.DeleteBDAccount(request);
            Assert.True(result.status == 2);
        }

        [Fact(DisplayName = "會計科目編輯（成功）")]
        public async Task EditBDAccountSuccess()
        {
            BDAccountParamDto request = new()
            {
                acctcode = "71412000",
                acctname = "測試費-其他",
                company = "WZS"
            };
            var result = await _BDAccountService.EditBDAccount(request, "Z22070025");
            Assert.True(result.status == 1);
        }

        [Fact(DisplayName = "會計科目編輯（失敗）")]
        public async Task EditBDAccountFail()
        {
            BDAccountParamDto request = new()
            {
                acctcode = "1412000",
                acctname = "測試費-其他",
                company = "WZS"
            };
            var result = await _BDAccountService.EditBDAccount(request, "Z22070025");
            Assert.True(result.status == 2);
        }
    }
}