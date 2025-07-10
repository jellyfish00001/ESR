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
    public class AuthMenuRepository : EfCoreRepository<ERSDbContext, AuthMenu, Guid>, IAuthMenuRepository
    {
        public AuthMenuRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }

        public async Task<List<string>> GetPublicMenus()
        {
            return (await GetDbSetAsync())
            .Where(q => q.ispublic==true)
            .Select(q => q.menukey).Distinct() // 提取 menukey 字段
            .ToList();
        }
    }
}
