using ERS.DTO;
using ERS.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
namespace ERS.IRepositories
{
    public interface IBDMealAreaRepository : IRepository<BDMealArea, Guid>
    {
        Task<IList<string>> GetCity(string company);
        Task<TravelDto> GetExpense(TravelDto travel);
        TravelDto TravelCount(List<BDMealArea> data, TravelDto travel);
    }
}
