using ERS.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace ERS.IRepositories
{
    public interface IBDInvoiceRailRepository : IRepository<BDInvoiceRail, Guid>
    {
        Task<List<BDInvoiceRail>> GetBDInvoiceRailList();
        Task<BDInvoiceRail> GetBDInvoiceRailById(Guid Id);
        Task<List<BDInvoiceRail>> GetBDInvoiceRailListByIds(List<Guid> Ids);
    }
}
