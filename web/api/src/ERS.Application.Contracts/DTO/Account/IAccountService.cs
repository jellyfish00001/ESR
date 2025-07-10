using System.Collections.Generic;
using System.Threading.Tasks;
namespace ERS.DTO.Account
{
    public interface IAccountService
    {
        Task<Result<List<string>>> GetQueryBanks(string company);
        Task<Result<List<QueryPostingDto>>> GetPageAccountReceipts(Request<QueryFormParamDto> parameters);
        Task<Result<string>> GenerateAccountList(QueryFormDto queryFormDto, string userId);
        Task<Result<List<GenerateFormDto>>> GetPageAccountList(Request<GenerateFormParamDto> parameters);
        Task<Result<string>> BatchDeleteAccountlist(List<string> carryno);
        Task<byte[]> DownloadAccountList(string carryno);
        Task<Result<ExcelDto<CarryDetailDto>>> GetCarryDetailExcelData(string carryno);
        Task<Result<string>> SaveAccountInfo(string rno, string userId);
        Task<Result<List<string>>> GetBanksBySite(string company);
    }
}