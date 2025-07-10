using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.BDAccount;
using Microsoft.AspNetCore.Http;
namespace ERS.IDomainServices
{
    public interface IBDAccountDomainService
    {
        Task<Result<List<QueryBDAcctDto>>> QueryPageBDAccount(Request<BDAccountParamDto> request);
        Task<Result<string>> AddBDAccount(BDAccountParamDto request, string userId);
        Task<Result<string>> EditBDAccount(BDAccountParamDto request, string userId);
        Task<Result<string>> DeleteBDAccount(BDAccountParamDto request);
        Task<byte[]> GetBDAccountExcel(Request<BDAccountParamDto> request);
        Task<Result<ExcelDto<QueryBDAcctDto>>> GetBDExcelData(Request<BDAccountParamDto> request);
        Task<Result<string>> BatchUploadBDAccount(IFormFile excelFile);
    }
}
