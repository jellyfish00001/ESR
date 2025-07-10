using ERS.Entities;
using ERS.EntityFrameworkCore;
using ERS.IRepositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace ERS.Repositories
{
    public class CashUberDetailRepository : EfCoreRepository<ERSDbContext, CashUberDetail, Guid>, ICashUberDetailRepository
    {
        public CashUberDetailRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }

        public async Task<List<CashUberDetail>> GetCashUberDetailsByNo(string rno)
        {
            var result = (await GetDbSetAsync()).Where(w => w.Rno == rno).ToList();
            return result;
        }
    }
}
