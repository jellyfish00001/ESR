using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using ERS.Entities;
using ERS.IRepositories;
using ERS.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;



namespace ERS.Repositories
{
    public class ApprovalPaperRepository : EfCoreRepository<ERSDbContext, ApprovalPaper, Guid>, IApprovalPaperRepository
    {
        public ApprovalPaperRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }

        public async Task<IEnumerable<ApprovalPaper>> GetByNo(string rno) => await (await GetDbSetAsync()).Where(i => i.rno == rno).ToListAsync();

        public async Task<ApprovalPaper> GetEFormPaperByNo(string rno) => await (await GetDbSetAsync()).Where(i => i.rno == rno).FirstOrDefaultAsync();

        public async Task<List<ApprovalPaper>> GetUnsignByEmplid(string emplid) => await (await GetDbSetAsync()).Where(i => i.emplid == emplid && i.status == "P").OrderBy(i => i.cdate).ToListAsync();

        public async Task<bool> UnSigned(string rno) => await (await GetDbSetAsync()).Where(b => b.rno == rno && b.status == "P").AsNoTracking().FirstOrDefaultAsync() == null ? false : true;//判断是否签核
    }
}
