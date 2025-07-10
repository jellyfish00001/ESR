using ERS.DTO;
using ERS.DTO.Approval;
using ERS.Entities;
using ERS.Model;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
namespace ERS.IDomainServices
{
    public interface IApprovalDomainService : IDomainService
    {
        Task<Result<string>> Reject(SignDto data, string cuser, string token, string devicetype);
        Task<Result<string>> Approval(SignDto data, string cuser, string token,string devicetype);
        Signs AddPayeeSign(string cuser, string payeeid);
        Task<IList<Signs>> AddCostSignBefore(CashHead list, int category = 1);
        Task<IList<Signs>> AddFinanceSign(CashHead list, bool invoiceAbnormal);
        Task<IList<Signs>> AddDefinePermissionSign(CashHead list, string token);
        Task<IList<Signs>> AddCostSignAfter(CashHead list, int category = 1);
        Task<Result<string>> CreateSignSummary(CashHead list, bool invoiceAbnormal, string token);
        Task<Result<List<SignlogDto>>> QueryHistoricalSignRecords(string rno);
        Task<Result<List<AlistDto>>> QuerySignedData(string rno);
        Task<Result<List<ToBeSignedDto>>> QuerySignedFormByUserId(Request<string> parameters,string userId);
        Task<Result<string>> Cancel(string rno, string cuser, string token);
        Task<Result<string>> Transform(SignDto data, string cuser, string token);
        Task<Result<IList<FinReviewDto>>> GetFinance(string user);
        Task<bool> GetIsAccountant1Sign(string rno, string cuser);
        Task<Result<SignProcessAndLogDto>> GetSignLogs(string rno, string token);
        Task<Result<bool>> IsAccountantOrNot(string user);
        Task<Result<List<ToBeSignedDto>>> QuerySignedFormByUserId(Request<string> parameters, string userId, string token);
        Task<Result<int>> IsApplicant(string rno, string user, string token);
        Task<Result<string>> CashXApproval(IFormCollection formCollection, string cuser, string token, string devicetype);
        Task<bool> SendMail(string msg,string subject);
    }
}
