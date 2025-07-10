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
    public class CashPaymentDetailRepository : EfCoreRepository<ERSDbContext, CashPaymentDetail, Guid>, ICashPaymentDetailRepository
    {
        public CashPaymentDetailRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
        public async Task<bool> IsExistSysNo(string rno)
        {
            bool flag = false;
            var query = (await GetDbSetAsync()).Where(w => w.Rno == rno).ToList();
            if(query.Count > 0)
            {
                flag = true;
            }
            return flag;
        }

        public async Task<List<CashPaymentDetail>> QueryPayListBySysNo(List<string> sysNos)
        {
            var result = (await GetDbSetAsync()).Where(w => sysNos.Contains(w.SysNo)).ToList();
            return result;
        }
    }
}