using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.BDAccount;
using ERS.IDomainServices;
using Microsoft.AspNetCore.Http;
using Volo.Abp.Application.Services;
namespace ERS.Services
{
    public class BDAccountService : ApplicationService, IBDAccountService
    {
        private IBDAccountDomainService _BDAccountDomainService;
        public BDAccountService(IBDAccountDomainService BDAccountDomainService)
        {
            _BDAccountDomainService = BDAccountDomainService;
        }
        public async Task<Result<List<QueryBDAcctDto>>> GetPageBDAcct(Request<BDAccountParamDto> request)
        {
            return await _BDAccountDomainService.QueryPageBDAccount(request);
        }
        public async Task<Result<string>> AddBDAccount(BDAccountParamDto request, string userId)
        {
            return await _BDAccountDomainService.AddBDAccount(request,userId);
        }
        public async Task<Result<string>> EditBDAccount(BDAccountParamDto request, string userId)
        {
            return await _BDAccountDomainService.EditBDAccount(request,userId);
        }
        public async Task<Result<string>> DeleteBDAccount(BDAccountParamDto request)
        {
            return await _BDAccountDomainService.DeleteBDAccount(request);
        }
        public async Task<byte[]> GetBDAccountExcel(Request<BDAccountParamDto> request)
        {
            return await _BDAccountDomainService.GetBDAccountExcel(request);
        }
        public async Task<Result<ExcelDto<QueryBDAcctDto>>> GetBDExcelData(Request<BDAccountParamDto> request)
        {
            return await _BDAccountDomainService.GetBDExcelData(request);
        }
        public async Task<Result<string>> BatchUploadBDAccount(IFormFile excelFile)
        {
            return await _BDAccountDomainService.BatchUploadBDAccount(excelFile);
        }
    }
}
