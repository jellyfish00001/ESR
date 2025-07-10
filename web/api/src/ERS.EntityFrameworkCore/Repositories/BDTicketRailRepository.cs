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
    public class BDTicketRailRepository : EfCoreRepository<ERSDbContext, BDTicketRail, Guid>, IBDTicketRailRepository
    {
        public BDTicketRailRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<BDTicketRail> GetBDTicketRailById(Guid Id)
        {
            return (await GetDbSetAsync()).Where(w => w.Id == Id).FirstOrDefault();
        }

        public async Task<List<BDTicketRail>> GetBDTicketRailList()
        {
            return await(await GetDbSetAsync()).AsNoTracking().ToListAsync();
        }

        public async Task<List<BDTicketRail>> GetBDTicketRailListByIds(List<Guid> Ids)
        {
            return (await GetDbSetAsync()).Where(w => Ids.Contains(w.Id)).ToList();
        }

        public async Task<List<BDTicketRail>> GetBDTicketRailsByCompany(string company)
        {
            return (await GetDbSetAsync()).Where(w => w.company == company).ToList();
        }

        public async Task<BDTicketRail> GetBDTicketRailsByCompanyAndVoucheryearAndVouchermonth(string company, string voucheryear, string vouchermonth)
        {
            return (await GetDbSetAsync()).Where(w => w.company == company && w.voucheryear == voucheryear && w.vouchermonth == vouchermonth).FirstOrDefault();
        }
    }
}