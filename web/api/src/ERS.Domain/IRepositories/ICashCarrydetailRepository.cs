using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Entities;
using Volo.Abp.Domain.Repositories;
namespace ERS.IRepositories
{
    public interface ICashCarrydetailRepository : IRepository<CashCarrydetail, Guid>
    {
        Task<List<CashCarrydetail>> GetListByCarryNo(string carryno);
        Task<bool> IsExistCarryNo(string rno);
    }
}