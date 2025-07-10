using ERS.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace ERS.IRepositories
{
    public interface ISendMobileFileLogRepository : IRepository<SendMobileFileLog, Guid>
    {
        Task<IList<SendMobileFileLog>> GetDataByRno(string rno);
    }
}
