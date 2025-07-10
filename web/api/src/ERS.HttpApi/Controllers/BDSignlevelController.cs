using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Attribute;
using ERS.DTO;
using ERS.DTO.BDSignlevel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ERS.Controllers
{
#if DEBUG
    [AllowAnonymous]
#endif     
     [Route("api/[controller]")]
     
    public class BDSignlevelController : BaseController
    {
        private IBDSignlevelService _BDSignlevelService;
        public BDSignlevelController(IBDSignlevelService BDSignlevelService)
        {
            _BDSignlevelService = BDSignlevelService;
        }

        //核決權限分頁條件查詢
        [HttpPost("query")]
        [Permission("ers.BDSignlevel.View")]
        public async Task<Result<List<QueryBDSignlevelDto>>> GetPageBDSignlevel([FromBody]Request<BDSignlevelParamDto> request)
        {
            return await _BDSignlevelService.GetPageBDSignlevel(request);
        }

        /// 核決權限添加
        [HttpPost()]
        [Permission("ers.BDSignlevel.Add")]
        public async Task<Result<string>> AddBDSignlevel([FromBody]BDSignlevelParamDto request)
        {
            return await _BDSignlevelService.AddBDSignlevel(request,this.userId);
        }

        /// 核決權限編輯
        [HttpPut()]
        [Permission("ers.BDSignlevel.Edit")]
        public async Task<Result<string>> EditBDSignlevel([FromBody]BDSignlevelParamDto request)
        {
            return await _BDSignlevelService.EditBDSignlevel(request,this.userId);
        }

        //核決權限刪除
        [HttpDelete()]
        [Permission("ers.BDSignlevel.Delete")]
        public async Task<Result<string>> DeleteBDSignlevel([FromBody]BDSignlevelParamDto request)
        {
            return await _BDSignlevelService.DeleteBDSignlevel(request);
        }

        //核決權限刪除
        [HttpDelete("deleteById")]
        [Permission("ers.BDSignlevel.Delete")]
        public async Task<Result<string>> DeleteBDSignlevelById([FromBody]List<Guid?> Ids)
        {
            return await _BDSignlevelService.DeleteBDSignlevelById(Ids);
        }

        //下載Excel資料
        [HttpPost("download")]
        [Permission("ers.BDSignlevel.Download")]
        public async Task<FileResult> DownloadBDSignlevel([FromBody]Request<BDSignlevelParamDto> request)
        {
            byte[] data = await _BDSignlevelService.GetBDSignlevelExcel(request);
            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" ,"BD05.xlsx");
        }

        //下載上傳的版模
        [HttpPost("downloadTemp")]
        [Permission("ers.BDSignlevel.Download")]
        public async Task<FileResult> DownloadBDSignlevelTemp()
        {
            byte[] data = await _BDSignlevelService.GetBDSignlevelExcelTemp();
            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" ,"BD05.xlsx");
        }

        //批量上傳核決權限
        [HttpPost("upload")]
        public async Task<Result<string>> UploadBDExp(IFormFile excelFile)
        {
            return await _BDSignlevelService.BatchUploadBDSignlevel(excelFile, this.userId);
        }
    }
}