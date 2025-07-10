using ERS.Application.Contracts.DTO.Communication;
using ERS.Application.Contracts.DTO.EmployeeInfo;
using ERS.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.IDomainServices;
using ERS.Application.Contracts.DTO.Employee;
namespace ERS.Controllers
{
#if DEBUG
    [AllowAnonymous]
#endif
    [Route("api/[controller]")]
    public class EmployeeInfoController : BaseController
    {
        private IEmployeeInfoService _EmployeeInfoService;
        private ITaxDataSyncService _taxDataSyncService;
        public EmployeeInfoController(IEmployeeInfoService EmployeeInfoService, ITaxDataSyncService TaxDataSyncService)
        {
            _EmployeeInfoService = EmployeeInfoService;
            _taxDataSyncService = TaxDataSyncService;
        }

        [HttpGet("")]
        public async Task<EmployeeDto> QueryEmployeeAsync()
        {
            return await _EmployeeInfoService.GetEmployeeAsync(this.userId);
        }
        [HttpGet("{user}")]
        public async Task<EmployeeDto> QueryEmployeeAsync(string user)
        {
            return await _EmployeeInfoService.GetEmployeeAsync(user);
        }
        // Get employee information by keyword
        [HttpGet("queryEmployeeInfosByKeyword")]
        public async Task<Result<List<EmployeeInfoDto>>> QueryEmployeeInfos(string keyword)
        {
            Result<List<EmployeeInfoDto>> result = new Result<List<EmployeeInfoDto>>();
            result.data = await _EmployeeInfoService.GetEmployeeInfos(keyword);
            result.message = L["Success"];
            
            return result;

            // return await _EmployeeInfoService.GetEmployeeInfos(keyword);
        }

        [HttpGet("getTaxData")]
        public async Task<Result<List<EmployeeInfoDto>>> GetTaxData()
        {
            Result<List<EmployeeInfoDto>> result = new Result<List<EmployeeInfoDto>>();
            await _taxDataSyncService.DownloadTaxDataAndSync();
            // result.data = "test";
            result.message = L["Success"];
            
            return result;

            // return await _EmployeeInfoService.GetEmployeeInfos(keyword);
        }
        
    }
}
