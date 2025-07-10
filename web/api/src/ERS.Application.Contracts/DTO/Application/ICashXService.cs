using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Application.Contracts.DTO.Application;
using ERS.DTO.Application.CashX;
using Microsoft.AspNetCore.Http;
namespace ERS.DTO.Application
{
    public interface ICashXService
    {
        Task<Result<string>> GenerateSummary(SummaryInfoDto summaryInfo);
        Task<Result<List<ApplyInfoDto>>> GetApplyInfoFromExcel(IFormFile excelFile, string company);
        Task<Result<int>> IsApplicant(string rno, string user, string token);
        Task<Result<CashResult>> Submit(IFormCollection formCollection, string user, string token);
        Task<Result<CashResult>> Keep(IFormCollection formCollection, string user, string token);
    }
}