using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Entities;
using Volo.Abp.Domain.Repositories;

namespace ERS.IRepositories
{
    public interface IEmpchsRepository : IRepository<Empchs, Guid>
    {
        Task<List<Empchs>> GetAllEmpchsList();
    }
}