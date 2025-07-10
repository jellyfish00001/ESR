

using ERS.Entities;
using ERS.EntityFrameworkCore;
using ERS.IRepositories;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;

using Volo.Abp.EntityFrameworkCore;

namespace ERS.Repositories
{

    public class BDCompanyCategoryRepository : EfCoreRepository<ERSDbContext, BDCompanyCategory, Guid>, IBDCompanyCategoryRepository
    {
        public BDCompanyCategoryRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<string> QueryAreaByCompanyCategory(string CompanyCategory)
        {
            return  await (await GetDbSetAsync()).Where(i => i.CompanyCategory == CompanyCategory).AsNoTracking().Select(i => i.Area).FirstOrDefaultAsync();
        }
    }
}