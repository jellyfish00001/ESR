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
    public class InvoiceRepository : EfCoreRepository<ERSDbContext, Invoice, Guid>, IInvoiceRepository
    {
        public InvoiceRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }
        public async Task<IEnumerable<Invoice>> GetByNo(string rno) => await (await GetDbSetAsync()).Where(i => i.rno == rno).ToListAsync();
        public async Task<bool> CheckIsExistFolder(Guid id) => await (await GetDbSetAsync()).Where(i => i.invoiceid == id).CountAsync() > 0;
        public async Task<IList<Invoice>> ReadDetailsByNo(string rno) => await (await GetDbSetAsync()).Where(i => i.rno == rno).AsNoTracking().ToListAsync();
    }
}
