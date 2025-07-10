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
    public class CashFileRepository : EfCoreRepository<ERSDbContext, CashFile, Guid>, ICashFileRepository
    {
        public CashFileRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }
        public async Task<IEnumerable<CashFile>> GetByNo(string rno) => await (await GetDbSetAsync()).Where(i => i.rno == rno).ToListAsync();

        public async Task<IEnumerable<CashFile>> GetByCuser(string cuser) => await (await GetDbSetAsync()).Where(i => i.cuser == cuser).ToListAsync();
        public async Task<IEnumerable<CashFile>> ReadOnlyByCuser(string cuser) => await (await GetDbSetAsync()).Where(i => i.cuser == cuser).AsNoTracking().ToListAsync();
    }
}
