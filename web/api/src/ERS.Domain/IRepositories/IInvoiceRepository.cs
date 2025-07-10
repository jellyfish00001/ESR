using ERS.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace ERS.IRepositories
{
    public interface IInvoiceRepository : IRepository<Invoice, Guid>
    {
        Task<IEnumerable<Invoice>> GetByNo(string rno);
        Task<bool> CheckIsExistFolder(Guid id);
        Task<IList<Invoice>> ReadDetailsByNo(string rno);
    }
}
