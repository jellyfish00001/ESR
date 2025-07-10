using ERS.Attribute;
using ERS.DTO;
using ERS.DTO.BDExp;
using ERS.DTO.BDExpenseSenario;
using ERS.DTO.BDSignlevel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ERS.Controllers
{
#if DEBUG
    [AllowAnonymous]
#endif
    [Route("api/[controller]")]
    public class BDExpenseSenarioController : BaseController
    {
        private IBDExpenseSenarioService _BDSenarioService;
        private IBDSignlevelService _BDSignlevelService;
        public BDExpenseSenarioController(IBDExpenseSenarioService iBDExpService, IBDSignlevelService iBDSignlevelService)
        {
            _BDSenarioService = iBDExpService;
            _BDSignlevelService = iBDSignlevelService;
        }

        public BDExpenseSenarioController(IBDExpenseSenarioService iBDSenarioService)
        {
            _BDSenarioService = iBDSenarioService;
        }

        /// <summary>
        /// 按公司別查詢會計科目
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        [HttpGet("acctcodes")]
        [Permission("ers.BDExp.View")]
        public async Task<Result<List<string>>> GetAccountantSubject(string company)
        {
            return await _BDSenarioService.GetAccountantSubject(company);
        }

        /// <summary>
        /// 获取核決權限代碼
        /// </summary>
        /// <returns></returns>
        [HttpGet("auditlevelcode")]
        public async Task<Result<List<string>>> GetAuditLevelCode(string company)
        {
            // return await _BDExpService.GetClassNo();
            Result<List<string>> result = new Result<List<string>>();
            result.data = await _BDSignlevelService.GetBDSignlevelByCompanyCode(company);
            result.message = L["Success"];

            return result;
        }
        /// <summary>
        /// 費用類別維護分页条件查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("query")]
        //[Permission("ers.BDExp.View")]
        public async Task<Result<List<BDExpFormDto>>> GetPageExpenseCategory([FromBody] Request<BDExpParamDto> request)
        {
            return await _BDSenarioService.GetPageExpenseCategory(request);
        }
        /// <summary>
        /// 費用類別維護(添加)
        /// 无需传递Id
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("senario")]
        //[Permission("ers.BDExp.Add")]
        public async Task<Result<string>> AddExpenseCategory([FromBody] AddBDExpDto request)
        {
            return await _BDSenarioService.AddExpenseCategory(request, this.userId);
        }
        /// <summary>
        /// 費用類別維護(編輯)
        /// Id必传
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("senario")]
        //[Permission("ers.BDExp.Edit")]
        public async Task<Result<string>> EditExpenseCategory([FromBody] EditBDExpDto request)
        {
            return await _BDSenarioService.EditExpenseCategory(request, this.userId);
        }
        /// <summary>
        /// 費用類別維護(删除)
        /// Id必传
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("delete")]
        //[Permission("ers.BDExp.Delete")]
        public async Task<Result<string>> DeleteExpenseCategory(Guid id)
        {
            return await _BDSenarioService.DeleteExpenseCategory(id);
        }
        /// <summary>
        /// 费用类别维护（Excel导出）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("download")]
        //[Permission("ers.BDExp.Download")]
        public async Task<FileResult> DownloadExpCategory([FromBody] BDExpParamDto request)
        {
            Byte[] data = await _BDSenarioService.GetExpenseCategoryExcel(request);
            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BD06.xlsx");
        }
        /// <summary>
        /// 費用類別維護（導出Excel數據）--废弃
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("exportdata")]
        //[Permission("ers.BDExp.Download")]
        public async Task<Result<ExcelDto<BDExpExcelDto>>> GetBDExpExcelData([FromBody] Request<BDExpParamDto> request)
        {
            return await _BDSenarioService.GetBDExpExcelData(request);
        }
        /// <summary>
        /// 批量上传报销情景
        /// </summary>
        /// <param name="excelFile"></param>
        /// <param name="company"></param>
        /// <returns></returns>
        [HttpPost("upload")]
        public async Task<Result<List<UploadBDExpDto>>> BatchUploadBDExp(IFormFile excelFile)
        {
            return await _BDSenarioService.BatchUploadBDExp(excelFile, this.userId);
        }

        /// <summary>
        /// 獲取費用類別清單
        /// </summary>
        /// <returns></returns>
        [HttpGet("expcodes")]
        //[Permission("ers.BDExp.View")]
        public async Task<Result<List<BDExpDto>>> GetExpenseCodes()
        {
            return await _BDSenarioService.GetExpenseCodes();
        }

        /// <summary>
        /// 按ID查詢報銷情景
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        //[Permission("ers.BDSenario.View")]
        public async Task<Result<BDSenarioDto>> GetSenarioByID([FromRoute] Guid id)
        {
            Result<BDSenarioDto> result = new()
            {
                data = await _BDSenarioService.GetSenarioById(id)
            };
            return result;
        }

        /// <summary>
        /// 按關鍵字查詢報銷情景選項清單
        /// </summary>
        /// <param name="bDSenarioOptionFilterDto"></param>
        /// <returns></returns>
        [HttpGet("options")]
        //[Permission("ers.BDSenario.View")]
        public async Task<Result<List<BDSenarioOptionDto>>> SearchSenarioOptionsByKeyword([FromQuery] BDSenarioOptionFilterDto bDSenarioOptionFilterDto)
        {
            Result<List<BDSenarioOptionDto>> result = new()
            {
                data = await _BDSenarioService.SearchSenarioOptionsByKeyword(bDSenarioOptionFilterDto)
            };
            return result;
        }

        ///// <summary>
        ///// 按條件查詢報銷情景清單
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns></returns>
        //[HttpGet("query")]
        //[Permission("ers.BDSenario.View")]
        //public async Task<Result<List<BDSenarioDTO>>> GetSenarios([FromQuery] Request<BDSenarioFilterDto> request)
        //{
        //    PagedResultDto<BDSenarioDTO> pagedResultDto = await _BDSenarioService.GetSenarios(request);
        //    Result<List<BDSenarioDTO>> result = new() { 
        //        data = pagedResultDto.Data,
        //        total = pagedResultDto.TotalCount
        //    };
        //    return result;
        //}
    }
}