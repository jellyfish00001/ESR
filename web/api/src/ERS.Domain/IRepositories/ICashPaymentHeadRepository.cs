using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Entities.Payment;
using Volo.Abp.Domain.Repositories;
namespace ERS.IRepositories
{
    public interface ICashPaymentHeadRepository : IRepository<CashPaymentHead, Guid>
    {
        Task<List<CashPaymentHead>> QueryPayListBySysNo(List<string> sysNos);
    }
}