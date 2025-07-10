using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Entities;
using Volo.Abp.Domain.Repositories;

namespace ERS.IRepositories
{
    public interface IApprovalFlowRepository: IRepository<ApprovalFlow, Guid>
    {
        Task<ApprovalFlow> GetApprovalFlowById(Guid? id);
        Task<List<ApprovalFlow>> GetApprovalFlowByRno(string rno);
        Task<ApprovalFlow> GetApprovalFlowByRnoStep(string rno, decimal step);
        Task<ApprovalFlow> UpdateNextFlowIdById(Guid flowId, Guid nextFlowId);
    }
}