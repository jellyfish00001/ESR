using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
namespace ERS.DTO.BDAccount
{
    public interface IBDAccountService
    {
        Task<Result<List<QueryBDAcctDto>>> GetPageBDAcct(Request<BDAccountParamDto> request);
        Task<Result<string>> AddBDAccount(BDAccountParamDto request, string userId);
        Task<Result<string>> EditBDAccount(BDAccountParamDto request, string userId);
        Task<Result<string>> DeleteBDAccount(BDAccountParamDto request);
        Task<byte[]> GetBDAccountExcel(Request<BDAccountParamDto> request);
        Task<Result<ExcelDto<QueryBDAcctDto>>> GetBDExcelData(Request<BDAccountParamDto> request);
        Task<Result<string>> BatchUploadBDAccount(IFormFile excelFile);
    }
}
