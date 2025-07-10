using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.BDExpenseDept;
using Microsoft.AspNetCore.Http;

namespace ERS.IDomainServices
{
    public interface IBDExpenseDeptDomainService
    {
        Task<Result<List<QueryBDExpenseDeptDto>>> GetPageBDExpenseDept(Request<QueryBDExpenseDeptDto> request);
        Task<Result<string>> AddBDExpenseDept(QueryBDExpenseDeptDto request, string userId);
        Task<Result<string>> EditBDExpenseDept(QueryBDExpenseDeptDto request, string userId);
        Task<Result<string>> DeleteBDExpenseDept(List<Guid?> Ids);
        Task<byte[]> GetBDExpenseDeptExcelTemp();
        Task<byte[]> GetBDExpenseDeptExcelData(Request<QueryBDExpenseDeptDto> request);
        Task<Result<string>> BatchUploadBDExpenseDept(IFormFile excelFile, string userId);
    }
}