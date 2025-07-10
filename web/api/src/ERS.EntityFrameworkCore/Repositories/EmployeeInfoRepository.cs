using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERS.Entities;
using ERS.EntityFrameworkCore;
using ERS.IRepositories;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace ERS.Repositories
{
    public class EmployeeInfoRepository : EfCoreRepository<ERSDbContext, EmployeeInfo, Guid>, IEmployeeInfoRepository
    {
        public EmployeeInfoRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<List<EmployeeInfo>> QueryByEmplidOrEngName(string keyword)
        {
            return await (await GetDbSetAsync()).Where(w => w.emplid.Contains(keyword) || w.name_a.ToLower().Contains(keyword.ToLower()) || w.name.ToLower().Contains(keyword)).AsNoTracking().Take(10).ToListAsync();
        }

        public async Task<bool> EmpIsExist(string emplid)
        {
            bool result = false;
            var query = (await GetDbSetAsync()).Where(w => w.emplid == emplid).FirstOrDefault();
            if(query != null)
            {
                result = true;
            }
            return result;
        }

        public async Task<EmployeeInfo> QueryByEmail(string email)
        {
            var query = (await GetDbSetAsync())
                      .Where(w => w.email_address_a.ToUpper() == email.ToUpper())
                      .FirstOrDefault();
            return query;
        }

        public async Task<EmployeeInfo> QueryByEmplid(string emplid)
        {
            var query = (await GetDbSetAsync())
                      .Where(w => w.emplid == emplid)
                      .FirstOrDefault();
            return query;
        }
    }
}
