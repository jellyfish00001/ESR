using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Application.Contracts.DTO.Application;
using ERS.DTO;
using ERS.DTO.Application;
using ERS.DTO.BDCar;
using Microsoft.AspNetCore.Http;
using Volo.Abp.Domain.Services;
namespace ERS.IDomainServices
{
    public interface ICash4DomainService : IDomainService
    {
        Task<Result<List<OvertimeMealDto>>> GetOvertimeMealCostListFromExcel(IFormFile excelFile, string company);
        Task<Result<List<ReimbListDto>>> GetReimbursementListFromExcel(IFormFile excelFile, string company);
        Task<Result<List<SelfDriveCostDto>>> GetSelfDriveCostListFromExcel(IFormFile excelFile, string company);
        Task<Result<CashResult>> Submit(IFormCollection formCollection, string user, string token, string status = "P");
        Task<Result<List<PayeeDto>>> GetPayeeInfo(string keyword);
    }
}
