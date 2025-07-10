using System;
using System.Threading.Tasks;
using ERS.Entities;
using Volo.Abp.Domain.Repositories;
namespace ERS.IRepositories
{
    public interface IBDFormRepository : IRepository<BDForm, Guid>
    {
        Task<string> GetNameByFormcode(string formcode);
    }
}
