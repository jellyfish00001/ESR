using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.Account;
using ERS.IDomainServices;
using Volo.Abp.Application.Services;
namespace ERS.Services.Account
{
    public class AccountService : ApplicationService, IAccountService
    {
        private IAccountDomainService _AccountDomainService;
        public AccountService(IAccountDomainService AccountDomainService)
        {
            _AccountDomainService = AccountDomainService;
        }
        public async Task<Result<List<string>>> GetQueryBanks(string company)
        {
            return await _AccountDomainService.QueryBanks(company);
        }
        public async Task<Result<List<string>>> GetBanksBySite(string company)
        {
            var result = await _AccountDomainService.QueryBanks(company);
            result.data.Add("Cash");//固定添加一个CASH
            return result;
        }
        public async Task<Result<List<QueryPostingDto>>> GetPageAccountReceipts(Request<QueryFormParamDto> parameters)
        {
            return await _AccountDomainService.QueryPageAccountReceipts(parameters);
        }
        public async Task<Result<string>> GenerateAccountList(QueryFormDto queryFormDto, string userId)
        {
            return await _AccountDomainService.GenerateAccountList(queryFormDto,userId);
        }
        public async Task<Result<List<GenerateFormDto>>> GetPageAccountList(Request<GenerateFormParamDto> parameters)
        {
            return await _AccountDomainService.QueryPageAccountList(parameters);
        }
        public async Task<Result<string>> BatchDeleteAccountlist(List<string> carryno)
        {
            return await _AccountDomainService.BatchDeleteAccountlist(carryno);
        }
        public async Task<byte[]> DownloadAccountList(string carryno)
        {
            return await _AccountDomainService.DownloadAccountList(carryno);
        }
        public async Task<Result<ExcelDto<CarryDetailDto>>> GetCarryDetailExcelData(string carryno)
        {
            return await _AccountDomainService.GetCarryDetailExcelData(carryno);
        }
        public async Task<Result<string>> SaveAccountInfo(string rno, string userId)
        {
            return await _AccountDomainService.SaveAccountInfo(rno, userId);
        }
    }
}