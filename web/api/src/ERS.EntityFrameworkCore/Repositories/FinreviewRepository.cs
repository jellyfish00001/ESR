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
    public class FinreviewRepository : EfCoreRepository<ERSDbContext, Finreview, Guid>, IFinreviewRepository
    {
        public FinreviewRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }

        public async Task<IList<Finreview>> GetFinanceByPlant(string company, string plant)
        {
            return await (await GetDbSetAsync()).Where(i => (i.plant == plant || i.plant == "OTHERS") && i.company == company && i.category == 0).AsNoTracking().ToListAsync();
        }

        public async Task<IList<Finreview>> ReadOnlyByCompany(List<string> companys) => await (await GetDbSetAsync()).Where(i => companys.Contains(i.company)).AsNoTracking().ToListAsync();

        public async Task<bool> IsAccountantOrNot(string userId)
        {
            bool result = false;
            var query = (await GetDbSetAsync()).Where(w => w.rv1.Contains(userId) || w.rv2.Contains(userId) || w.rv2.Contains(userId)).ToList();
            if(query.Count > 0)
            {
                result = true;
            }
            return result;
        }

        public async Task<Finreview> GetFinreviewById(Guid? Id)
        {
            return await (await GetDbSetAsync()).Where(w => w.Id == Id).FirstOrDefaultAsync();
        }

        public async Task<List<Finreview>> GetFinreviewsByIds(List<Guid?> Ids)
        {
            return await (await GetDbSetAsync()).Where(w => Ids.Contains(w.Id)).ToListAsync();
        }

        public async Task<IList<Finreview>> GetCashXFinanceByCompany(string companycode)
        {
            return await (await GetDbSetAsync()).Where(w => w.company_code == companycode && w.category == 1).AsNoTracking().ToListAsync();
        }
    }
}
