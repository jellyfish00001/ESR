using ERS.Entities.Uber;
using ERS.EntityFrameworkCore;
using ERS.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace ERS.Repositories
{
    public class UberTransactionalDayRepository : EfCoreRepository<ERSDbContext, UberTransactionalDay, Guid>, IUberTransactionalDayRepository
    {
        public UberTransactionalDayRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }

        public async  Task<List<UberTransactionalDay>> GetUberTransactionalDayToSign()
        {
            return (await GetDbSetAsync()).Where( w => w.rno ==  null && w.SignStatus=="N").ToList();
        }
    }
}
