using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Entities;
using Volo.Abp.Domain.Repositories;
namespace ERS.IRepositories
{
    public interface ICashCarryheadRepository : IRepository<CashCarryhead, Guid>
    {
        Task<List<CashCarryhead>> GetListByCarryNo(string carryno);
    }
}