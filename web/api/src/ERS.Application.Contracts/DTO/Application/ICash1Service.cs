using ERS.Application.Contracts.DTO.Application;
using ERS.DTO.BDCar;
using ERS.DTO.BDExp;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ERS.DTO.Application
{
    public interface ICash1Service
    {
        Task<Result<IEnumerable<BDExpDto>>> GetGeneralCostExp(string company);
        Task<Result<CashResult>> Submit(IFormCollection formCollection, string user, string token);
        Task<Result<CashResult>> Keep(IFormCollection formCollection, string user, string token);
        Task<Result<List<SelfDriveCostDto>>> GetSelfDriveCostListFromExcel(IFormFile excelFile, string company);
        Task<Result<List<OvertimeMealDto>>> GetOvertimeMealCostListFromExcel(IFormFile excelFile, string company);
        Task<Result<TravelDto>> GetMealExpense(TravelDto data);
        Task<Result<IList<string>>> GetCityByCompany(string company);
    }
}
