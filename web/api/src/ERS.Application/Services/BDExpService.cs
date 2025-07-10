using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO.BDExp;
using Volo.Abp.Application.Services;
using ERS.IDomainServices;
using ERS.DTO;
using Microsoft.AspNetCore.Http;
namespace ERS.Services
{
    public class BDExpService : ApplicationService, IBDExpService
    {
        private IBDExpDomainService _BDExpDomainService;
        public BDExpService(IBDExpDomainService BDExpDomainService)
        {
            _BDExpDomainService = BDExpDomainService;
        }
        public async Task<Result<List<BDExpFormDto>>> GetPageExpenseCategory(Request<BDExpParamDto> request)
        {
            return await _BDExpDomainService.GetPageExpenseCategory(request);
        }
        public async Task<Result<List<string>>> GetAccountantSubject(string company)
        {
            return await _BDExpDomainService.GetAccountantSubject(company);
        }
        public async Task<Result<List<string>>> GetClassNo()
        {
            return await _BDExpDomainService.GetClassNo();
        }
        public async Task<Result<string>> AddExpenseCategory(AddBDExpDto request, string userId)
        {
            return await _BDExpDomainService.AddExpenseCategory(request,userId);
        }
        public async Task<Result<string>> EditExpenseCategory(EditBDExpDto request, string userId)
        {
            return await _BDExpDomainService.EditExpenseCategory(request, userId);
        }
        public async Task<Result<string>> DeleteExpenseCategory(BDExpFormDto request)
        {
            return await _BDExpDomainService.DeleteExpenseCategory(request);
        }
        public async Task<Byte[]> GetExpenseCategoryExcel(BDExpParamDto request)
        {
            return await _BDExpDomainService.GetExpenseCategoryExcel(request);
        }
        public async Task<Result<ExcelDto<BDExpExcelDto>>> GetBDExpExcelData(Request<BDExpParamDto> request)
        {
            return await _BDExpDomainService.GetBDExpExcelData(request);
        }
        public async Task<Result<List<UploadBDExpDto>>> BatchUploadBDExp(IFormFile excelFile, string userId)
        {
            return await _BDExpDomainService.BatchUploadBDExp(excelFile, userId);
        }
        public async Task<Result<List<BDExpDto>>> GetXZExp(string company)
        {
            return await _BDExpDomainService.GetXZExp(company);
        }
    }
}