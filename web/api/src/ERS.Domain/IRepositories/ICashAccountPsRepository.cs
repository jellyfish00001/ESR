using System;
using System.Collections.Generic;
using ERS.Entities;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
namespace ERS.IRepositories
{
    public interface ICashAccountPsRepository : IRepository<CashAccountPs, Guid>
    {
        Task<IEnumerable<CashAccountPs>> GetCashAccountPsBySql(string sql);
    }
}
