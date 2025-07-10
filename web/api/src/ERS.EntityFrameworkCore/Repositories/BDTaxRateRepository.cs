using System;
using System.Linq;
using System.Threading.Tasks;
using ERS.Entities;
using ERS.EntityFrameworkCore;
using ERS.IRepositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace ERS.Repositories
{
    public class BDTaxRateRepository : EfCoreRepository<ERSDbContext, BDTaxRate, Guid>, IBDTaxRateRepository
    {
        public BDTaxRateRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<string> GetSAPCodeByTaxRate(decimal taxRate)
        {
            return (await GetDbSetAsync()).Where(w => w.taxrate == taxRate).FirstOrDefault()?.sapcode;
        }
    }
}