using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Application.Contracts.DTO.Application;
using ERS.DTO;
using ERS.DTO.Application;
using ERS.DTO.Application.CashX;
using ERS.IDomainServices;
using Microsoft.AspNetCore.Http;
using Volo.Abp.Application.Services;
namespace ERS.Services.Application
{
    public class CashXService : ApplicationService, ICashXService
    {
        private ICashXDomainService _CashXDomainService;
        private IApprovalDomainService _ApprovalDomainService;
        public CashXService(
            ICashXDomainService CashXDomainService,
            IApprovalDomainService ApprovalDomainService)
        {
            _CashXDomainService = CashXDomainService;
            _ApprovalDomainService = ApprovalDomainService;
        }
        public Task<Result<string>> GenerateSummary(SummaryInfoDto summaryInfo)
        {
            return _CashXDomainService.GenerateSummary(summaryInfo);
        }
        public async Task<Result<int>> IsApplicant(string rno, string user, string token)
        {
            return await _ApprovalDomainService.IsApplicant(rno, user, token);
        }
        public Task<Result<List<ApplyInfoDto>>> GetApplyInfoFromExcel(IFormFile excelFile, string company)
        {
            return _CashXDomainService.GetApplyInfoFromExcel(excelFile, company);
        }
        public async Task<Result<CashResult>> Submit(IFormCollection formCollection, string user, string token) => await _CashXDomainService.Submit(formCollection, user, token, "P");
        public async Task<Result<CashResult>> Keep(IFormCollection formCollection, string user, string token) => await _CashXDomainService.Submit(formCollection, user, token, "T");
    }
}