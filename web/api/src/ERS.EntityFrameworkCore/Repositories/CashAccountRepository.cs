using ERS.DTO.Employee;
using ERS.Entities;
using ERS.EntityFrameworkCore;
using ERS.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace ERS.Repositories
{
    public class CashAccountRepository : EfCoreRepository<ERSDbContext, CashAccount, Guid>, ICashAccountRepository
    {
        public CashAccountRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }
        
        public async Task<CashAccountDto> GetAccount(string user) => await (await GetDbSetAsync()).Where(i => i.emplid == user).Select(i => new CashAccountDto { account = i.account, bank = i.bank }).AsNoTracking().FirstOrDefaultAsync();

        public async Task<List<string>> GetAllBankByCompany(string company) => await (await GetDbSetAsync()).Where(w => w.company == company).Select(w => w.bank).Distinct().ToListAsync();
        public async Task<List<CashAccount>> ReadCAListByEmplid() => await (await GetDbSetAsync()).AsNoTracking().ToListAsync();
        public async Task<List<CashAccount>> GetCashAccountList()
        {
            return await (await GetDbSetAsync()).ToListAsync();
        }
    }
}
