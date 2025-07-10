using System;
using System.Threading.Tasks;
using ERS.Entities.Bank;
using Volo.Abp.Domain.Repositories;
namespace ERS.IRepositories
{
    public interface IComBankRepository : IRepository<ComBank, Guid>
    {
        Task<string> GetVenderCode(string company, string bank);
    }
}