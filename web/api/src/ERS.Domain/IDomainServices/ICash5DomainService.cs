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
    public interface ICash5DomainService : IDomainService
    {
        Task<Result<CashResult>> Submit(IFormCollection formCollection, string user, string token, string status = "P");
        Task<Result<CashResult>> Keep(IFormCollection formCollection, string user, string token, string status = "T");
        Task<Result<List<Cash5ExcelDto>>> CheckExcel(IFormFile excelFile, string company, decimal totalAmount, string userId);
    }
}