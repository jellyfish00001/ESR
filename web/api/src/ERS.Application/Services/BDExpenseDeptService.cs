using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.BDExpenseDept;
using ERS.IDomainServices;
using Microsoft.AspNetCore.Http;
using Volo.Abp.Application.Services;

namespace ERS.Services
{
    public class BDExpenseDeptService : ApplicationService, IBDExpenseDeptService
    {
        private IBDExpenseDeptDomainService _BDExpenseDeptDomainService;
        public BDExpenseDeptService(IBDExpenseDeptDomainService BDExpenseDeptDomainService)
        {
            _BDExpenseDeptDomainService = BDExpenseDeptDomainService;
        }

        public async Task<Result<List<QueryBDExpenseDeptDto>>> GetPageBDExpenseDept(Request<QueryBDExpenseDeptDto> request)
        {
            return await _BDExpenseDeptDomainService.GetPageBDExpenseDept(request);
        }

        public async Task<Result<string>> AddBDExpenseDept(QueryBDExpenseDeptDto request, string userId)
        {
            return await _BDExpenseDeptDomainService.AddBDExpenseDept(request, userId);
        }

        public async Task<Result<string>> EditBDExpenseDept(QueryBDExpenseDeptDto request, string userId)
        {
            return await _BDExpenseDeptDomainService.EditBDExpenseDept(request, userId);
        }

        public async Task<Result<string>> DeleteBDExpenseDept(List<Guid?> Ids)
        {
            return await _BDExpenseDeptDomainService.DeleteBDExpenseDept(Ids);
        }

        public async Task<byte[]> GetBDExpenseDeptExcelTemp()
        {
            return await _BDExpenseDeptDomainService.GetBDExpenseDeptExcelTemp();
        }

        public async Task<byte[]> GetBDExpenseDeptExcelData(Request<QueryBDExpenseDeptDto> request)
        {
            return await _BDExpenseDeptDomainService.GetBDExpenseDeptExcelData(request);
        }

        public async Task<Result<string>> BatchUploadBDExpenseDept(IFormFile excelFile, string userId)
        {
            return await _BDExpenseDeptDomainService.BatchUploadBDExpenseDept(excelFile, userId);
        }
    }
}