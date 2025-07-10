using System;
using System.Threading.Tasks;
using ERS.Entities;
using Volo.Abp.Domain.Repositories;
namespace ERS.IRepositories
{
    public interface IBDTaxRateRepository : IRepository<BDTaxRate, Guid>
    {
        Task<string> GetSAPCodeByTaxRate(decimal taxRate);
    }
}