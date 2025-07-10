using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ERS.DTO.Application;
using Microsoft.AspNetCore.Authorization;
using ERS.DTO;
using System.Collections.Generic;
using ERS.DTO.Approval;
using Microsoft.AspNetCore.Http;

namespace ERS.Controllers
{
#if DEBUG
    [AllowAnonymous]
#endif
    [Route("api/[controller]")]
    public class CashController : BaseController
    {
        private ICash2Service _Cash2Service;
        private ICashService _CashService;
        public CashController(ICash2Service Cash2Service, ICashService CashService)
        {
            _Cash2Service = Cash2Service;
            _CashService = CashService;
        }
        [HttpGet("intranet")]
        public bool Intranet()
        {
            string ipaddress = "";
            ipaddress = this.HttpContext.Request.Headers.ContainsKey("X-Real-IP") ? this.HttpContext.Request.Headers["X-Real-IP"].ToString() : "";
            ipaddress = this.HttpContext.Request.Headers.ContainsKey("X-Forwarded-For") ? this.HttpContext.Request.Headers["X-Forwarded-For"].ToString() : "";
            ipaddress = this.HttpContext.Connection.RemoteIpAddress.ToString();
            if (ipaddress == "::1" || ipaddress == "127.0.0.1" || ipaddress.StartsWith("10."))
                return true;
            else
                return false;
        }
        [HttpGet("checkip"), AllowAnonymous]
        public string CheckIP()
        {
            string res1 = this.HttpContext.Request.Headers.ContainsKey("X-Real-IP") ? this.HttpContext.Request.Headers["X-Real-IP"].ToString() : "";
            string res2 = this.HttpContext.Request.Headers.ContainsKey("X-Forwarded-For") ? this.HttpContext.Request.Headers["X-Forwarded-For"].ToString() : "";
            string ipaddress = this.HttpContext.Connection.RemoteIpAddress.ToString();
            return ipaddress + "," + res1 + "," + res2;
        }
        [HttpPost("query")]
        public async Task<Result<ApplicationDto>> GetQueryApplication([FromBody] Query data) => await _Cash2Service.GetQueryApplications(data.rno);
        /// <summary>
        /// 签核
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("approval")]
        public async Task<Result<string>> Approval([FromBody] SignDto data)
        {
            return await _CashService.Approval(data, this.userId, this.token);
        }
        /// <summary>
        /// 驳回
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("reject")]
        public async Task<Result<string>> Reject([FromBody] SignDto data) => await _CashService.Reject(data, this.userId, this.token);
        /// <summary>
        /// 获取签核流程及签核记录
        /// </summary>
        /// <param name="rno"></param>
        /// <returns></returns>
        [HttpPost("sign/logs")]
        public async Task<Result<SignProcessAndLogDto>> GetSignLogs(string rno) => await _CashService.GetSignLogs(rno, this.token);
        /// <summary>
        /// 根据单号获取签核历史数据（e_form_signlog）
        /// </summary>
        /// <param name="rno"></param>
        /// <returns></returns>
        [HttpPost("sign/records")]
        public async Task<Result<List<SignlogDto>>> GetHistoricalSignRecordsByRno(string rno) => await _CashService.GetHistoricalSignRecords(rno);
        /// <summary>
        /// 根据单号获取签核数据（e_form_alist）
        /// </summary>
        /// <param name="rno"></param>
        /// <returns></returns>
        [HttpPost("sign/process")]
        public async Task<Result<List<AlistDto>>> GetSignedDataByRno(string rno) => await _CashService.GetSignedData(rno);
        [HttpPost("self/signs")]
        public async Task<Result<List<ToBeSignedDto>>> GetUnSignedList([FromBody] Request<string> request)
        {
            //return await _CashService.QuerySignedFormByUserId(request, this.userId, this.token);
            return await _CashService.QuerySignedFormByUserId(request, this.userId);

        }
        [HttpPost("sign/cancel")]
        public async Task<Result<string>> Cancel(string rno) => await _CashService.Cancel(rno, this.userId, this.token);
        [HttpPost("sign/transform")]
        public async Task<Result<string>> Transform([FromBody] SignDto data) => await _CashService.Transform(data, this.userId, this.token);
        [HttpPost("sign/isaccountant1")]
        public async Task<bool> IsAccountant1Sign(string rno) => await _CashService.IsAccountant1Sign(rno, this.userId);
        [HttpPost("SendMail")]
        public async Task<bool> SendMail(string msg,string subject) => await _CashService.SendMail(msg,subject);

        [HttpPost("UpdateCashDetailRemark")]
        public async Task<Result<string>> UpdateCashDetaillRemark([FromBody] CashDetailDto cashDetailDto) => await _Cash2Service.UpdateCashDetailRemark(cashDetailDto);

        [HttpPost("UpdateCashDetailData")]
        public async Task<Result<string>> UpdateCashDetailData(IFormCollection formCollection) => await _Cash2Service.UpdateCashDetailRequestAmount(formCollection);

        [HttpPost("UpdateInvoiceData")]
        public async Task<Result<string>> UpdateInvoiceData(IFormCollection formCollection) => await _Cash2Service.UpdateInvoiceData(formCollection);
    }
}