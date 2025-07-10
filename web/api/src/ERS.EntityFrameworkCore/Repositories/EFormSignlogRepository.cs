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
    public class EFormSignlogRepository : EfCoreRepository<ERSDbContext, EFormSignlog, Guid>, IEFormSignlogRepository
    {
        public EFormSignlogRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }

        public async Task<List<EFormSignlog>> GetListByRno(string rno)
        {
            var result =  (await GetDbSetAsync()).Where(w => w.rno == rno).AsNoTracking().ToList();
            result = result.OrderBy(w => w.step).ThenBy(i => i.cdate).ToList();
            return result;
        }
    }
}
