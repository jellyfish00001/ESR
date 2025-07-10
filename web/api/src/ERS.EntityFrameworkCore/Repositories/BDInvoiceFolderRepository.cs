using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERS.Entities;
using ERS.EntityFrameworkCore;
using ERS.IRepositories;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace ERS.Repositories
{
    public class BDInvoiceFolderRepository : EfCoreRepository<ERSDbContext, BDInvoiceFolder, Guid>, IBDInvoiceFolderRepository
    {
        public BDInvoiceFolderRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<bool> IsExist(string invno, string invcode)
        {
            return (await GetDbSetAsync()).Where(w => w.invno == invno && w.invcode == invcode).ToList().Count > 0;
        }
        public async Task<List<BDInvoiceFolder>> GetBDInvoiceFolderList()
        {
            return await (await GetDbSetAsync()).AsNoTracking().ToListAsync();
        }
        public async Task<List<string>> GetPayTypeList()
        {
            return await (await GetDbSetAsync()).Select(w => w.paytype).Where(i => !string.IsNullOrEmpty(i)).Distinct().AsNoTracking().ToListAsync();
        }
        public async Task<List<BDInvoiceFolder>> GetInvInfoListById(List<Guid?> ids)
        {
            return await (await GetDbSetAsync()).Where(w => ids.Contains(w.Id)).ToListAsync();
        }
        public async Task<BDInvoiceFolder> GetInvInfoById(Guid id)
        {
            return await (await GetDbSetAsync()).Where(w => w.Id == id).FirstOrDefaultAsync();
        }

        public async Task InsertRno(List<Guid?> ids, string rno)
        {
            if (ids.Count == 0) return;
            List<BDInvoiceFolder> data = await (await GetDbSetAsync()).Where(i => ids.Contains(i.Id)).ToListAsync();
            foreach(BDInvoiceFolder item in data)
            {
                item.rno = rno;
                item.paytype = ERSConsts.InvoicePayTypeEnum.Requested.ToValue();
            }
            if (data.Count > 0)
                (await GetDbSetAsync()).UpdateRange(data);
        }
        public async Task CancelRno(List<Guid?> ids)
        {
            if (ids.Count == 0) return;
            List<BDInvoiceFolder> data = await (await GetDbSetAsync()).Where(i => ids.Contains(i.Id)).ToListAsync();
            foreach (BDInvoiceFolder item in data)
            {
                item.rno = "";
                item.paytype = ERSConsts.InvoicePayTypeEnum.Unrequested.ToValue();
            }
            if (data.Count > 0)
                (await GetDbSetAsync()).UpdateRange(data);
        }
        public async Task CancelRno(string rno)
        {
            List<BDInvoiceFolder> data = await (await GetDbSetAsync()).Where(i => i.rno == rno).ToListAsync();
            foreach (BDInvoiceFolder item in data)
            {
                item.rno = "";
                item.paytype = ERSConsts.InvoicePayTypeEnum.Unrequested.ToValue();
            }
            if (data.Count > 0)
                (await GetDbSetAsync()).UpdateRange(data);
        }
        /// <summary>
        /// 选择的发票是否存在异常（已删、已报、已分享、已编辑）
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="cuser"></param>
        /// <returns></returns>
        public async Task<bool> CheckInvoiceIsAbnormal(List<Guid?> ids, string cuser)
        {
            if (ids.Count == 0) return false;
            List<BDInvoiceFolder> data = await (await GetDbSetAsync()).Where(i => ids.Contains(i.Id)).AsNoTracking().ToListAsync();
            if (data.Count != ids.Count) return true;
            return data.Where(i => !string.IsNullOrEmpty(i.rno) || i.emplid != cuser || i.isedit).Count() > 0;
        }

        public async Task<bool> CheckInvoiceExistAutopa(string invno, string invcode) => string.IsNullOrEmpty(invcode) ? await (await GetDbSetAsync()).Where(i => i.invno == invno && i.existautopa).CountAsync() > 0 : await (await GetDbSetAsync()).Where(i => i.invno == invno && i.invcode == invcode && i.existautopa).CountAsync() > 0;

        public async Task<bool> CheckInvNoDuplicate(string invno, string id)
        {
            return (await GetDbSetAsync()).Where(w => w.invno == invno && w.Id.ToString() != id).ToList().Count > 0;
        }
    }
}