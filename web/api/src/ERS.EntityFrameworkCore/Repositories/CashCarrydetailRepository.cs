using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERS.Entities;
using ERS.EntityFrameworkCore;
using ERS.IRepositories;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace ERS.Repositories
{
    public class CashCarrydetailRepository : EfCoreRepository<ERSDbContext, CashCarrydetail, Guid>, ICashCarrydetailRepository
    {
        public CashCarrydetailRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<List<CashCarrydetail>> GetListByCarryNo(string carryno)
        {
            return (await  GetDbSetAsync()).Where(w => w.carryno == carryno).ToList();
        }

        public async Task<bool> IsExistCarryNo(string rno)
        {
            return (await GetDbSetAsync()).Where(w => w.rno == rno && w.rnostatus == "N").AsNoTracking().Count() > 0;
        }
    }
}