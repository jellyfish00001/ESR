using ERS.Entities;
using ERS.EntityFrameworkCore;
using ERS.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace ERS.Repositories
{
    public class AuthUserCompanyRepository : EfCoreRepository<ERSDbContext, AuthUserCompany, Guid>, IAuthUserCompanyRepository
    {
        public AuthUserCompanyRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }
        public async Task<List<string>> GetUserCompanys(string userId,string module)
        {
            return (await GetDbSetAsync())
            .Where(q => q.userkey==userId && q.module==module)
            .Select(q => q.company).Distinct() // 提取 company 字段
            .ToList();
        }
    }
}
