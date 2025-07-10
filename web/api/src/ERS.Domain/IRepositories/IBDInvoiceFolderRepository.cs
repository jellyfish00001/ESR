using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Entities;
using Volo.Abp.Domain.Repositories;
namespace ERS.IRepositories
{
    public interface IBDInvoiceFolderRepository : IRepository<BDInvoiceFolder, Guid>
    {
        Task<bool> IsExist(string invno, string invcode);
        Task<List<BDInvoiceFolder>> GetBDInvoiceFolderList();
        Task<List<string>> GetPayTypeList();
        Task<List<BDInvoiceFolder>> GetInvInfoListById(List<Guid?> ids);
        Task<BDInvoiceFolder> GetInvInfoById(Guid id);
        Task InsertRno(List<Guid?> ids, string rno);
        Task CancelRno(List<Guid?> ids);
        Task CancelRno(string rno);
        Task<bool> CheckInvoiceIsAbnormal(List<Guid?> ids, string cuser);
        Task<bool> CheckInvoiceExistAutopa(string invno, string invcode);
        Task<bool> CheckInvNoDuplicate(string invno,string id);
    }
}