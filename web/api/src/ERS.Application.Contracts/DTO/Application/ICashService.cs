using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO.Approval;
using Microsoft.AspNetCore.Http;
namespace ERS.DTO.Application
{
    public interface ICashService
    {
        Task<Result<string>> Approval(SignDto data, string cuser, string token);
        Task<Result<string>> Reject(SignDto data, string cuser, string token);
        Task<Result<List<SignlogDto>>> GetHistoricalSignRecords(string rno);
        Task<Result<SignProcessAndLogDto>> GetSignLogs(string rno, string token);
        Task<Result<List<AlistDto>>> GetSignedData(string rno);
        Task<Result<List<ToBeSignedDto>>> QuerySignedFormByUserId(Request<string> parameters, string userId);
        Task<Result<List<ToBeSignedDto>>> QuerySignedFormByUserId(Request<string> parameters, string userId, string token);
        Task<Result<string>> Cancel(string rno, string cuser, string token);
        Task<Result<string>> Transform(SignDto data, string cuser, string token);
        //Task<Result<IList<FinReviewDto>>> Transform1(string company);
        Task<bool> IsAccountant1Sign(string rno, string cuser);
        Task<Result<string>> CashXApproval(IFormCollection formCollection, string cuser, string token);
        Task<bool> SendMail(string msg, string subject);
    }
}
