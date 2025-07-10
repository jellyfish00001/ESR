using ERS.Entities;
using ERS.EntityFrameworkCore;
using ERS.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace ERS.Repositories
{
    public class EFormAuditRepository : EfCoreRepository<ERSDbContext, EFormAudit, Guid>, IEFormAuditRepository
    {
        public EFormAuditRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }

        public async Task<string> GetAudit(string emplid, string company,string formcode)
        {
            IList<string> code = new List<string>() { formcode, "ALL" };
            return await (await GetDbSetAsync()).Where(i => i.company == company && i.emplid == emplid && code.Contains(i.formcode) && i.sdate<= DateTime.Now && i.edate>= DateTime.Now).AsNoTracking().Select(i => i.auditid).FirstOrDefaultAsync();
        }

        public async Task<EFormAudit> GetAuditById(Guid? Id)
        {
            return await (await GetDbSetAsync()).Where(w => w.Id == Id).FirstOrDefaultAsync();
        }

        public async Task<List<EFormAudit>> GetAuditsByIds(List<Guid?> Ids)
        {
            return await (await GetDbSetAsync()).Where(w => Ids.Contains(w.Id)).ToListAsync();
        }
    }
}
