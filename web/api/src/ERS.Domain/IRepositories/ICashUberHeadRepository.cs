using ERS.Entities;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
namespace ERS.IRepositories
{
    public interface ICashUberHeadRepository : IRepository<CashUberHead, Guid>
    {
        Task<CashUberHead> GetByNo(string rno);
    }
}
