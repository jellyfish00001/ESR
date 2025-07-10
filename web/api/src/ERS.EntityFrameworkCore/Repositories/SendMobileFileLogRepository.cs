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
    public class SendMobileFileLogRepository : EfCoreRepository<ERSDbContext, SendMobileFileLog, Guid>, ISendMobileFileLogRepository
    {
        public SendMobileFileLogRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }

        public async Task<IList<SendMobileFileLog>> GetDataByRno(string rno)
        {
            return (await GetDbSetAsync()).Where(b => b.rno == rno).OrderBy(o=>o.cdate).AsNoTracking().ToList();
        }
    }
}
