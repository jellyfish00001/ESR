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
    public class PmcsPjcodeRepository : EfCoreRepository<ERSDbContext, PmcsPjCode, Guid>, IPmcsPjcodeRepository
    {
        public PmcsPjcodeRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {
            
        }

        public async Task<List<PmcsPjCode>> QueryPjcode(string code, string company)
        {
            return await (await GetDbSetAsync()).Where(b => (b.code.StartsWith(code.Trim()) || b.description.StartsWith(code.Trim()))).AsNoTracking().Take(10).ToListAsync();
        }
    }
    
}
