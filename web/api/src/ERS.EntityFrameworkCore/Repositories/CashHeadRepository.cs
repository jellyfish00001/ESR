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
    public class CashHeadRepository : EfCoreRepository<ERSDbContext, CashHead, Guid>, ICashHeadRepository
    {
        public CashHeadRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }

        public async Task<CashHead> GetByNo(string rno) => await (await GetDbSetAsync()).Where(i => i.rno == rno).FirstOrDefaultAsync();

        //判断单据是否为同一人提交
        public async Task<bool> IsSelfCreate(string rno,string user) => await (await GetDbSetAsync()).Where(b => b.rno == rno&&b.cuser.Equals(user)).FirstOrDefaultAsync()==null?true:false;

        //查詢部門下所有單據
        public async Task<IList<CashHead>> ReadByDeptid(List<string> deptid)
        {
           return (await GetDbSetAsync()).Where(b => deptid.Any( c => c.Equals(b.deptid))).AsNoTracking().ToList();
        }

         public async Task<IList<CashHead>> GetUserHead(string user)
        {
           return (await GetDbSetAsync()).Where(b => b.cuser==user).AsNoTracking().ToList();
        }

        public async Task<IList<CashHead>> GetCashHead(List<string> rnolist)
        {
            return (await GetDbSetAsync()).Where(b => rnolist.Contains(b.rno)).ToList();
        }
        public async Task<CashHead> GetCashHeadByNo(string rno) => await (await GetDbSetAsync()).Where(i => i.rno == rno).FirstOrDefaultAsync();

        public async Task<string> GetPrCodefromCASH_HEAD(string rno)
        {
            var result = string.Empty;
            if(result != null)
            {
                result = await (await GetDbSetAsync()).Where(w => w.rno == rno).AsNoTracking().Select(w => w.projectcode).FirstOrDefaultAsync();
            }
            return result;
        }
        
        public async Task<string> GetBCurrency(string rno)
        {
            var basecurr = (await GetDbSetAsync()).Where(w => w.rno == rno).Select(w => w.currency).FirstOrDefault();
            return basecurr;
        }

        public async Task<List<CashHead>> GetCashHeadsByNo(string rno)
        {
            return await(await GetDbSetAsync()).Where(w => w.rno == rno).ToListAsync();
        }
        public async Task<CashHead> ReadCashHeadsByNo(string rno) => await (await GetDbSetAsync()).Where(w => w.rno == rno).AsNoTracking().FirstOrDefaultAsync();

        public async Task<List<string>> ReadCuserByRno(List<string> rnos) => await (await GetDbSetAsync()).Where(i => rnos.Contains(i.rno)).Select(i => i.cuser).AsNoTracking().Distinct().ToListAsync();
    }
}
