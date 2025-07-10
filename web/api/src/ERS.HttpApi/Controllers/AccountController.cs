using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ERS.DTO.Account;
using ERS.DTO;
using ERS.Attribute;
using ERS.IDomainServices;
namespace ERS.Controllers
{
#if DEBUG
    [AllowAnonymous]
#endif
    [Route("api/[controller]")]
    public class AccountController : BaseController
    {
        private IAccountService _AccountService;
        private IAccountDomainService _accountDomainService;
        public AccountController(IAccountService AccountService, IAccountDomainService accountDomainService)
        {
            _AccountService = AccountService;
            _accountDomainService = accountDomainService;
        }
        /// <summary>
        /// 根据company获取全部银行api
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        [HttpGet("banks")]
        public async Task<Result<List<string>>> QueryBanksByCompany(string company)
        {
            return (await _AccountService.GetQueryBanks(company));
        }
        /// <summary>
        /// 入账清单单据信息查询
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [HttpPost("accountreceipts")]
        [Permission("ers.Account.View")]
        public async Task<Result<List<QueryPostingDto>>> QueryAccountReceipts([FromBody]Request<QueryFormParamDto> parameters)
        {
            return (await _AccountService.GetPageAccountReceipts(parameters));
        }
        /// <summary>
        /// 入账清单生成
        /// </summary>
        /// <param name="queryFormDtos"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost("accountlist/generate")]
        [Permission("ers.Account.Add")]
        public async Task<Result<string>> GenerateAccountList([FromBody]QueryFormDto queryFormDtos)
        {
            return (await _AccountService.GenerateAccountList(queryFormDtos,this.userId));
        }
        /// <summary>
        /// 入账清单维护（查询）
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [HttpPost("accountlist/query")]
        [Permission("ers.Account.View")]
        public async Task<Result<List<GenerateFormDto>>> QueryPageAccountList([FromBody]Request<GenerateFormParamDto> parameters)
        {
            return (await _AccountService.GetPageAccountList(parameters));
        }
        /// <summary>
        /// 批量删除入账清单
        /// </summary>
        /// <param name="carryno"></param>
        /// <returns></returns>
        [HttpDelete("accountlist")]
        [Permission("ers.Account.Delete")]
        public async Task<Result<string>> BatchDeleteAccountlist([FromBody] List<string> carryno)
        {
            return (await _AccountService.BatchDeleteAccountlist(carryno));
        }
        [HttpPost("accountlist/download")]
        [Permission("ers.Account.Download")]
        public async Task<FileResult> DownloadAccountList(string carryno)
        {
            byte[] data = await _AccountService.DownloadAccountList(carryno);
            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", carryno +".xlsx");
        }
        /// <summary>
        /// 导出生成Excel所需数据
        /// </summary>
        /// <param name="carryno"></param>
        /// <returns></returns>
        [HttpPost("accountlist/exportdata")]
        [Permission("ers.Account.Download")]
        public async Task<Result<ExcelDto<CarryDetailDto>>> GetCarryDetailExcelData(string carryno)
        {
            return await _AccountService.GetCarryDetailExcelData(carryno);
        }
        /// <summary>
        /// 补入账清单 cash_detail_pst
        /// </summary>
        /// <param name="rno"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost("SaveAccountInfo")]
        public async Task<Result<string>> SaveAccountInfo(string rno, string userId)
        {
           return await _accountDomainService.SaveAccountInfo(rno, userId);
        }
    }
}