using ERS.Domain.IDomainServices;
using ERS.DTO;
using ERS.DTO.Application;
using ERS.DTO.Approval;
using ERS.IDomainServices;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
namespace ERS.Services.Application
{
    public class CashService : ApplicationService, ICashService
    {
        private IApprovalDomainService _ApprovalDomainService;
        private IApplicationDomainService _ApplicationDomainService;
        public CashService(IApprovalDomainService ApprovalDomainService, IApplicationDomainService ApplicationDomainService)
        {
            _ApprovalDomainService = ApprovalDomainService;
            _ApplicationDomainService = ApplicationDomainService;
        }
        public async Task<Result<string>> Approval(SignDto data, string cuser, string token) => await _ApprovalDomainService.Approval(data, cuser, token,"web");
        public async Task<Result<string>> CashXApproval(IFormCollection formCollection, string cuser, string token) => await _ApprovalDomainService.CashXApproval(formCollection, cuser, token,"web");
        public async Task<Result<string>> Reject(SignDto data, string cuser, string token) => await _ApprovalDomainService.Reject(data, cuser, token, "web");
        public async Task<Result<List<SignlogDto>>> GetHistoricalSignRecords(string rno) => await _ApprovalDomainService.QueryHistoricalSignRecords(rno);
        public async Task<Result<SignProcessAndLogDto>> GetSignLogs(string rno, string token) => await _ApprovalDomainService.GetSignLogs(rno, token);
        public async Task<Result<List<AlistDto>>> GetSignedData(string rno) => await _ApprovalDomainService.QuerySignedData(rno);
        public async Task<Result<List<ToBeSignedDto>>> QuerySignedFormByUserId(Request<string> parameters, string userId) => await _ApprovalDomainService.QuerySignedFormByUserId(parameters, userId);
        public async Task<Result<List<ToBeSignedDto>>> QuerySignedFormByUserId(Request<string> parameters, string userId, string token) => await _ApprovalDomainService.QuerySignedFormByUserId(parameters, userId, token);
        public async Task<Result<string>> Cancel(string rno, string cuser, string token) => await _ApprovalDomainService.Cancel(rno, cuser, token);
        public async Task<Result<string>> Transform(SignDto data, string cuser, string token) => await _ApprovalDomainService.Transform(data, cuser, token);
        //public async Task<Result<IList<FinReviewDto>>> Transform1(string company) => await _ApprovalDomainService.GetFinance(company);
        public async Task<bool> IsAccountant1Sign(string rno, string cuser) => await _ApprovalDomainService.GetIsAccountant1Sign(rno, cuser);
        public async Task<bool> SendMail(string msg, string subject) => await _ApprovalDomainService.SendMail(msg, subject);
    }
}
