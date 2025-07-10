using ERS.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
namespace ERS.IRepositories
{
    public interface ICashFileRepository : IRepository<CashFile, Guid>
    {
        Task<IEnumerable<CashFile>> GetByNo(string rno);
        Task<IEnumerable<CashFile>> GetByCuser(string cuser);
        Task<IEnumerable<CashFile>> ReadOnlyByCuser(string cuser);
    }
}
