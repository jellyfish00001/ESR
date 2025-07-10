using ERS.Application.Contracts.DTO.Application;
using ERS.DTO;
using ERS.DTO.Application;
using ERS.DTO.BDCar;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
namespace ERS.IDomainServices
{
    public interface ICash1DomainService : IDomainService
    {
        Task<Result<CashResult>> Submit(IFormCollection formCollection, string user, string token, string status = "P");
        Task<Result<TravelDto>> GetMealExpense(TravelDto data);
        Task<Result<List<SelfDriveCostDto>>> GetSelfDriveCostListFromExcel(IFormFile excelFile, string company);
        Task<Result<List<OvertimeMealDto>>> GetOvertimeMealCostListFromExcel(IFormFile excelFile, string company);
        Task<Result<IList<string>>> GetCityByCompany(string company);
    }
}
