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
    public class EFormHeadRepository : EfCoreRepository<ERSDbContext, EFormHead, Guid>, IEFormHeadRepository
    {
        public EFormHeadRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }
        public async Task<EFormHead> GetByNo(string rno) => await (await GetDbSetAsync()).Where(i => i.rno == rno).FirstOrDefaultAsync();

        //判断单据签核状态是否为进行中
        public async Task<bool> IsSigned(string rno) => await (await GetDbSetAsync()).Where(b => b.rno == rno && (b.status.Equals("P") || b.status.Equals("A"))).FirstOrDefaultAsync() == null ? false : true;

        public async Task<EFormHead> GetByCuser(string cuser) => await (await GetDbSetAsync()).Where(i => i.cuser == cuser).FirstOrDefaultAsync();

        public async Task<IList<EFormHead>> GetEFormHeadList(List<string> rnolist)
        {
            return (await GetDbSetAsync()).Where(b => rnolist.Contains(b.rno)).ToList();
        }
    }
}
