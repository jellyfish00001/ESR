using System;
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
    public class EmployeeRepository : EfCoreRepository<ERSDbContext, Employee, Guid>, IEmployeeRepository
    {
        public EmployeeRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<string> GetCnameByEmplid(string emplid)
        {
            return (await GetDbSetAsync()).Where(w => w.emplid == emplid).Select(w => w.cname).FirstOrDefault();
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

        public async Task<string> GetCompanyCodeByUser(string emplid)
        {
            return (await GetDbSetAsync()).Where(w => w.emplid == emplid).Select(w => w.company).FirstOrDefault();
        }

        public async Task<bool> CheckIsLeft(string emplid) => await (await GetDbSetAsync()).Where(i => i.emplid == emplid && string.IsNullOrEmpty(i.tdate)).AsNoTracking().CountAsync() > 0;
    }
}
