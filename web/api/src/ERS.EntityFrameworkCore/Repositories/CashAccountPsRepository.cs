using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Entities;
using ERS.EntityFrameworkCore;
using ERS.IRepositories;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace ERS.Repositories
{
    public class CashAccountPsRepository : EfCoreRepository<ERSDbContext, CashAccountPs, Guid>, ICashAccountPsRepository
    {
        public CashAccountPsRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {
            
        }

        public async Task<IEnumerable<CashAccountPs>> GetCashAccountPsBySql(string sql)
        {
            return await (await GetDbSetAsync()).FromSqlRaw(sql).ToListAsync();
        }
    }
}
