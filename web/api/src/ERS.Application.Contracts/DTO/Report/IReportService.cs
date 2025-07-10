using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.Auditor;
using ERS.DTO.Report;
using NPOI.XSSF.UserModel;
namespace ERS.Application.Contracts.DTO.Report
{
    public interface IReportService
    {
        Task<Result<List<ReportDto>>> GetQueryPageReport(Request<ReportQueryDto> request);
        Task<Result<List<ReportDto>>> GetQueryPageReportDetail(Request<ReportQueryDto> request);
        Task<XSSFWorkbook> DownloadReport(Request<ReportQueryDto> request);
        Task<XSSFWorkbook> DownloadAppliedDetailReport(Request<ReportQueryDto> request);
        Task<XSSFWorkbook> DownloadSignedReport(Request<QuerySignedReportDto> request);
        Task<XSSFWorkbook> DownloadSignedDetailReport(Request<QuerySignedReportDto> request);
        Task<Result<List<ReportDto>>> GetPaperReport(Request<ReportQueryDto> request);
        Task<XSSFWorkbook> DownloadPaper(Request<ReportQueryDto> request);
        Task<Result<List<AdvOffsetDto>>> GetAdvOffsetReport(Request<AdvOffsetQueryDto> request);
        Task<Result<ExcelDto<EntertainOverspendDetailDto>>> QueryOverspendDetail(GetOverSpendExcelDto input);
        Task<Result<List<ReportDto>>> QuerySignedReport(Request<QuerySignedReportDto> request);
        Task<Result<List<ReportDto>>> QuerySignedReportDetail(Request<QuerySignedReportDto> request);
        Task<Result<List<BDFormDto>>> QueryFormType();

        // Task<Result<List<StatisticsDto>>> GetStatisticsData(string token);
        // Task<byte[]> DownloadStatisticsTable(string token);
    }
}