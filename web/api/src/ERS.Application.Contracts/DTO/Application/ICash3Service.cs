using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Application.Contracts.DTO.Application;
using ERS.Domain.Entities.Application;
using ERS.DTO.BDExp;
using Microsoft.AspNetCore.Http;
namespace ERS.DTO.Application
{
    public interface ICash3Service
    {
        Task<Result<List<BDExpDto>>> GetAdvanceScene(string input, string company);
        Task<Result<List<BDExpDto>>> GetAllAdvanceScenes(string company);
        Task<Result<List<BDExpDto>>> GetFilecategoryByExpcode(string expcode, string company, int category);
        Task<Result<List<BDPayDto>>> GetBDPayType();
        Task<Result<string>> GetSignAttachment(string rno, string Token);
        Task<Result<CashResult>> Submit(IFormCollection formCollection, string user, string token);
        Task<Result<CashResult>> Keep(IFormCollection formCollection, string user, string token);
        Task<Result<List<OverdueUserDto>>> OverdueUser(string user);
        Task<Result<List<string>>> GetUnreversAdvRno(string word, string company);
        Task<Result<bool>> IsChangeApplicant(string emplid, decimal amount, string company);
        Task<Result<ChangePayeeTipsDto>> ChangePayeeTips(string emplid, decimal amount, string company);
    }
}
