using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Application.Contracts.DTO.Application;
using ERS.DTO.BDExp;
using Microsoft.AspNetCore.Http;
namespace ERS.DTO.Application
{
    public interface ICash6Service
    {
        Task<Result<CashResult>> Submit(IFormCollection formCollection, string user, string token);
        Task<Result<CashResult>> Keep(IFormCollection formCollection, string user, string token);
        Task<Result<IEnumerable<BDExpDto>>> GetReturnTaiwanExp(string company);
    }
}