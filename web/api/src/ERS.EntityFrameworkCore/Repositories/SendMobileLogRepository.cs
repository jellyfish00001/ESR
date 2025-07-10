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
    public class SendMobileLogRepository : EfCoreRepository<ERSDbContext, SendMobileLog, Guid>, ISendMobileLogRepository
    {
        public SendMobileLogRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }

        public async Task<SendMobileLog> GetDataByMessageId(string messageId)
        {
            return (await GetDbSetAsync()).Where(b => b.messageid == messageId).AsNoTracking().First();
        }

        public async Task<IList<SendMobileLog>> GetDataByRecallStatusAndRno(string recallstatus, string rno)
        {
            return (await GetDbSetAsync()).Where(b => b.recallstatus == recallstatus && b.rno == rno).AsNoTracking().ToList();
        }

        public async Task<IList<SendMobileLog>> GetDataByRecallStatus(string recallstatus)
        {
            return (await GetDbSetAsync()).Where(b => b.recallstatus == recallstatus).AsNoTracking().ToList();
        }

        public async Task<IList<SendMobileLog>> GetDataBySignstatusAndRno(string signstatus, string rno)
        {
            return (await GetDbSetAsync()).Where(b => b.signstatus == signstatus && b.rno == rno).AsNoTracking().ToList();
        }
    }
}
