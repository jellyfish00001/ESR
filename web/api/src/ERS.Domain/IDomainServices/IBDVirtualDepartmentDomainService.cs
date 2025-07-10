using ERS.DTO;
using ERS.DTO.BDExpenseDept;
using ERS.DTO.BDVirtualDepartments;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERS.IDomainServices
{
    public interface IBDVirtualDepartmentDomainService
    {
        Task<Result<List<QueryBDVirtualDepartmentsDto>>> GetPageBDVirtualDepartments(Request<QueryBDVirtualDepartmentsDto> request);
        Task<Result<string>> AddBDVirtualDepartment(QueryBDVirtualDepartmentsDto request, string userId);
        Task<Result<string>> EditBDVirtualDepartment(QueryBDVirtualDepartmentsDto request, string userId);
        Task<Result<string>> DeleteBDVirtualDepartment(List<Guid?> Ids);
        Task<byte[]> GetBDVirtualDepartmentExcelTemp();
        Task<byte[]> GetBDVirtualDepartmentExcelData(Request<QueryBDExpenseDeptDto> request);
        Task<Result<string>> BatchUploadBDVirtualDepartment(IFormFile excelFile, string userId);
    }
}