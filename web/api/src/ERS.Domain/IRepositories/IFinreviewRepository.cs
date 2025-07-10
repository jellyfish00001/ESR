using ERS.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace ERS.IRepositories
{
    public interface IFinreviewRepository : IRepository<Finreview, Guid>
    {
        Task<IList<Finreview>> GetFinanceByPlant(string company, string plant);
        Task<IList<Finreview>> ReadOnlyByCompany(List<string> company);
        Task<bool> IsAccountantOrNot(string userId);
        Task<Finreview> GetFinreviewById(Guid? Id);
        Task<List<Finreview>> GetFinreviewsByIds(List<Guid?> Ids);
        Task<IList<Finreview>> GetCashXFinanceByCompany(string companycode);
    }
}
