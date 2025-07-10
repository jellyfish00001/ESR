using ERS.Entities;
using System;
using Volo.Abp.Domain.Repositories;

namespace ERS.IRepositories
{
    public interface IMobileCallBackLogRepository : IRepository<MobileCallBackLog, Guid>
    {
       
    }
}
