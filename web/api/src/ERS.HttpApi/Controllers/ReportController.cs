using ERS.Application.Contracts.DTO.PJcode;
using ERS.Application.Contracts.DTO.Report;
using ERS.DTO;
using ERS.DTO.Auditor;
using ERS.DTO.Report;
using ERS.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
namespace ERS.Controllers
{
#if DEBUG
    [AllowAnonymous]
    #endif
    [Route("api/[controller]")]
    public class ReportController : BaseController
    {
        private IReportService _reportService;
        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }
        [HttpPost("request")]
        public async Task<Result<List<ReportDto>>> GetQueryPageReport([FromBody]Request<ReportQueryDto> request){
            return await _reportService.GetQueryPageReport(request);
        }
        [HttpPost("request/detail")]
        public async Task<Result<List<ReportDto>>> GetQueryPageReportDetail([FromBody] Request<ReportQueryDto> request)
        {
            return await _reportService.GetQueryPageReportDetail(request);
        }
        [HttpPost("paper/request")]
        public async Task<Result<List<ReportDto>>> GetPaperReport([FromBody]Request<ReportQueryDto> request){
            return await _reportService.GetPaperReport(request);
        }
        [HttpPost("request/download")]
        public async Task<FileResult> DownloadReport([FromBody]Request<ReportQueryDto> request){
            XSSFWorkbook workbook = await _reportService.DownloadReport(request);
            byte[] data = null;
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                data = ms.ToArray();
            }
            string sFileName = $"ReportDetails_{TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local).ToString("yyyy-MM-dd")}";
            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName+".xlsx" );
        }
        [HttpPost("request/detail/download")]
        public async Task<FileResult> DownloadAppliedDetailReport([FromBody] Request<ReportQueryDto> request)
        {
            XSSFWorkbook workbook = await _reportService.DownloadAppliedDetailReport(request);
            byte[] data = null;
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                data = ms.ToArray();
            }
            string sFileName = $"ApliedReportDetails_{TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local).ToString("yyyy-MM-dd")}";
            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName + ".xlsx");
        }
        [HttpPost("request/signed/download")]
        public async Task<FileResult> DownloadSignedReport([FromBody] Request<QuerySignedReportDto> request)
        {
            XSSFWorkbook workbook = await _reportService.DownloadSignedReport(request);
            byte[] data = null;
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                data = ms.ToArray();
            }
            string sFileName = $"SignedReportDetails_{TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local).ToString("yyyy-MM-dd")}";
            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName + ".xlsx");
        }
        [HttpPost("request/signeddetail/download")]
        public async Task<FileResult> DownloadSignedDetailReport([FromBody] Request<QuerySignedReportDto> request)
        {
            XSSFWorkbook workbook = await _reportService.DownloadSignedDetailReport(request);
            byte[] data = null;
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                data = ms.ToArray();
            }
            string sFileName = $"SignedReportDetails_{TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local).ToString("yyyy-MM-dd")}";
            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName + ".xlsx");
        }

        [HttpPost("paper/request/download")]
        public async Task<FileResult> DownloadPaperReport([FromBody]Request<ReportQueryDto> request){
            XSSFWorkbook workbook = await _reportService.DownloadPaper(request);
            byte[] data = null;
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                data = ms.ToArray();
            }
            string sFileName = $"PaperReportDetails_{TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local).ToString("yyyy-MM-dd")}";
            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName+".xlsx" );
        }
        [HttpPost("advoffsetreport/request")]
        public async Task<Result<List<AdvOffsetDto>>> GetAdvOffsetReport([FromBody]Request<AdvOffsetQueryDto> request){
            return await _reportService.GetAdvOffsetReport(request);
        }
        /// <summary>
        /// 交際費扣減部門年度績效獎金預算報表數據查詢
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("overspend")]
        public async Task<Result<ExcelDto<EntertainOverspendDetailDto>>> QueryOverspendDetail([FromBody]GetOverSpendExcelDto input)
        {
            return await _reportService.QueryOverspendDetail(input);
        }
        /// <summary>
        /// 已簽核表單查詢
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("signed/query")]
        public async Task<Result<List<ReportDto>>> QuerySignedReport([FromBody]Request<QuerySignedReportDto> request)
        {
            return await _reportService.QuerySignedReport(request);
        }
        [HttpPost("signed/query/detail")]
        public async Task<Result<List<ReportDto>>> QuerySignedReportDetail([FromBody] Request<QuerySignedReportDto> request)
        {
            return await _reportService.QuerySignedReportDetail(request);
        }
        [HttpGet("formtype")]
        public Task<Result<List<BDFormDto>>> QueryFormType()
        {
            return _reportService.QueryFormType();
        }
        // [HttpPost("statistics/query")]
        // public async Task<Result<List<StatisticsDto>>> GetStatisticsData()
        // {
        //     return await _reportService.GetStatisticsData(this.token);
        // }
        // [HttpPost("statistics/download")]
        // public async Task<FileResult> DownloadStatisticsData()
        // {
        //     byte[] data = await _reportService.DownloadStatisticsTable(this.token);
        //     return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" ,"tongji.xlsx");
        // }
    }
}