using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.Account;
using ERS.DTO.Application;
using ERS.DTO.Application.CashX;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ERS.DTO.BDExp;
using Microsoft.AspNetCore.Http;
using ERS.Application.Contracts.DTO.Application;
namespace ERS.Controllers
{
    /// <summary>
    /// 薪资请款
    /// </summary>
#if DEBUG
    [AllowAnonymous]
#endif
    [Route("api/[controller]")]
    public class CashXController : BaseController
    {
        private IAccountService _AccountService;
        private ICashXService _CashXService;
        private IBDExpService _BDExpService;
        private ICashService _CashService;
        public CashXController(
            IAccountService AccountService,
            ICashXService CashXService,
            IBDExpService BDExpService,
            ICashService CashService)
        {
            _AccountService = AccountService;
            _CashXService = CashXService;
            _BDExpService = BDExpService;
            _CashService = CashService;
        }
        /// <summary>
        /// 根据Site获取银行别
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        [HttpGet("Banks")]
        public async Task<Result<List<string>>> GetBanksBySite(string company)
        {
            return await _AccountService.GetBanksBySite(company);
        }
        /// <summary>
        /// 根据单据信息生成摘要text
        /// </summary>
        /// <param name="summaryInfo"></param>
        /// <returns></returns>
        [HttpPost("Summary")]
        public Task<Result<string>> GenerateSummary([FromBody]SummaryInfoDto summaryInfo)
        {
            return _CashXService.GenerateSummary(summaryInfo);
        }
        /// <summary>
        /// 获取薪资请款情景
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        [HttpGet("ExpInfo")]
        public async Task<Result<List<BDExpDto>>> GetXZExp(string company)
        {
            return await _BDExpService.GetXZExp(company);
        }
        /// <summary>
        /// 判断当前登录人是否为申请人簽核 1表示当前登录人是申请人
        /// </summary>
        /// <param name="rno"></param>
        /// <returns></returns>
        [HttpGet("IsApplicant")]
        public async Task<Result<int>> IsApplicant(string rno) => await _CashXService.IsApplicant(rno, this.userId, this.token);
        /// <summary>
        /// 大量上传获取请款信息
        /// </summary>
        /// <param name="excelFile"></param>
        /// <param name="company"></param>
        /// <returns></returns>
        [HttpPost("ApplyInfo")]
        public async Task<Result<List<ApplyInfoDto>>> GetApplyInfoFromExcel(IFormFile excelFile, string company)
        {
            return await _CashXService.GetApplyInfoFromExcel(excelFile, company);
        }
        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="formCollection">cashdetail新增companycode rdate（需款日期） summary（薪资所属期）</param>
        /// <returns></returns>
        [HttpPost("Submit")]
        public async Task<Result<CashResult>> Submit(IFormCollection formCollection) => await _CashXService.Submit(formCollection, this.userId, this.token);
        /// <summary>
        /// 暂存
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost("Keep")]
        public async Task<Result<CashResult>> Keep(IFormCollection formCollection) => await _CashXService.Keep(formCollection, this.userId, this.token);
        /// <summary>
        /// 薪資請款簽核
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost("Approval")]
        public async Task<Result<string>> Approval(IFormCollection formCollection) => await _CashService.CashXApproval(formCollection, this.userId, this.token);
    }
}