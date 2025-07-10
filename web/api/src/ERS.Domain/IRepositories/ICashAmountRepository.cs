using ERS.DTO;
using ERS.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
namespace ERS.IRepositories
{
    public interface ICashAmountRepository : IRepository<CashAmount, Guid>
    {
        Task<CashAmount> GetByNo(string rno);
        Task<IList<CashAmount>> ReadByRno(List<string> rno);
        Task<IList<CashAmount>> GetRnoAll(List<string> rno);
        Task<IList<CashAmount>> GetuserAll(string user);
        Task<CashAmount> GetCashAmountByNo(string rno);
        Task<Result<string>> Reversal(CashHead cash);
        Task Kickback(string rno);
    }
}
