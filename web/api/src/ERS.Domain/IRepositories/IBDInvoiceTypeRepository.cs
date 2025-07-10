using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Entities;
using Volo.Abp.Domain.Repositories;
namespace ERS.IRepositories
{
    public interface IBDInvoiceTypeRepository : IRepository<BDInvoiceType, Guid>
    {
        Task<List<BDInvoiceType>> GetBDInvoiceTypeList();
        Task<bool> IsRepeat(string company, string invcode);
        Task<BDInvoiceType> GetBDInvoiceTypeById(Guid Id);
        Task<List<BDInvoiceType>> GetBDInvoiceTypeListByIds(List<Guid> Ids);
        Task<List<BDInvoiceType>> GetInvoiceTypesByCompany(string company);
        Task<List<BDInvoiceType>> ReadElectInvByComapny(string company);
    }
}