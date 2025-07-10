using ERS.DTO;
using ERS.DTO.Account;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ERS.Services
{
    public class AccountServiceTest : ERSApplicationTestBase
    {
        private IAccountService _accountService;
        public AccountServiceTest()
        {
            _accountService = GetRequiredService<IAccountService>();
        }

        [Fact(DisplayName = "入账清单批量删除（成功）")]
        public async Task BatchDeleteSucess()
        {
            List<string> carryno = new List<string>()
            {
                "2110130001"
            };
            var result = await _accountService.BatchDeleteAccountlist(carryno);
            Assert.True(result.status == 1);
        }

        [Fact(DisplayName = "入账清单批量删除（失败）")]
        public async Task BatchDeleteFail()
        {
            List<string> carryno = new List<string>()
            {
                "211013001"
            };
            var result = await _accountService.BatchDeleteAccountlist(carryno);
            Assert.True(result.status == 2);
        }

        [Theory(DisplayName = "根據公司別獲取銀行")]
        [InlineData("WZS")]
        public async Task GetBankByCompany(string company)
        {
            var result = await _accountService.GetQueryBanks(company);
            Assert.True(result.data.Count > 0);
        }

        [Fact(DisplayName = "查询获取单据信息api（分页）入账清单")]
        public async Task QueryPostingSuccess()
        {
            Request<QueryFormParamDto> parameters = new()
            {
                pageIndex = 1,
                pageSize = 10,
                data = new QueryFormParamDto()
                {
                    companyList = new List<string>()
                    {
                        "WHQ"
                    }
                }
            };
            var result = await _accountService.GetPageAccountReceipts(parameters);
            Assert.True(result.data.Count == 0);
        }

        [Fact(DisplayName = "生成入賬清單")]
        public async Task GenerateAccountList()
        {
            QueryFormDto queryFormDto = new()
            {
                rno = new List<string>()
                {
                    "B2012170001"
                },
                company = "WZS"
            };
            var result = await _accountService.GenerateAccountList(queryFormDto,"Z22070025");
            Assert.True(result.status == 2);
        }

        [Fact(DisplayName = "生成入賬清單（薪資請款）")]
        public async Task GenerateCashXAccountList()
        {
            QueryFormDto queryFormDto = new()
            {
                rno = new List<string>()
                {
                    "XZ23080100001"
                },
                company = "WZS"
            };
            var result = await _accountService.GenerateAccountList(queryFormDto,"Z22070025");
            Assert.True(result.status == 1 && !String.IsNullOrEmpty(result.data));
        }

        [Fact(DisplayName = "薪資請款與其他單據一起入賬")]
        public async Task GenerateCashXAccountListError()
        {
            QueryFormDto queryFormDto = new()
            {
                rno = new List<string>()
                {
                    "XZ23080100001",
                    "E23012000001"
                },
                company = "WZS"
            };
            var result = await _accountService.GenerateAccountList(queryFormDto,"Z22070025");
            Assert.True(result.status == 2 && String.IsNullOrEmpty(result.data));
        }
    }
}