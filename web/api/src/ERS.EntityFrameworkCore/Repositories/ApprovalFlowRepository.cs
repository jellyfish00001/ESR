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
    public class ApprovalFlowRepository : EfCoreRepository<ERSDbContext, ApprovalFlow, Guid>, IApprovalFlowRepository
    {
        public ApprovalFlowRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<ApprovalFlow> GetApprovalFlowById(Guid? id){
            return await (await GetDbSetAsync()).Where(w => w.Id == id).FirstOrDefaultAsync();
        }
        public async Task<List<ApprovalFlow>> GetApprovalFlowByRno(string rno){
            return await (await GetDbSetAsync()).Where(w => w.rno == rno).ToListAsync();
        }

        public async Task<ApprovalFlow> GetApprovalFlowByRnoStep(string rno, decimal step){
            return await (await GetDbSetAsync()).Where(w => w.rno == rno && w.step == step).FirstOrDefaultAsync();
        }

        public async Task<ApprovalFlow> UpdateNextFlowIdById(Guid flowId, Guid nextFlowId){
            var approvalFlow = await (await GetDbSetAsync()).Where(w => w.Id == flowId).FirstOrDefaultAsync();
            if (approvalFlow != null)
            {
                approvalFlow.nextflowid = nextFlowId;
                return await UpdateAsync(approvalFlow);
            }
            return null;
        }
    }
}