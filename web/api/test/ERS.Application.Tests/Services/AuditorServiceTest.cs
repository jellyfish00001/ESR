using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERS.DTO.Auditor;
using Xunit;
using ERS.DTO;


namespace ERS.Services
{
    public class AuditorServiceTest : ERSApplicationTestBase
    {
        private IAuditorService _AuditorService;
        public AuditorServiceTest()
        {
            _AuditorService = GetRequiredService<IAuditorService>();
        }

        [Fact(DisplayName = "簽核主管Auditor添加（成功）")]
        public async Task AddAuditorSuccess()
        {
            AuditorParamsDto auditorParamsDto = new()
            {
                formcode = "CASH_3",
                emplid = "Z21032798",
                deptid = "ALL",
                auditid = "Z19081961",
                sdate = DateTime.Parse("2022-12-15 14:46:34.234"),
                edate = DateTime.Parse("2023-12-15 14:46:34.234"),
                company = "WZS"
            };
            var result = await _AuditorService.AddAuditor(auditorParamsDto, "Z22070025");
            Assert.True(result.status == 1);
        }

        [Fact(DisplayName = "簽核主管Auditor添加失敗（工號不存在）")]
        public async Task AddAuditorFail()
        {
            AuditorParamsDto auditorParamsDto = new()
            {
                formcode = "CASH_3",
                emplid = "Z22070025",//工號不存在
                deptid = "ALL",
                auditid = "Z19081961",
                sdate = DateTime.Parse("2022-12-15 14:46:34.234"),
                edate = DateTime.Parse("2023-12-15 14:46:34.234"),
                company = "WZS"
            };
            var result = await _AuditorService.AddAuditor(auditorParamsDto, "Z22070025");
            Assert.True(result.status == 2);
        }

        [Fact(DisplayName = "簽核主管Auditor删除（成功）")]
        public async Task DeleteAuditorSuccess()
        {
            Request<AuditorParamsDto> request = new()
            {
                pageIndex = 1,
                pageSize = 10,
                data = new AuditorParamsDto()
                {
                    deptid = "ALL",
                    companyList  = new List<string> { "WZS" }
                }
            };
            var query = await _AuditorService.GetPageAuditors(request);
            Guid? id = query.data.FirstOrDefault().Id;
            List<Guid?> Ids = new List<Guid?>();
            Ids.Add(id);
            var result = await _AuditorService.DeleteAuditors(Ids);
            Assert.True(result.status == 1);
        }

        [Fact(DisplayName = "簽核主管Auditor刪除（失敗）")]
        public async Task DeleteAuditorFail()
        {
            Guid? id = new Guid("3a08d084-e8d6-f47a-628a-72abbd22eea3");
            List<Guid?> Ids = new List<Guid?>();
            Ids.Add(id);
            var result = await _AuditorService.DeleteAuditors(Ids);
            Assert.True(result.status == 2);
        }

        [Fact(DisplayName = "簽核主管Auditor編輯（成功）")]
        public async Task EditAuditorSuccess()
        {
            Request<AuditorParamsDto> request = new()
            {
                pageIndex = 1,
                pageSize = 10,
                data = new AuditorParamsDto()
                {
                    deptid = "ALL",
                    companyList = new List<string>()
                    {
                        "WZS"
                    }
                }
            };
            var query = await _AuditorService.GetPageAuditors(request);
            Guid? id = query.data.FirstOrDefault().Id;
            AuditorParamsDto auditorParamsDto = new()
            {
                Id = id,
                formcode = "CASH_3",
                emplid = "Z19081961",
                auditid = "Z19081961",
                deptid = "MZ660",
                company = "WZS"
            };
            var result = await _AuditorService.EditAuditor(auditorParamsDto, "Z22070025");
            Assert.True(result.status == 1);
        }

        [Fact(DisplayName = "簽核主管Auditor編輯（失敗）")]
        public async Task EditAuditorFail()
        {
            Request<AuditorParamsDto> request = new()
            {
                pageIndex = 1,
                pageSize = 10,
                data = new AuditorParamsDto()
                {
                    deptid = "ALL",
                    companyList = new List<string>()
                    {
                        "WZS"
                    }
                }
            };
            var query = await _AuditorService.GetPageAuditors(request);
            Guid? id = query.data.FirstOrDefault().Id;
            AuditorParamsDto auditorParamsDto = new()
            {
                Id = id,
                formcode = "CASH_3",
                emplid = "Z19081961",
                deptid = "MZ660",
                company = "WZS"
            };
            var result = await _AuditorService.EditAuditor(auditorParamsDto, "Z22070025");
            Assert.True(result.status == 2);
        }

        [Fact(DisplayName = "簽核主管Auditor查詢（有數據）")]
        public async Task QueryAuditorHaveData()
        {
            Request<AuditorParamsDto> request = new()
            {
                pageIndex = 1,
                pageSize = 10,
                data = new AuditorParamsDto()
                {
                    deptid = "ALL",
                    companyList = new List<string>()
                    {
                        "WZS"
                    }
                }
            };
            var query = await _AuditorService.GetPageAuditors(request);
            Assert.True(query.data.Count > 0);
        }

        [Fact(DisplayName = "簽核主管Auditor查詢（無數據）")]
        public async Task QueryAuditorHaveNoData()
        {
            Request<AuditorParamsDto> request = new()
            {
                pageIndex = 1,
                pageSize = 10,
                data = new AuditorParamsDto()
                {
                    deptid = "ALL",
                    formcode = "Test",
                    companyList = new List<string>()
                    {
                        "WZS"
                    }
                }
            };
            var query = await _AuditorService.GetPageAuditors(request);
            Assert.True(query.data.Count == 0 && query.status == 1);
        }
    }
}