using ERS.Attribute;
using ERS.DTO;
using ERS.DTO.BDExpenseDept;
using ERS.DTO.BDVirtualDepartments;
using ERS.Services;
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
    
    public class BDVirtualDepartmentController : BaseController
    {
        private IBDVirtualDetpartmentService _BDVirtulaDepartmentsService;

        public BDVirtualDepartmentController(IBDVirtualDetpartmentService iBDVirtulaDepartmentsService)
        {
            _BDVirtulaDepartmentsService = iBDVirtulaDepartmentsService;
        }

        // 虛擬部門白名單分頁條件查詢
        [HttpPost("query")]
        [Permission("ers.BDVirtualDepartments.View")]
        public async Task<Result<List<QueryBDVirtualDepartmentsDto>>> GetBDVirtualDepartments([FromBody] Request<QueryBDVirtualDepartmentsDto> request)
        {
            return await _BDVirtulaDepartmentsService.GetBDVirtualDepartments(request);
        }

        /// 虛擬部門添加
        [HttpPost()]
        [Permission("ers.BDVirtualDepartments.Add")]
        public async Task<Result<string>> AddBDVirtualDepartment([FromBody] QueryBDVirtualDepartmentsDto request)
        {
            return await _BDVirtulaDepartmentsService.AddBDVirtualDepartment(request, this.userId);
        }

        /// 虛擬部門編輯
        [HttpPut()]
        [Permission("ers.BDVirtualDepartments.Edit")]
        public async Task<Result<string>> EditBDVirtualDepartment([FromBody] QueryBDVirtualDepartmentsDto request)
        {
            return await _BDVirtulaDepartmentsService.EditBDVirtualDepartment(request, this.userId);
        }

        // 虛擬部門刪除
        [HttpDelete()]
        [Permission("ers.BDVirtualDepartments.Delete")]
        public async Task<Result<string>> DeleteBDVirtualDepartment([FromBody] List<Guid?> Ids)
        {
            return await _BDVirtulaDepartmentsService.DeleteBDVirtualDepartment(Ids);
        }

        // 下載Excel資料
        [HttpPost("download")]
        [Permission("ers.BDVirtualDepartments.Download")]
        public async Task<FileResult> DownloadBDVirtualDepartment([FromBody] Request<QueryBDExpenseDeptDto> request)
        {
            byte[] data = await _BDVirtulaDepartmentsService.GetBDVirtualDepartmentExcelData(request);
            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BDExpenseDept.xlsx");
        }

        // 下載上傳的版模
        [HttpPost("downloadTemp")]
        [Permission("ers.BDVirtualDepartments.Download")]
        public async Task<FileResult> DownloadBDExpenseDeptTemp()
        {
            byte[] data = await _BDVirtulaDepartmentsService.GetBDVirtualDepartmentExcelTemp();
            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BDExpenseDept_Temp.xlsx");
        }

        // 批量上傳虛擬部門
        [HttpPost("upload")]
        public async Task<Result<string>> UploadBDVirtualDepartment(IFormFile excelFile)
        {
            return await _BDVirtulaDepartmentsService.BatchUploadBDVirtualDepartment(excelFile, this.userId);
        }
    }
}