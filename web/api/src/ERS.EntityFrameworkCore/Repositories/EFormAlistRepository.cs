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
    public class EFormAlistRepository : EfCoreRepository<ERSDbContext, EFormAlist, Guid>, IEFormAlistRepository
    {
        public EFormAlistRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }

        public async Task<List<EFormAlist>> GetListByRno(string rno)
        {
            List<EFormAlist> result = (await GetDbSetAsync()).Where(w => w.rno == rno).AsNoTracking().ToList();
            result = result.OrderBy(i => i.step).ThenBy(i => i.cdate).ToList();
            return result;
        }

        public async Task<bool> IsSign(string rno)
        {
            bool result = false;
            var query = (await GetDbSetAsync()).Where(w => w.rno == rno && w.status != "F").ToList();
            if(query.Count > 0)
            {
                result = true;
            }
            return result;
        }
    }
}
