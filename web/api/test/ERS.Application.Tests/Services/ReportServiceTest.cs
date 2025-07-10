using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using ERS.Application.Contracts.DTO.Report;
using ERS.DTO;
using ERS.DTO.Report;

namespace ERS.Application.Tests.Services
{
    public class ReportServiceTest : ERSApplicationTestBase
    {
        private IReportService _reportService;
        private Request<ReportQueryDto> request = new Request<ReportQueryDto>()
        {
            data = new ReportQueryDto(){
                startdate = null,
                enddate = null,
                rno = "",
                cemplid = "",
                status = new List<string>(),
                category = new List<string>(),
                formcode = new List<string>(),
                company = new List<string>()
            }
        };

        private Request<AdvOffsetQueryDto> advrequest = new Request<AdvOffsetQueryDto>()
        {
            data = new AdvOffsetQueryDto()
            {
                rno = null,
                cuser = null,
                status = null,
                company = new List<string>()
            }
        };

        public ReportServiceTest()
        {
            _reportService = GetRequiredService<IReportService>();
        }

        [Fact(DisplayName = "報表查詢")]
        public async Task Get_Reportdetails()
        {
            var result = await _reportService.GetQueryPageReport(request);
            Assert.NotNull(result);
        }

        [Fact(DisplayName = "按开始日期查詢")]
        public async Task Get_ReportdetailsBySdate()
        {
            request.data.startdate = Convert.ToDateTime("2021/08/10");
            var result = await _reportService.GetQueryPageReport(request);
            Assert.NotNull(result);
        }

        [Fact(DisplayName = "按结束日期查詢")]
        public async Task Get_ReportdetailsByEdate()
        {
            request.data.enddate = Convert.ToDateTime("2022/08/12");
            var result = await _reportService.GetQueryPageReport(request);
            Assert.NotNull(result);
        }

        [Fact(DisplayName = "按申請單號查詢")]
        public async Task Get_ReportdetailsByRno()
        {
            request.data.rno = "E22081000001";
            var result = await _reportService.GetQueryPageReport(request);
            Assert.NotNull(result);
        }

        [Fact(DisplayName = "按申請人工號查詢")]
        public async Task Get_ReportdetailsByCemplid()
        {
            request.data.cemplid = "Z21036528";
            var result = await _reportService.GetQueryPageReport(request);
            Assert.False(result.data.Count() > 0);
        }

        //[Fact(DisplayName = "報表查詢結果下載")]
        //public async Task download_reportdetails()
        //{
        //    request.data.rno = "E22081000001";
        //    Console.WriteLine(JsonConvert.SerializeObject(request));
        //    var result = await _reportService.DownloadReport(request);
        //    Assert.NotNull(result);
        //}

        //[Fact(DisplayName = "纸本单据查询")]
        //public async Task Get_PaperReport()
        //{
        //    var result = await _reportService.GetPaperReport(request);
        //    Assert.True(result.total >= 1);
        //}

        //[Fact(DisplayName = "下载查询纸本单据")]
        //public async Task Download_PaperReport()
        //{
        //    var result = await _reportService.DownloadPaper(request);
        //    Assert.NotNull(result);
        //}

        //[Fact(DisplayName = "预支冲账查询")]
        //public async Task Get_AdvOffsetReport()
        //{
        //    advrequest.data.rno = "A22092000001";
        //    var result = await _reportService.GetAdvOffsetReport(advrequest);
        //    Assert.True(result.total >= 1);
        //}
    }
}