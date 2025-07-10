using ERS.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace ERS.IRepositories
{
    public interface ISendMobileLogRepository : IRepository<SendMobileLog, Guid>
    {
        Task<IList<SendMobileLog>> GetDataByRecallStatusAndRno(string recallstatus, string rno);

        Task<SendMobileLog> GetDataByMessageId(string messageId);

        Task<IList<SendMobileLog>> GetDataBySignstatusAndRno(string signstatus, string rno);

        Task<IList<SendMobileLog>> GetDataByRecallStatus(string recallstatus);

    }
}
