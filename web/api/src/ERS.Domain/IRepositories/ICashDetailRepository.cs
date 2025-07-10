using ERS.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
namespace ERS.IRepositories
{
    public interface ICashDetailRepository : IRepository<CashDetail, Guid>
    {
        Task<IEnumerable<CashDetail>> GetByNo(string rno);
        Task<IList<CashDetail>>  ReadByDeptid(List<string> list,string fromcode);
        Task<List<CashDetail>> GetCashDetail(List<string> rnolist);
        Task<DateTime?> GetDateByAdvrno(string rno);
        Task<IList<CashDetail>> UserDetail(string user,string fromcode);
        Task<CashDetail> GetCashDetailByNo(string rno);
        Task<List<CashDetail>> GetCashDetailsByNo(string rno);
        Task<List<CashDetail>> ReadCashDetailsByNo(string rno);
    }
}
