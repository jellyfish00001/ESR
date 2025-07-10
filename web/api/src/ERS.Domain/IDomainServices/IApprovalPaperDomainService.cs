using System.Threading.Tasks;
using ERS.DTO;
using ERS.Entities;
using Volo.Abp.Domain.Services;
using ERS.Application.Contracts.DTO.PapreSign;
using System.Collections.Generic;
using ERS.DTO.PapreSign;
namespace ERS.IDomainServices
{
    public interface IApprovalPaperDomainService : IDomainService
    {
        //新增纸本单
        Task<Result<string>> addPaper(string rno, string formcode, string user, string status);
        //根据单号查询纸本单
        Task<ApprovalPaper> queryPaper(string rno);
        //修改纸本单状态
        Task<Result<string>> UpdatePaper(string rno, string formcode, string user, string status);
        // 按签核人工号分页查询待签核的纸本单（含发票）
        Task<Result<List<PaperDto>>> QueryUnsignPaperByemplid(Request<PaperQueryDto> request);
        //纸本单签核
        Task<Result<string>> SignPaper(List<string> rno, string emplid, string token, bool isFinance = false);
        Task<Result<string>> RejectPaper(string rno, string user);
        Task<Result<string>> SignPaper(List<string> rno, string emplid, bool isFinance = false);
    }
}
