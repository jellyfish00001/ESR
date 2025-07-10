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
    public class BDashReturnRepository : EfCoreRepository<ERSDbContext, BDCashReturn, Guid>, IBDashReturnRepository
    {
        public BDashReturnRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<List<BDCashReturn>> GetBDCashReturn()
        {
          return (await GetDbSetAsync()).ToList();
        }
    }
}
