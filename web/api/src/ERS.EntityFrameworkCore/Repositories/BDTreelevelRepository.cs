using System;
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
    public class BDTreelevelRepository : EfCoreRepository<ERSDbContext, BDTreelevel, Guid>, IBDTreelevelRepository
    {
        public BDTreelevelRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
        
        public async Task<BDTreelevel> GetBDTreelevelAll()
        {
            return await (await GetDbSetAsync()).FirstOrDefaultAsync();
        }
        
         public async Task<BDTreelevel> GetBDTreelevelByLevelnum(decimal levelnum)
        {
            return await (await GetDbSetAsync()).Where(w => w.levelnum == levelnum).FirstOrDefaultAsync();
        }

        
    }
}