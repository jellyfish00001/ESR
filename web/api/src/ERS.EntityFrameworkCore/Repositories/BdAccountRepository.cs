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
    public class BdAccountRepository : EfCoreRepository<ERSDbContext, BdAccount, Guid>, IBdAccountRepository
    {
        public BdAccountRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<List<string>> GetAcctcodeByCompany(string company)
        {
            return await (await GetDbSetAsync()).Where(w => w.company == company).AsNoTracking().Select(w => w.acctcode).Distinct().ToListAsync();
        }

        public async Task<BdAccount> GetBdAccount(string acctcode, string company)
        {
            return await (await GetDbSetAsync()).Where(w => w.acctcode == acctcode && w.company == company).FirstOrDefaultAsync();
        }
    }
}