using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.Account;
using Volo.Abp.Domain.Services;
namespace ERS.IDomainServices
{
    public interface IAccountDomainService : IDomainService
    {
        Task<Result<List<string>>> QueryBanks(string company);
        Task<Result<List<QueryPostingDto>>> QueryPageAccountReceipts(Request<QueryFormParamDto> parameters);
        Task<Result<string>> GenerateAccountList(QueryFormDto queryFormDto, string userId);
        Task<Result<List<GenerateFormDto>>> QueryPageAccountList(Request<GenerateFormParamDto> parameters);
        Task<Result<string>> BatchDeleteAccountlist(List<string> carryno);
        Task<byte[]> DownloadAccountList(string carryno);
        Task<Result<string>> SaveAccountInfo(string rno, string userId);
        Task<Result<ExcelDto<CarryDetailDto>>> GetCarryDetailExcelData(string carryno);
        Task<Result<string>> SaveCashXAccInfo(string rno, string userId);
    }
}