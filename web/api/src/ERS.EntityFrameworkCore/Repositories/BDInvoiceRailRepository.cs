using System;
using System.Collections.Generic;
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
    public class BDInvoiceRailRepository : EfCoreRepository<ERSDbContext, BDInvoiceRail, Guid>, IBDInvoiceRailRepository
    {
        public BDInvoiceRailRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<BDInvoiceRail> GetBDInvoiceRailById(Guid Id)
        {
            return (await GetDbSetAsync()).Where(w => w.Id == Id).FirstOrDefault();
        }

        public async Task<List<BDInvoiceRail>> GetBDInvoiceRailList()
        {
            return await(await GetDbSetAsync()).AsNoTracking().ToListAsync();
        }

        public async Task<List<BDInvoiceRail>> GetBDInvoiceRailListByIds(List<Guid> Ids)
        {
            return (await GetDbSetAsync()).Where(w => Ids.Contains(w.Id)).ToList();
        }

        public async Task<List<BDInvoiceRail>> GetBDInvoiceRailsByCompany(string company)
        {
            return (await GetDbSetAsync()).Where(w => w.company == company).ToList();
        }

    }
}