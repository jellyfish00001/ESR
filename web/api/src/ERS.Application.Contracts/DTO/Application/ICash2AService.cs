using ERS.DTO;
using ERS.DTO.Application;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ERS.Application.Contracts.DTO.Application
{
    public interface ICash2AService
    {
        Task<Result<CashResult>> Submit(IFormCollection formCollection, string user, string token);
        Task<Result<CashResult>> Keep(IFormCollection formCollection, string user, string token);
        Task<Result<List<MealCostDto>>> MealCost(string company);
    }
}