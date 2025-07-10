using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Application.Contracts.DTO.Application;
using Microsoft.AspNetCore.Http;
namespace ERS.DTO.Application
{
    public interface ICash3AService
    {
        Task<Result<List<DeferredDto>>> DeferredQuery(string user);
        Task<Result<CashResult>> Submit(IFormCollection formCollection, string user, string token);
        Task<Result<CashResult>> Keep(IFormCollection formCollection, string user, string token);
    }
}
