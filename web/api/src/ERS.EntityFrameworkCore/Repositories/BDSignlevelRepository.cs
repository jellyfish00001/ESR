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
    public class BDSignlevelRepository : EfCoreRepository<ERSDbContext, BDSignlevel, Guid>, IBDSignlevelRepository
    {
        public BDSignlevelRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }//Task<BDSignlevel> GetBDSignlevel(string company);

        public async Task<BDSignlevel> GetBDSignlevelByCompany(string company)
        {
            return await (await GetDbSetAsync()).Where(w => w.company == company).FirstOrDefaultAsync();
        }

        public async Task<BDSignlevel> GetBDSignlevel(string company, string item, string signlevel)
        {
            return await (await GetDbSetAsync()).Where(w => w.company == company && w.item == item && w.signlevel == signlevel).FirstOrDefaultAsync();
        }

        public async Task<List<BDSignlevel>> GetBDSignlevelByIds(List<Guid?> Ids){
            return await (await GetDbSetAsync()).Where(w => Ids.Contains(w.Id)).ToListAsync();
        }

        public async Task<List<BDSignlevel>> GetBDSignlevelByCompanyandMoney(string company, decimal money)
        {
            return await (await GetDbSetAsync()).Where(w => w.company == company && w.money <= money).ToListAsync();
        }
    }
}