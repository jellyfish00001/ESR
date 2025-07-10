using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Application.Contracts.DTO.Application;
using ERS.DTO.BDCar;
using Microsoft.AspNetCore.Http;
namespace ERS.DTO.Application
{
    public interface ICash4Service
    {
        Task<Result<List<ReimbListDto>>> GetReimbursementListFromExcel(IFormFile excelFile, string company);
        Task<Result<List<OvertimeMealDto>>> GetOvertimeMealCostListFromExcel(IFormFile excelFile, string company);
        Task<Result<List<SelfDriveCostDto>>> GetSelfDriveCostListFromExcel(IFormFile excelFile, string company);
        Task<Result<CashResult>> Submit(IFormCollection formCollection, string user, string token);
        Task<Result<CashResult>> Keep(IFormCollection formCollection, string user, string token);
        Task<Result<List<PayeeDto>>> GetPayeeInfo(string keyword);
    }
}
