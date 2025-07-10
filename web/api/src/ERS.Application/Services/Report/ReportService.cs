using ERS.Application.Contracts.DTO.Report;
using ERS.Domain.DomainServices;
using ERS.Domain.IDomainServices;
using ERS.DTO;
using ERS.DTO.AppConfig;
using ERS.DTO.Auditor;
using ERS.DTO.Report;
using ERS.Entities;
using ERS.Localization;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
namespace ERS.Application.Services.Report
{
    public class ReportService : ApplicationService, IReportService
    {
        private IReportDomainService _reportDomainService;
        private IAppConfigService _AppConfigService;
        public ReportService(IReportDomainService reportDomainService, IAppConfigService AppConfigService)
        {
            _reportDomainService = reportDomainService;
            LocalizationResource = typeof(ERSResource);
            _AppConfigService = AppConfigService;
        }
        public async Task<Result<List<ReportDto>>> GetQueryPageReport(Request<ReportQueryDto> request)
        {
           return await _reportDomainService.queryPageReport(request);
        }
        public async Task<Result<List<ReportDto>>> GetQueryPageReportDetail(Request<ReportQueryDto> request)
        {
            return await _reportDomainService.queryPageReportDetail(request);
        }
        public async Task<XSSFWorkbook> DownloadReport(Request<ReportQueryDto> request)
        {
            var resultworkbook = await _reportDomainService.DownloadReport(request);
            ISheet sheet = resultworkbook.GetSheet("sheet");
            IRow rowHeader = sheet.CreateRow(0);
            rowHeader.CreateCell(0).SetCellValue(L["company"]);
            rowHeader.CreateCell(1).SetCellValue(L["rno"]);
            rowHeader.CreateCell(2).SetCellValue(L["formname"]);
            rowHeader.CreateCell(3).SetCellValue(L["deptid"]);
            rowHeader.CreateCell(4).SetCellValue(L["cemplid"]);
            rowHeader.CreateCell(5).SetCellValue(L["cname"]);
            rowHeader.CreateCell(6).SetCellValue(L["cdate"]);
            rowHeader.CreateCell(7).SetCellValue(L["expname"]);
            rowHeader.CreateCell(8).SetCellValue(L["expdeptid"]);
            rowHeader.CreateCell(9).SetCellValue(L["currency"]);
            rowHeader.CreateCell(10).SetCellValue(L["actamt"]);
            rowHeader.CreateCell(11).SetCellValue(L["stepname"]);
            rowHeader.CreateCell(12).SetCellValue(L["payment"]);
            rowHeader.CreateCell(13).SetCellValue("发票");
            rowHeader.CreateCell(14).SetCellValue("附件");
            rowHeader.CreateCell(15).SetCellValue("是否异常");
            return resultworkbook;
        }
        public async Task<XSSFWorkbook> DownloadAppliedDetailReport(Request<ReportQueryDto> request)
        {
            var resultworkbook = await _reportDomainService.DownloadAppliedDetailReport(request);
            ISheet sheet = resultworkbook.GetSheet("sheet");
            IRow rowHeader = sheet.CreateRow(0);
            rowHeader.CreateCell(0).SetCellValue(L["company"]);
            rowHeader.CreateCell(1).SetCellValue(L["rno"]);
            rowHeader.CreateCell(2).SetCellValue(L["formcode"]);
            rowHeader.CreateCell(3).SetCellValue(L["deptid"]);
            rowHeader.CreateCell(4).SetCellValue(L["cemplid"]);
            rowHeader.CreateCell(5).SetCellValue(L["cname"]);
            rowHeader.CreateCell(6).SetCellValue(L["digest"]);
            rowHeader.CreateCell(7).SetCellValue(L["expname"]);
            rowHeader.CreateCell(8).SetCellValue(L["rdate"]);
            rowHeader.CreateCell(9).SetCellValue(L["invno"]);
            rowHeader.CreateCell(10).SetCellValue(L["expdeptid"]);
            rowHeader.CreateCell(11).SetCellValue(L["currency"]);
            rowHeader.CreateCell(12).SetCellValue("actamt");
            rowHeader.CreateCell(13).SetCellValue("untaxamount");
            rowHeader.CreateCell(14).SetCellValue("taxamount");
            rowHeader.CreateCell(15).SetCellValue(L["invcode"]);
            rowHeader.CreateCell(16).SetCellValue(L["cdate"]);
            rowHeader.CreateCell(17).SetCellValue(L["formname"]);
            rowHeader.CreateCell(18).SetCellValue(L["projectcode"]);
            rowHeader.CreateCell(19).SetCellValue(L["payeeId"]);
            rowHeader.CreateCell(20).SetCellValue("payeename");
            rowHeader.CreateCell(21).SetCellValue("stepname");
            rowHeader.CreateCell(22).SetCellValue("apid");
            rowHeader.CreateCell(23).SetCellValue("payment");
            return resultworkbook;
        }

