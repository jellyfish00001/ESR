using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Attribute;
using ERS.DTO;
using ERS.DTO.BDExp;
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
    public class BDExpController : BaseController
    {
        private IBDExpService _BDExpService;
        private IBDSignlevelService _BDSignlevelService;
        public BDExpController(IBDExpService BDExpService, IBDSignlevelService BDSignlevelService)
        {
            _BDExpService = BDExpService;
            _BDSignlevelService = BDSignlevelService;
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
            return await _BDExpService.GetAccountantSubject(company);
        }
        /// <summary>
        /// 获取ClasNo
        /// </summary>
        /// <returns></returns>
        [HttpGet("classno")]
        public async Task<Result<List<string>>> GetClassNo(string company)
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
        [HttpPost("expcategory/query")]
        [Permission("ers.BDExp.View")]
        public async Task<Result<List<BDExpFormDto>>> GetPageExpenseCategory([FromBody]Request<BDExpParamDto> request)
        {
            return await _BDExpService.GetPageExpenseCategory(request);
        }
        /// <summary>
        /// 費用類別維護(添加)
        /// 无需传递Id
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("expcategory")]
        [Permission("ers.BDExp.Add")]
        public async Task<Result<string>> AddExpenseCategory([FromBody]AddBDExpDto request)
        {
            return await _BDExpService.AddExpenseCategory(request,this.userId);
        }
        /// <summary>
        /// 費用類別維護(編輯)
        /// Id必传
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("expcategory")]
        [Permission("ers.BDExp.Edit")]
        public async Task<Result<string>> EditExpenseCategory([FromBody]EditBDExpDto request)
        {
            return await _BDExpService.EditExpenseCategory(request, this.userId);
        }
        /// <summary>
        /// 費用類別維護(删除)
        /// Id必传
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpDelete("expcategory")]
        [Permission("ers.BDExp.Delete")]
        public async Task<Result<string>> DeleteExpenseCategory([FromBody]BDExpFormDto request)
        {
            return await _BDExpService.DeleteExpenseCategory(request);
        }
        /// <summary>
        /// 费用类别维护（Excel导出）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("expcategory/download")]
        [Permission("ers.BDExp.Download")]
        public async Task<FileResult> DownloadExpCategory([FromBody]BDExpParamDto request)
        {
            Byte[] data = await _BDExpService.GetExpenseCategoryExcel(request);
            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" ,"BD06.xlsx");
        }
        /// <summary>
        /// 費用類別維護（導出Excel數據）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("exportdata")]
        [Permission("ers.BDExp.Download")]
        public async Task<Result<ExcelDto<BDExpExcelDto>>> GetBDExpExcelData([FromBody]Request<BDExpParamDto> request)
        {
            return await _BDExpService.GetBDExpExcelData(request);
        }
        /// <summary>
        /// 批量上传报销情景
        /// </summary>
        /// <param name="excelFile"></param>
        /// <param name="company"></param>
        /// <returns></returns>
        [HttpPost("expcategory/upload")]
        public async Task<Result<List<UploadBDExpDto>>> BatchUploadBDExp(IFormFile excelFile)
        {
            return await _BDExpService.BatchUploadBDExp(excelFile, this.userId);
        }
    }
}