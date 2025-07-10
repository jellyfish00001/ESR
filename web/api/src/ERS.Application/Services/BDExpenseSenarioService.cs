using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.BDExp;
using ERS.DTO.BDExpenseSenario;
using ERS.IDomainServices;
using Microsoft.AspNetCore.Http;
using Volo.Abp.Application.Services;

namespace ERS.Services
{
    public class BDExpenseSenarioService : ApplicationService, IBDExpenseSenarioService
    {

        private IBDExpenseSenarioDomainService _BDSenarioDomainService;

        public BDExpenseSenarioService(IBDExpenseSenarioDomainService BDSenarioDomainService)
        {
            _BDSenarioDomainService = BDSenarioDomainService;
        }

        public async Task<Result<List<BDExpFormDto>>> GetPageExpenseCategory(Request<BDExpParamDto> request)
        {
            return await _BDSenarioDomainService.GetPageExpenseCategory(request);
        }
        public async Task<Result<List<string>>> GetAccountantSubject(string company)
        {
            return await _BDSenarioDomainService.GetAccountantSubject(company);
        }
        public async Task<Result<List<string>>> GetClassNo()
        {
            return await _BDSenarioDomainService.GetClassNo();
        }
        public async Task<Result<string>> AddExpenseCategory(AddBDExpDto request, string userId)
        {
            return await _BDSenarioDomainService.AddExpenseCategory(request, userId);
        }
        public async Task<Result<string>> EditExpenseCategory(EditBDExpDto request, string userId)
        {
            return await _BDSenarioDomainService.EditExpenseCategory(request, userId);
        }
        public async Task<Result<string>> DeleteExpenseCategory(Guid id)
        {
            return await _BDSenarioDomainService.DeleteExpenseCategory(id);
        }
        public async Task<Byte[]> GetExpenseCategoryExcel(BDExpParamDto request)
        {
            return await _BDSenarioDomainService.GetExpenseCategoryExcel(request);
        }
        public async Task<Result<ExcelDto<BDExpExcelDto>>> GetBDExpExcelData(Request<BDExpParamDto> request)
        {
            return await _BDSenarioDomainService.GetBDExpExcelData(request);
        }
        public async Task<Result<List<UploadBDExpDto>>> BatchUploadBDExp(IFormFile excelFile, string userId)
        {
            return await _BDSenarioDomainService.BatchUploadBDExp(excelFile, userId);
        }
        public async Task<Result<List<BDExpDto>>> GetXZExp(string company)
        {
            return await _BDSenarioDomainService.GetXZExp(company);
        }
        public async Task<Result<List<BDExpDto>>> GetExpenseCodes()
        {
            return await _BDSenarioDomainService.GetExpenseCodes();
        }

        public async Task<BDSenarioDto> GetSenarioById(Guid id)
        {
            return await _BDSenarioDomainService.GetSenarioById(id);
        }

        public async Task<List<BDSenarioOptionDto>> SearchSenarioOptionsByKeyword(BDSenarioOptionFilterDto bDSenarioOptionFilterDto)
        {
            return await _BDSenarioDomainService.SearchSenarioOptionsByKeyword(bDSenarioOptionFilterDto);
        }

        //public async Task<PagedResultDto<BDSenarioDTO>> GetSenarios(Request<BDSenarioFilterDto> request) {
        //    return await _BDSenarioDomainService.GetSenarios(request);
        //}
    }
}