        public async Task<XSSFWorkbook> DownloadSignedReport(Request<QuerySignedReportDto> request)
        {
            var resultworkbook = await _reportDomainService.DownloadSignedReport(request);
            ISheet sheet = resultworkbook.GetSheet("sheet");
            IRow rowHeader = sheet.CreateRow(0);
            rowHeader.CreateCell(0).SetCellValue(L["company"]);
            rowHeader.CreateCell(1).SetCellValue(L["rno"]);
            rowHeader.CreateCell(2).SetCellValue(L["formname"]);
            rowHeader.CreateCell(3).SetCellValue(L["deptid"]);
            rowHeader.CreateCell(4).SetCellValue(L["cemplid"]);
            rowHeader.CreateCell(5).SetCellValue(L["cname"]);
            rowHeader.CreateCell(6).SetCellValue(L["cdate"]);
            rowHeader.CreateCell(7).SetCellValue(L["expname"]);
            rowHeader.CreateCell(8).SetCellValue(L["expdeptid"]);
            rowHeader.CreateCell(9).SetCellValue(L["currency"]);
            rowHeader.CreateCell(10).SetCellValue(L["actamt"]);
            rowHeader.CreateCell(11).SetCellValue(L["stepname"]);
            rowHeader.CreateCell(12).SetCellValue(L["payment"]);
            rowHeader.CreateCell(13).SetCellValue("发票");
            rowHeader.CreateCell(14).SetCellValue("附件");
            rowHeader.CreateCell(15).SetCellValue("是否异常");
            return resultworkbook;
        }
        public async Task<XSSFWorkbook> DownloadSignedDetailReport(Request<QuerySignedReportDto> request)
        {
            var resultworkbook = await _reportDomainService.DownloadSignedDetailReport(request);
            ISheet sheet = resultworkbook.GetSheet("sheet");
            IRow rowHeader = sheet.CreateRow(0);
            rowHeader.CreateCell(0).SetCellValue(L["company"]);
            rowHeader.CreateCell(1).SetCellValue(L["rno"]);
            rowHeader.CreateCell(2).SetCellValue(L["formcode"]);
            rowHeader.CreateCell(3).SetCellValue(L["deptid"]);
            rowHeader.CreateCell(4).SetCellValue(L["cemplid"]);
            rowHeader.CreateCell(5).SetCellValue(L["cname"]);
            rowHeader.CreateCell(6).SetCellValue(L["digest"]);
            rowHeader.CreateCell(7).SetCellValue(L["expname"]);
            rowHeader.CreateCell(8).SetCellValue(L["rdate"]);
            rowHeader.CreateCell(9).SetCellValue(L["invno"]);
            rowHeader.CreateCell(10).SetCellValue(L["expdeptid"]);
            rowHeader.CreateCell(11).SetCellValue(L["currency"]);
            rowHeader.CreateCell(12).SetCellValue("actamt");
            rowHeader.CreateCell(13).SetCellValue("untaxamount");
            rowHeader.CreateCell(14).SetCellValue("taxamount");
            rowHeader.CreateCell(15).SetCellValue(L["invcode"]);
            rowHeader.CreateCell(16).SetCellValue(L["cdate"]);
            rowHeader.CreateCell(17).SetCellValue(L["formname"]);
            rowHeader.CreateCell(18).SetCellValue(L["projectcode"]);
            rowHeader.CreateCell(19).SetCellValue(L["payeeId"]);
            rowHeader.CreateCell(20).SetCellValue("payeename");
            rowHeader.CreateCell(21).SetCellValue("stepname");
            rowHeader.CreateCell(22).SetCellValue("apid");
            rowHeader.CreateCell(23).SetCellValue("payment");
            return resultworkbook;
        }

        public async Task<Result<List<ReportDto>>> QuerySignedReport(Request<QuerySignedReportDto> request)
        {
            return await _reportDomainService.QuerySignedReport(request);
        }
        public async Task<Result<List<ReportDto>>> QuerySignedReportDetail(Request<QuerySignedReportDto> request)
        {
            return await _reportDomainService.QuerySignedReportDetail(request);

        }

        public async Task<Result<List<ReportDto>>> GetPaperReport(Request<ReportQueryDto> request)
        {
            return await _reportDomainService.QueryPaperReport(request);
        }
        public async Task<XSSFWorkbook> DownloadPaper(Request<ReportQueryDto> request)
        {
            var resultworkbook = await _reportDomainService.DownloadPaper(request);
            ISheet sheet = resultworkbook.GetSheet("sheet");
            IRow rowHeader = sheet.CreateRow(0);
            rowHeader.CreateCell(0).SetCellValue(L["company"]);
            rowHeader.CreateCell(1).SetCellValue(L["rno"]);
            rowHeader.CreateCell(2).SetCellValue(L["formname"]);
            rowHeader.CreateCell(3).SetCellValue(L["deptid"]);
            rowHeader.CreateCell(4).SetCellValue(L["cemplid"]);
            rowHeader.CreateCell(5).SetCellValue(L["cname"]);
            rowHeader.CreateCell(6).SetCellValue(L["cdate"]);
            rowHeader.CreateCell(7).SetCellValue(L["expname"]);
            rowHeader.CreateCell(8).SetCellValue(L["expdeptid"]);
            rowHeader.CreateCell(9).SetCellValue(L["currency"]);
            rowHeader.CreateCell(10).SetCellValue(L["actamt"]);
            rowHeader.CreateCell(11).SetCellValue(L["stepname"]);
            return resultworkbook;
        }
        public async Task<Result<List<AdvOffsetDto>>> GetAdvOffsetReport(Request<AdvOffsetQueryDto> request)
        {
            return await _reportDomainService.QueryAdvOffsetReport(request);
        }
        public async Task<Result<ExcelDto<EntertainOverspendDetailDto>>> QueryOverspendDetail(GetOverSpendExcelDto input)
        {
            return await _reportDomainService.QueryOverspendDetail(input);
        }




        public async Task<Result<List<BDFormDto>>> QueryFormType()
        {
            return await _reportDomainService.QueryFormType();
        }

        // public async Task<Result<List<StatisticsDto>>> GetStatisticsData(string token)
        // {
        //     return await _reportDomainService.GetStatisticsData(token);
        // }
        // public async Task<byte[]> DownloadStatisticsTable(string token)
        // {
        //     return await _reportDomainService.DownloadStatisticsTable(token);
        // }
    }
}
