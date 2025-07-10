using System;
using System.Collections.Generic;
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
    public class BDExpenseDeptRepository : EfCoreRepository<ERSDbContext, BDExpenseDept, Guid>, IBDExpenseDeptRepository
    {
        public BDExpenseDeptRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<BDExpenseDept> GetBDExpenseDept(string company, string deptid, string isvirtualdept){
          return await (await GetDbSetAsync()).Where(w => w.company == company && w.deptid == deptid && w.isvirtualdept == isvirtualdept).FirstOrDefaultAsync();
        }

        public async Task<List<BDExpenseDept>> GetBDExpenseDeptByIds(List<Guid?> Ids){
            return await (await GetDbSetAsync()).Where(w => Ids.Contains(w.Id)).ToListAsync();
        }
    }
}