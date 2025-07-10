using ERS.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace ERS.IRepositories
{
    public interface IBDTicketRailRepository : IRepository<BDTicketRail, Guid>
    {
        Task<List<BDTicketRail>> GetBDTicketRailList();
        Task<BDTicketRail> GetBDTicketRailById(Guid Id);
        Task<List<BDTicketRail>> GetBDTicketRailListByIds(List<Guid> Ids);
        Task<List<BDTicketRail>> GetBDTicketRailsByCompany(string company);
        Task<BDTicketRail> GetBDTicketRailsByCompanyAndVoucheryearAndVouchermonth(string company, string voucheryear, string vouchemonth);
    }
}
