using ERS.Entities.Uber;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
namespace ERS.IRepositories
{
    public interface IUberTransactionalDayRepository : IRepository<UberTransactionalDay, Guid>
    {
        Task<List<UberTransactionalDay>> GetUberTransactionalDayToSign();
    }
}
