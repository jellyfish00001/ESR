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
    public class AuthRoleApiRepository : EfCoreRepository<ERSDbContext, AuthRoleApi, Guid>, IAuthRoleApiRepository
    {
        public AuthRoleApiRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }
        public async Task<List<string>> GetApis(List<string> roleList)
        {
            return (await GetDbSetAsync())
            .Where(q => roleList.Contains(q.rolekey))
            .Select(q => q.apikey).Distinct() // 提取 apikey 字段
            .ToList();
        }
    }
}
