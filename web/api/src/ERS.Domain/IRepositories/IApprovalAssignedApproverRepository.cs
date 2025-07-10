using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Entities;
using Volo.Abp.Domain.Repositories;

namespace ERS.IRepositories
{
    public interface IApprovalAssignedApproverRepository: IRepository<ApprovalAssignedApprover, Guid>
    {
        Task<ApprovalAssignedApprover> GetApprovalAssignedApproverById(Guid? id);
    }
}