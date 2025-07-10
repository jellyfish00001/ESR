using ERS.Application.Contracts.DTO.Application;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERS.DTO.Application
{
    public interface ICash5Service
    {
        Task<Result<CashResult>> Submit(IFormCollection formCollection, string user, string token, string status = "P");
        Task<Result<CashResult>> Keep(IFormCollection formCollection, string user, string token, string status = "T");
        Task<Result<List<Cash5ExcelDto>>> CheckExcel(IFormFile excelFile, string company, decimal totalAmount, string userId);
    }
}