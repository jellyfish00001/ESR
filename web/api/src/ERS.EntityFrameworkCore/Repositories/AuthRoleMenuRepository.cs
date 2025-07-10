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
    public class AuthRoleMenuRepository : EfCoreRepository<ERSDbContext, AuthRoleMenu, Guid>, IAuthRoleMenuRepository
    {
        public AuthRoleMenuRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }

        public async Task<List<string>> GetRoleMenus(List<string> roleList)
        {
            return (await GetDbSetAsync())
            .Where(q => roleList.Contains(q.rolekey))
            .Select(q => q.menukey).Distinct() // 提取 menukey 字段
            .ToList();
        }
    }
}
