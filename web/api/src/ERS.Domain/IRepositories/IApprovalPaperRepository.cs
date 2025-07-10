using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Entities;
using Volo.Abp.Domain.Repositories;
namespace ERS.IRepositories
{
    public interface IApprovalPaperRepository : IRepository<ApprovalPaper, Guid>
    {
        Task<IEnumerable<ApprovalPaper>> GetByNo(string rno);
        Task<List<ApprovalPaper>> GetUnsignByEmplid(string emplid);// 根据工号查询未签核纸本单
        Task<bool> UnSigned(string rno);
        Task<ApprovalPaper> GetEFormPaperByNo(string rno);
    }
}
