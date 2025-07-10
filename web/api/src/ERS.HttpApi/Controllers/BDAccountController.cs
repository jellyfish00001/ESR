using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Attribute;
using ERS.DTO;
using ERS.DTO.BDAccount;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace ERS.Controllers
{
#if DEBUG
    [AllowAnonymous]
#endif
    [Route("api/[controller]")]
    public class BDAccountController : BaseController
    {
        private IBDAccountService _BDAccountService;
        public BDAccountController(IBDAccountService BDAccountService)
        {
            _BDAccountService = BDAccountService;
        }
        /// <summary>
        /// 会计科目分页条件查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("query")]
        [Permission("ers.BDAccount.View")]
        public async Task<Result<List<QueryBDAcctDto>>> GetPageBDAcct([FromBody]Request<BDAccountParamDto> request)
        {
            return await _BDAccountService.GetPageBDAcct(request);
        }
        /// <summary>
        /// 会计科目添加
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost()]
        [Permission("ers.BDAccount.Add")]
        public async Task<Result<string>> AddBDAccount([FromBody]BDAccountParamDto request)
        {
            return await _BDAccountService.AddBDAccount(request,this.userId);
        }
        /// <summary>
        /// 會計科目編輯
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut()]
        [Permission("ers.BDAccount.Edit")]
        public async Task<Result<string>> EditBDAccount([FromBody]BDAccountParamDto request)
        {
            return await _BDAccountService.EditBDAccount(request,this.userId);
        }
        /// <summary>
        /// 會計科目刪除
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpDelete()]
        [Permission("ers.BDAccount.Delete")]
        public async Task<Result<string>> DeleteBDAccount([FromBody]BDAccountParamDto request)
        {
            return await _BDAccountService.DeleteBDAccount(request);
        }
        [HttpPost("download")]
        [Permission("ers.BDAccount.Download")]
        public async Task<FileResult> DownloadBDAccount([FromBody]Request<BDAccountParamDto> request)
        {
            byte[] data = await _BDAccountService.GetBDAccountExcel(request);
            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" ,"BD05.xlsx");
        }
        [HttpPost("exportdata")]
        [Permission("ers.BDAccount.Download")]
        public async Task<Result<ExcelDto<QueryBDAcctDto>>> GetBDExcelData([FromBody]Request<BDAccountParamDto> request)
        {
            return await _BDAccountService.GetBDExcelData(request);
        }
        /// <summary>
        /// 大量上传会计科目
        /// </summary>
        /// <param name="excelFile"></param>
        /// <returns></returns>
        [HttpPost("upload")]
        public async Task<Result<string>> BatchUploadBDAccount(IFormFile excelFile) => await _BDAccountService.BatchUploadBDAccount(excelFile);
    }
}
