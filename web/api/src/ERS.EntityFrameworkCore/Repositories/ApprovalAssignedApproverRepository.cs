using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERS.Entities;
using ERS.EntityFrameworkCore;
using ERS.IRepositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ERS.Repositories
{
    public class ApprovalAssignedApproverRepository : EfCoreRepository<ERSDbContext, ApprovalAssignedApprover, Guid>, IApprovalAssignedApproverRepository
    {
        public ApprovalAssignedApproverRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<ApprovalAssignedApprover> GetApprovalAssignedApproverById(Guid? id)
        {
            return await (await GetDbSetAsync()).Where(w => w.Id == id).FirstOrDefaultAsync();
        }
    }
}