using ERS.DTO;
using ERS.DTO.BDExp;
using ERS.DTO.BDExpenseSenario;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
namespace ERS.IDomainServices
{
    public interface IBDExpenseSenarioDomainService : IDomainService
    {
        Task<BDSenarioDto> GetSenarioById(Guid Id);
        Task<List<BDSenarioOptionDto>> SearchSenarioOptionsByKeyword(BDSenarioOptionFilterDto bDSenarioOptionFilterDto);
        //Task<PagedResultDto<BDSenarioDTO>> GetSenarios(Request<BDSenarioFilterDto> request);

        Task<ExpAcctDto> GetExpAcct(string expcode, string company);
        Task<Result<IEnumerable<BDExpDto>>> GetGeneralCostExp(string company);
        Task<string> GetSignDType(string company, IList<string> codes, IList<string> acctcodes);
        Task<Result<List<BDExpFormDto>>> GetPageExpenseCategory(Request<BDExpParamDto> request);
        Task<Result<List<string>>> GetAccountantSubject(string company);
        Task<Result<List<string>>> GetClassNo();
        Task<Result<string>> AddExpenseCategory(AddBDExpDto request, string userId);
        Task<Result<string>> EditExpenseCategory(EditBDExpDto request, string userId);
        Task<Result<string>> DeleteExpenseCategory(Guid id);
        Task<Byte[]> GetExpenseCategoryExcel(BDExpParamDto request);
        Task<Result<ExcelDto<BDExpExcelDto>>> GetBDExpExcelData(Request<BDExpParamDto> request);
        Task<Result<IEnumerable<BDExpDto>>> GetReturnTaiwanExp(string company);
        Task<Result<List<UploadBDExpDto>>> BatchUploadBDExp(IFormFile excelFile, string userId);
        Task<Result<List<BDExpDto>>> GetXZExp(string company);
        Task<Result<List<BDExpDto>>> GetExpenseCodes();
    }
}
