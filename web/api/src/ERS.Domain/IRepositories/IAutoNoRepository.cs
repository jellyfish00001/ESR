using ERS.Entities;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
namespace ERS.IRepositories
{
    public interface IAutoNoRepository : IRepository<EAutono, Guid>
    {
        Task<string> CreateCash1No();
        Task<string> CreateCash2No();
        Task<string> CreateCash3No();
        Task<string> CreateCash3ANo();
        Task<string> CreateCash4No();
        Task<string> CreateCash5No();
        Task<string> CreateCash6No();
        Task<string> CreateCashXNo();
        Task<string> CreateAccountNo();
        Task<string> CreatePaymentNo();
        Task<string> CreateCash1RNo();
        Task<string> CreateCashUberRNo();
    }
}
