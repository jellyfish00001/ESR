using ERS.Entities;
using ERS.EntityFrameworkCore;
using ERS.IRepositories;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace ERS.Repositories
{
    public class CashUberHeadRepository : EfCoreRepository<ERSDbContext, CashUberHead, Guid>, ICashUberHeadRepository
    {
        public CashUberHeadRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }
        public async Task<CashUberHead> GetByNo(string rno) =>  (await GetDbSetAsync()).Where(i => i.Rno == rno).FirstOrDefault();
    }
}
