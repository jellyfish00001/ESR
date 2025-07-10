using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ERS.DTO.BDExp;

namespace ERS.DTO.BDExpenseSenario
{
    public interface IBDExpService
    {
        Task<Result<List<BDExpFormDto>>> GetPageExpenseCategory(Request<BDExpParamDto> request);
        Task<Result<List<string>>> GetAccountantSubject(string company);
        Task<Result<List<string>>> GetClassNo();
        Task<Result<string>> AddExpenseCategory(AddBDExpDto request, string userId);
        Task<Result<string>> EditExpenseCategory(EditBDExpDto request, string userId);
        Task<Result<string>> DeleteExpenseCategory(BDExpFormDto request);
        Task<Byte[]> GetExpenseCategoryExcel(BDExpParamDto request);
        Task<Result<ExcelDto<BDExpExcelDto>>> GetBDExpExcelData(Request<BDExpParamDto> request);
        Task<Result<List<UploadBDExpDto>>> BatchUploadBDExp(IFormFile excelFile, string userId);
        Task<Result<List<BDExpDto>>> GetXZExp(string company);
    }
}