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
    public class BDInvoiceTypeRepository : EfCoreRepository<ERSDbContext, BDInvoiceType, Guid>, IBDInvoiceTypeRepository
    {
        public BDInvoiceTypeRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<List<BDInvoiceType>> GetBDInvoiceTypeList()
        {
            return await (await GetDbSetAsync()).AsNoTracking().ToListAsync();
        }

        public async Task<bool> IsRepeat(string company, string invcode)
        {
            return (await GetDbSetAsync()).Any(w => w.InvTypeCode == invcode && w.company == company);
        }

        public async Task<BDInvoiceType> GetBDInvoiceTypeById(Guid Id)
        {
            return (await GetDbSetAsync()).Where(w => w.Id == Id).FirstOrDefault();
        }

        public async Task<List<BDInvoiceType>> GetBDInvoiceTypeListByIds(List<Guid> Ids)
        {
            return (await GetDbSetAsync()).Where(w => Ids.Contains(w.Id)).ToList();
        }

        public async Task<List<BDInvoiceType>> GetInvoiceTypesByCompany(string company)
        {
            return (await GetDbSetAsync()).Where(w => w.company == company).ToList();
        }
        public async Task<List<BDInvoiceType>> ReadElectInvByComapny(string company) => await (await GetDbSetAsync()).Where(i => i.company == company).AsNoTracking().ToListAsync();
    }
}