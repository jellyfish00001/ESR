using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.FinApprover;
using Xunit;

namespace ERS.Services
{
    //財務簽核人員維護
    public class FinApproverServiceTest : ERSApplicationTestBase
    {
        private IFinApproverService _FinApproverService;
        public FinApproverServiceTest()
        {
            _FinApproverService = GetRequiredService<IFinApproverService>();
        }

        [Fact(DisplayName = "財務簽核人員查詢（有數據）")]
        public async Task QueryFinApproverSuccess()
        {
            Request<FinApproverParamsDto> request = new()
            {
                pageIndex = 1,
                pageSize = 10,
                data = new()
                {
                    company = "WZS",
                    plant = "UO",
                    companyList = new()
                    {
                        "WZS"
                    }
                }
            };
            var result = await _FinApproverService.QueryPageFinApprover(request);
            Assert.True(result.data.Count > 0);
        }

        [Fact(DisplayName = "財務簽核人員查詢（無數據）")]
        public async Task QueryFinApproverFail()
        {
            Request<FinApproverParamsDto> request = new()
            {
                pageIndex = 1,
                pageSize = 10,
                data = new()
                {
                    companyList = new(){
                        "WTZS"
                    },
                    plant = "UO"
                }
            };
            var result = await _FinApproverService.QueryPageFinApprover(request);
            Assert.True(result.data.Count == 0);
        }

        [Fact(DisplayName = "財務簽核人員添加（成功）")]
        public async Task AddFinApproverSuccess()
        {
            AddFinApproverDto request = new()
            {
                company = "WZS",
                company_code = "130",
                plant = "UO0",
                rv1 = "Z21032798"//工號存在
            };
            var result = await _FinApproverService.AddFinApprover(request, "Z21032798");
            Assert.True(result.status == 1);
        }

        [Fact(DisplayName = "財務簽核人員添加（失敗）")]
        public async Task AddFinApproverFail()
        {
            AddFinApproverDto request = new()
            {
                company = "WZS",
                company_code = "130",
                plant = "UO",
                rv1 = "Z22070025"//工號不存在
            };
            var result = await _FinApproverService.AddFinApprover(request, "Z22070025");
            Assert.True(result.status == 2);
        }

        [Fact(DisplayName = "財務簽核人員編輯（成功）")]
        public async Task EditFinApproverSuccess()
        {
            Request<FinApproverParamsDto> queryrequest = new()
            {
                pageIndex = 1,
                pageSize = 10,
                data = new()
                {
                    company = "WZS",
                    plant = "UO",
                    companyList = new List<string>
                    {
                        "WZS"
                    }
                }
            };
            var query = await _FinApproverService.QueryPageFinApprover(queryrequest);
            FinApproverDto request = new()
            {
                Id = query.data.FirstOrDefault().Id,
                company = "WTZS",
                plant = "SU",
                company_code = "130",
                rv1 = "Z21032798"
            };
            var result = await _FinApproverService.EditFinApprover(request, "Z22070025");
            Assert.True(result.status == 1);
        }

        [Fact(DisplayName = "財務簽核人員編輯（失敗）")]
        public async Task EditFinApproverFail()
        {
            Request<FinApproverParamsDto> queryrequest = new()
            {
                pageIndex = 1,
                pageSize = 10,
                data = new()
                {
                    company = "WZS",
                    plant = "UO",
                    companyList = new List<string>
                    {
                        "WZS"
                    }
                }
            };
            var query = await _FinApproverService.QueryPageFinApprover(queryrequest);
            FinApproverDto request = new()
            {
                Id = query.data.FirstOrDefault().Id,
                company = "WTZS",
                plant = "SU",
                company_code = "130",
                rv1 = "Z2103279"
            };
            var result = await _FinApproverService.EditFinApprover(request, "Z22070025");
            Assert.True(result.status == 2);
        }

        [Fact(DisplayName = "財務簽核人員刪除（成功）")]
        public async Task DeleteFinApproverSuccess()
        {
            Request<FinApproverParamsDto> queryrequest = new()
            {
                pageIndex = 1,
                pageSize = 10,
                data = new()
                {
                    company = "WZS",
                    plant = "UO",
                    companyList = new()
                    {
                        "WZS"
                    }
                }
            };
            var query = await _FinApproverService.QueryPageFinApprover(queryrequest);
            List<Guid?> Ids = new();
            Ids.Add(query.data.FirstOrDefault().Id);
            var result = await _FinApproverService.DeleteFinApprover(Ids);
            Assert.True(result.status == 1);
        }

        [Fact(DisplayName = "財務簽核人員刪除（失敗）")]
        public async Task DeleteFinApproverFail()
        {
            List<Guid?> Ids = new();
            Guid id = new("3a08d085-4b75-1162-14d0-fefe5f57ee8f");
            Ids.Add(id);
            var result = await _FinApproverService.DeleteFinApprover(Ids);
            Assert.True(result.status == 2);
        }
    }
}