using ERS.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
namespace ERS.IRepositories
{
    public interface ICashHeadRepository : IRepository<CashHead, Guid>
    {
        Task<CashHead> GetByNo(string rno);
        Task<bool> IsSelfCreate(string rno,string user);
        Task<IList<CashHead>> ReadByDeptid(List<string> deptid);
        Task<IList<CashHead>> GetUserHead(string user);
        Task<IList<CashHead>> GetCashHead(List<string> rnolist);
        Task<CashHead> GetCashHeadByNo(string rno);
        Task<string> GetPrCodefromCASH_HEAD(string rno);
        Task<string> GetBCurrency(string rno);
        Task<List<CashHead>> GetCashHeadsByNo(string rno);
        Task<List<string>> ReadCuserByRno(List<string> rnos);
        Task<CashHead> ReadCashHeadsByNo(string rno);
    }
}
