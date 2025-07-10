using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
namespace ERS.DTO.BDSignlevel
{
    public interface IBDSignlevelService
    {
        Task<Result<List<QueryBDSignlevelDto>>> GetPageBDSignlevel(Request<BDSignlevelParamDto> request);
        Task<List<string>> GetBDSignlevelByCompanyCode(string company);
        Task<Result<string>> AddBDSignlevel(BDSignlevelParamDto request, string userId);
        Task<Result<string>> EditBDSignlevel(BDSignlevelParamDto request, string userId);
        Task<Result<string>> DeleteBDSignlevel(BDSignlevelParamDto request);
        Task<Result<string>> DeleteBDSignlevelById(List<Guid?> Ids);
        Task<byte[]> GetBDSignlevelExcel(Request<BDSignlevelParamDto> request);
        Task<byte[]> GetBDSignlevelExcelTemp();
        Task<Result<ExcelDto<QueryBDSignlevelDto>>> GetBDExcelData(Request<BDSignlevelParamDto> request);
        Task<Result<string>> BatchUploadBDSignlevel(IFormFile excelFile, string userId);
    }
}