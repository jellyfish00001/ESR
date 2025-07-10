using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.BDSignlevel;
using ERS.IDomainServices;
using Microsoft.AspNetCore.Http;
using Volo.Abp.Application.Services;
// Signlevel
namespace ERS.Services
{
    public class BDSignlevelService : ApplicationService, IBDSignlevelService
    {
        private IBDSignlevelDomainService _BDSignlevelDomainService;

        public BDSignlevelService(IBDSignlevelDomainService BDSignlevelDomainService)
        {
            _BDSignlevelDomainService = BDSignlevelDomainService;
        }

        public async Task<Result<List<QueryBDSignlevelDto>>> GetPageBDSignlevel(Request<BDSignlevelParamDto> request){
            return await _BDSignlevelDomainService.GetPageBDSignlevel(request);
        }
        public async Task<List<string>> GetBDSignlevelByCompanyCode(string company){
            return await _BDSignlevelDomainService.GetBDSignlevelByCompanyCode(company);
        }
        public async Task<Result<string>> AddBDSignlevel(BDSignlevelParamDto request, string userId){
            return await _BDSignlevelDomainService.AddBDSignlevel(request, userId);
        }
        public async Task<Result<string>> EditBDSignlevel(BDSignlevelParamDto request, string userId){
            return await _BDSignlevelDomainService.EditBDSignlevel(request, userId);
        }
        public async Task<Result<string>> DeleteBDSignlevel(BDSignlevelParamDto request){
            return await _BDSignlevelDomainService.DeleteBDSignlevel(request);
        }
        public async Task<Result<string>> DeleteBDSignlevelById(List<Guid?> Ids){
            return await _BDSignlevelDomainService.DeleteBDSignlevelById(Ids);
        }
        public async Task<byte[]> GetBDSignlevelExcel(Request<BDSignlevelParamDto> request){
            return await _BDSignlevelDomainService.GetBDSignlevelExcel(request);
        }
        public async Task<byte[]> GetBDSignlevelExcelTemp(){
            return await _BDSignlevelDomainService.GetBDSignlevelExcelTemp();
        }
        public async Task<Result<ExcelDto<QueryBDSignlevelDto>>> GetBDExcelData(Request<BDSignlevelParamDto> request){
            return await _BDSignlevelDomainService.GetBDExcelData(request);
        }
        public async Task<Result<string>> BatchUploadBDSignlevel(IFormFile excelFile, string userId){
            return await _BDSignlevelDomainService.BatchUploadBDSignlevel(excelFile, userId);
        }
    }
}