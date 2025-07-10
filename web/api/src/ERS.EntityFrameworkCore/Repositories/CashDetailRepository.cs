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
    public class CashDetailRepository : EfCoreRepository<ERSDbContext, CashDetail, Guid>, ICashDetailRepository
    {
        public CashDetailRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }
        public async Task<IEnumerable<CashDetail>> GetByNo(string rno) => await (await GetDbSetAsync()).Where(i => i.rno == rno).ToListAsync();

                //找到傳入部門的所有人提交的單據
        public  async Task<IList<CashDetail>>  ReadByDeptid(List<string> list,string fromcode)
        {
            return (await GetDbSetAsync()).Where(b => b.formcode==fromcode && list.Any(c => c.Equals(b.deptid))).AsNoTracking().ToList();
        }

        
        public  async Task<IList<CashDetail>> UserDetail(string user,string fromcode)
        {
            return (await GetDbSetAsync()).Where(b => b.cuser == user && b.formcode == fromcode).AsNoTracking().ToList();
        }

        public async Task<List<CashDetail>> GetCashDetail(List<string> rnolist)
        {
            return (await GetDbSetAsync()).Where(b => rnolist.Contains(b.rno)).ToList();
        }
        
        //根据单号获取cdate
        public async Task<DateTime?> GetDateByAdvrno(string rno)
        {
            return (await GetDbSetAsync()).Where(b => b.rno == rno).Select(x => x.cdate).FirstOrDefault();
        }

        public async Task<CashDetail> GetCashDetailByNo(string rno) => await (await GetDbSetAsync()).Where(i => i.rno == rno).FirstOrDefaultAsync();

        public async Task<List<CashDetail>> GetCashDetailsByNo(string rno)
        {
            var result = (await GetDbSetAsync()).Where(w => w.rno == rno).ToList();
            return result;
        }
        public async Task<List<CashDetail>> ReadCashDetailsByNo(string rno) => await (await GetDbSetAsync()).Where(w => w.rno == rno).AsNoTracking().ToListAsync();
    }
}
