using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Domain.Entities.Application;
using ERS.DTO;
using Microsoft.AspNetCore.Http;
using ERS.DTO.BDExp;
using Volo.Abp.Domain.Services;
using ERS.Application.Contracts.DTO.Application;
using ERS.DTO.Application;
using ERS.Entities;
namespace ERS.IDomainServices
{
    public interface ICash3DomainService : IDomainService
    {
        Task<Result<CashResult>> Submit(IFormCollection formCollection, string user, string token, string status = "P");
        // 查询预支场景（模糊查询）
        Task<Result<List<BDExpDto>>> FuzzyQueryScene(string input,string company);
        // 查询预支场景（全部）
        public Task<Result<List<BDExpDto>>> QueryAllScenes(string company);
        // 根据expcode查询附件类型
        Task<Result<List<BDExpDto>>> QueryFilecategoryByExpcode(string expcode, string company, int category);
        // 获取请款方式
        Task<Result<List<BDPayDto>>> QueryBDPayType();
        // 根据单号从bpm获取签呈附件
        Task<Result<string>> GetSignAttachment(string rno, string Token);
        Task<Result<List<OverdueUserDto>>> OverdueUser(string user);
        Task<Result<List<string>>> FuzzyQueryAdvRno(string word, string company);
        Task<Result<bool>> IsChangeApplicant(string emplid, decimal amount, string company);
        Task<Result<ChangePayeeTipsDto>> ChangePayeeTips(string emplid, decimal amount, string company);
        Task<Result<string>> Reversal(List<CashDetail> cashDetails);
        Task Kickback(string rno);
    }
}
