using ERS.Attribute;
using ERS.DTO;
using ERS.DTO.BDExpenseDept;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERS.Controllers
{
#if DEBUG
    [AllowAnonymous]
#endif     
    [Route("api/[controller]")]
    
    public class BDExpenseDeptController : BaseController
    {
        private IBDExpenseDeptService _BDExpenseDeptService;

        public BDExpenseDeptController(IBDExpenseDeptService iBDExpenseDeptService)
        {
            _BDExpenseDeptService = iBDExpenseDeptService;
        }

        // 歸屬部門白名單分頁條件查詢
        [HttpPost("query")]
        [Permission("ers.BDExpenseDept.View")]
        public async Task<Result<List<QueryBDExpenseDeptDto>>> GetPageBDExpenseDept([FromBody]Request<QueryBDExpenseDeptDto> request)
        {
            return await _BDExpenseDeptService.GetPageBDExpenseDept(request);
        }

        /// 歸屬部門添加
        [HttpPost()]
        [Permission("ers.BDExpenseDept.Add")]
        public async Task<Result<string>> AddBDExpenseDept([FromBody]QueryBDExpenseDeptDto request)
        {
            return await _BDExpenseDeptService.AddBDExpenseDept(request,this.userId);
        }

        /// 歸屬部門編輯
        [HttpPut()]
        [Permission("ers.BDExpenseDept.Edit")]
        public async Task<Result<string>> EditBDExpenseDept([FromBody]QueryBDExpenseDeptDto request)
        {
            return await _BDExpenseDeptService.EditBDExpenseDept(request,this.userId);
        }

        // 歸屬部門刪除
        [HttpDelete()]
        [Permission("ers.BDExpenseDept.Delete")]
        public async Task<Result<string>> DeleteBDExpenseDept([FromBody]List<Guid?> Ids)
        {
            return await _BDExpenseDeptService.DeleteBDExpenseDept(Ids);
        }

        // 下載Excel資料
        [HttpPost("download")]
        [Permission("ers.BDExpenseDept.Download")]
        public async Task<FileResult> DownloadBDExpenseDept([FromBody]Request<QueryBDExpenseDeptDto> request)
        {
            byte[] data = await _BDExpenseDeptService.GetBDExpenseDeptExcelData(request);
            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" ,"BDExpenseDept.xlsx");
        }

        // 下載上傳的版模
        [HttpPost("downloadTemp")]
        [Permission("ers.BDExpenseDept.Download")]
        public async Task<FileResult> DownloadBDExpenseDeptTemp()
        {
            byte[] data = await _BDExpenseDeptService.GetBDExpenseDeptExcelTemp();
            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" ,"BDExpenseDept_Temp.xlsx");
        }

        // 批量上傳歸屬部門
        [HttpPost("upload")]
        public async Task<Result<string>> UploadBDExpenseDept(IFormFile excelFile)
        {
            return await _BDExpenseDeptService.BatchUploadBDExpenseDept(excelFile, this.userId);
        }

    }
}