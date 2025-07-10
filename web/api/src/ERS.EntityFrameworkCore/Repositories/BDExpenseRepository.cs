using ERS.Entities;
using ERS.EntityFrameworkCore;
using ERS.IRepositories;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using System.Reflection.Emit;

namespace ERS.Repositories
{
    public class BDExpenseRepository : EfCoreRepository<ERSDbContext, BDExpense, Guid>, IBDExpenseRepository
    {
        public BDExpenseRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
        public async Task<List<BDExpense>> GetExpenseCodes()
        {
            return (await GetDbSetAsync())
                //.WhereIf(!string.IsNullOrEmpty(company), b => b.company == company)
                .OrderBy(b => b.ExpenseName)
                .ToList();
        }
    }
}
