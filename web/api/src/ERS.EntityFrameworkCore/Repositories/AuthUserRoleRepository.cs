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
    public class AuthUserRoleRepository : EfCoreRepository<ERSDbContext, AuthUserRole, Guid>, IAuthUserRoleRepository
    {
        public AuthUserRoleRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }

        public async Task<List<string>> GetUserRole(string userId)
        {
            return (await GetDbSetAsync())
           .Where(q => q.userkey == userId)
           .Select(q => q.rolekey).Distinct() // 提取 menukey 字段
           .ToList();
        }
    }
}
