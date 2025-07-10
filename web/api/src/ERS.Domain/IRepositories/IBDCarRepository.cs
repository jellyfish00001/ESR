using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Entities;
using Volo.Abp.Domain.Repositories;
namespace ERS.IRepositories
{
    public interface IBDCarRepository : IRepository<BDCar,Guid>
    {
        Task<List<BDCar>> QueryBDCarByCompany(string company);
    }
}