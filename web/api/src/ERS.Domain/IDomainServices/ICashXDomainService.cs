using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Application.Contracts.DTO.Application;
using ERS.DTO;
using ERS.DTO.Application.CashX;
using Microsoft.AspNetCore.Http;
using Volo.Abp.Domain.Services;
namespace ERS.IDomainServices
{
    public interface ICashXDomainService : IDomainService
    {
        Task<Result<string>> GenerateSummary(SummaryInfoDto summaryInfo);
        Task<Result<List<ApplyInfoDto>>> GetApplyInfoFromExcel(IFormFile excelFile, string company);
        Task<Result<CashResult>> Submit(IFormCollection formCollection, string user, string token, string status = "P");
    }
}