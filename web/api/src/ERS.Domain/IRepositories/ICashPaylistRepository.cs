using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Entities.Payment;
using Volo.Abp.Domain.Repositories;
namespace ERS.IRepositories
{
    public interface ICashPaylistRepository : IRepository<CashPaymentDetail, Guid>
    {
        Task<bool> IsExistSysNo(string rno);
        Task<List<CashPaymentDetail>> QueryPayListBySysNo(List<string> sysNos);
    }
}