using ERS.DomainServices;
using ERS.DTO;
using ERS.DTO.BDExpenseDept;
using ERS.DTO.BDVirtualDepartments;
using ERS.IDomainServices;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ERS.Services
{
    public class BDVirtualDetpartmentService : ApplicationService, IBDVirtualDetpartmentService
    {
        private IBDVirtualDepartmentDomainService _BDVirtualDepartmentDomainService;
        public BDVirtualDetpartmentService(IBDVirtualDepartmentDomainService BDVirtualDepartmentDomainService)
        {
            _BDVirtualDepartmentDomainService = BDVirtualDepartmentDomainService;
        }

        public async Task<Result<List<QueryBDVirtualDepartmentsDto>>> GetBDVirtualDepartments(Request<QueryBDVirtualDepartmentsDto> request)
        {
            return await _BDVirtualDepartmentDomainService.GetPageBDVirtualDepartments(request);
        }

        public async Task<Result<string>> AddBDVirtualDepartment(QueryBDVirtualDepartmentsDto request, string userId)
        {
            return await _BDVirtualDepartmentDomainService.AddBDVirtualDepartment(request, userId);
        }

        public async Task<Result<string>> EditBDVirtualDepartment(QueryBDVirtualDepartmentsDto request, string userId)
        {
            return await _BDVirtualDepartmentDomainService.EditBDVirtualDepartment(request, userId);
        }

        public async Task<Result<string>> DeleteBDVirtualDepartment(List<Guid?> Ids)
        {
            return await _BDVirtualDepartmentDomainService.DeleteBDVirtualDepartment(Ids);
        }

        public async Task<byte[]> GetBDVirtualDepartmentExcelTemp()
        {
            return await _BDVirtualDepartmentDomainService.GetBDVirtualDepartmentExcelTemp();
        }

        public async Task<byte[]> GetBDVirtualDepartmentExcelData(Request<QueryBDExpenseDeptDto> request)
        {
            return await _BDVirtualDepartmentDomainService.GetBDVirtualDepartmentExcelData(request);
        }

        public async Task<Result<string>> BatchUploadBDVirtualDepartment(IFormFile excelFile, string userId)
        {
            return await _BDVirtualDepartmentDomainService.BatchUploadBDVirtualDepartment(excelFile, userId);
        }
    }
}