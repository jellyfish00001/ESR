using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERS.Entities.Payment;
using ERS.EntityFrameworkCore;
using ERS.IRepositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace ERS.Repositories
{
    public class CashPaymentHeadRepository : EfCoreRepository<ERSDbContext, CashPaymentHead, Guid>, ICashPaymentHeadRepository
    {
        public CashPaymentHeadRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
  
        public async Task<List<CashPaymentHead>> QueryPayListBySysNo(List<string> sysNos)
        {
            var result = (await GetDbSetAsync()).Where(w => sysNos.Contains(w.SysNo)).ToList();
            return result;
        }
    }
}