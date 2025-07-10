using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERS.Entities;
using ERS.EntityFrameworkCore;
using ERS.IRepositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace ERS.Repositories
{
    public class CashCarryheadRepository : EfCoreRepository<ERSDbContext, CashCarryhead, Guid>, ICashCarryheadRepository
    {
        public CashCarryheadRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<List<CashCarryhead>> GetListByCarryNo(string carryno)
        {
            return (await GetDbSetAsync()).Where(w => w.carryno == carryno).ToList();
        }
    }
}