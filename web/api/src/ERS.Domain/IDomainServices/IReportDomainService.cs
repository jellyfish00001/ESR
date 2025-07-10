using ERS.Application.Contracts.DTO.Report;
using ERS.DTO;
using ERS.DTO.Auditor;
using ERS.DTO.Report;
using NPOI.XSSF.UserModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
namespace ERS.Domain.IDomainServices
{
    public interface IReportDomainService : IDomainService
    {
        Task<Result<List<ReportDto>>> queryPageReport(Request<ReportQueryDto> request);
        Task<Result<List<ReportDto>>> queryPageReportDetail(Request<ReportQueryDto> request);
        Task<XSSFWorkbook> DownloadReport(Request<ReportQueryDto> request);
        Task<XSSFWorkbook> DownloadAppliedDetailReport(Request<ReportQueryDto> request);
        Task<XSSFWorkbook> DownloadSignedReport(Request<QuerySignedReportDto> request);
        Task<XSSFWorkbook> DownloadSignedDetailReport(Request<QuerySignedReportDto> request);
        Task<Result<List<ReportDto>>> QueryPaperReport(Request<ReportQueryDto> request);
        Task<XSSFWorkbook> DownloadPaper(Request<ReportQueryDto> request);
        Task<Result<List<AdvOffsetDto>>> QueryAdvOffsetReport(Request<AdvOffsetQueryDto> request);
        Task<Result<ExcelDto<EntertainOverspendDetailDto>>> QueryOverspendDetail(GetOverSpendExcelDto input);
        Task<Result<List<ReportDto>>> QuerySignedReport(Request<QuerySignedReportDto> request);
        Task<Result<List<ReportDto>>> QuerySignedReportDetail(Request<QuerySignedReportDto> request);
        Task<Result<List<BDFormDto>>> QueryFormType();

        //Task<Result<List<StatisticsDto>>> GetStatisticsData(string token);
        //Task<byte[]> DownloadStatisticsTable(string token);
    }
}