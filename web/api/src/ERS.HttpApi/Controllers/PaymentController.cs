using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO.Payment;
using Microsoft.AspNetCore.Authorization;
using ERS.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ERS.Attribute;
namespace ERS.Controllers
{
#if DEBUG
    [AllowAnonymous]
#endif
    [Route("api/[controller]")]
    public class PaymentController : BaseController
    {
        private IPaymentService _PaymentService;
        public PaymentController(IPaymentService PaymentService)
        {
            _PaymentService = PaymentService;
        }
        /// <summary>
        /// 付款清單生成
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost("paylist/generate")]
        [Permission("ers.Payment.Add")]
        public async Task<Result<string>> GeneratePaymentList(IFormCollection formCollection)
        {
            return (await _PaymentService.GeneratePaylist(formCollection,this.userId, this.token));
        }
        /// <summary>
        /// 已付款清单查询
        /// </summary>
        /// <param name="request">筛选参数</param>
        /// <returns></returns>
        [HttpPost("paylist/paid/query")]
        [Permission("ers.Payment.View")]
        public async Task<Result<List<PaymentListDto>>> GetPagePayList([FromBody]Request<PayParamsDto> request)
        {
            return (await _PaymentService.GetPagePayList(request));
        }
        /// <summary>
        /// 未付款清单查询
        /// </summary>
        /// <param name="request">筛选参数</param>
        /// <returns></returns>
        [HttpPost("paylist/unpaid/query")]
        [Permission("ers.Payment.View")]
        public async Task<Result<List<PaymentListDto>>> GetPageUnpaidList([FromBody]Request<string> request)
        {
            return (await _PaymentService.GetPageUnpaidList(request, this.userId));
        }
        /// <summary>
        /// 按流水单号、银行别查询付款清单
        /// </summary>
        /// <param name="sysno"></param>
        /// <param name="bank"></param>
        /// <returns></returns>
        [HttpPost("paylist/details/query")]
        [Permission("ers.Payment.View")]
        public async Task<List<PaymentDetailDto>> GetPaylistBySysNo(string sysno, string bank)
        {
            return (await _PaymentService.GetPaylistDetails(sysno,bank));
        }
        /// <summary>
        /// 刪除付款清單
        /// </summary>
        /// <param name="sysno"></param>
        /// <returns></returns>
        [HttpDelete("paylist")]
        [Permission("ers.Payment.Delete")]
        public async Task<Result<string>> DeletePaylist([FromBody]List<string> sysno)
        {
            return (await _PaymentService.DeletePaylist(sysno,this.token));
        }
        /// <summary>
        /// 点击download按钮下载付款明细
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [HttpPost("remittance/download")]
        [Permission("ers.Payment.Download")]
        public async Task<FileResult> DownloadRemittanceExcel([FromBody] RemittanceDto parameters)
        {
             byte[] data = await _PaymentService.GetRemittanceExcel(parameters);
             string suffix = "";
             if(parameters.bank == "中國建設銀行" || parameters.bank == "中国建设银行")
             {
                suffix = "CCB";
             }else if(parameters.bank == "中國工商銀行" || parameters.bank == "中国工商银行")
             {
                suffix = "ICBC";
             }
             return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", parameters.sysno+ suffix +".xlsx");
        }
        /// <summary>
        /// 点击sysno按钮根据sysno与bank下载零用金付款清单api
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("pettylist/download")]
        [Permission("ers.Payment.Download")]
        public async Task<FileResult> DownloadPettyListExcel([FromBody]UpdatePaymentDto request)
        {
            byte[] data = await _PaymentService.GetPettyListExcel(request);
            string fileName = request.bank + "零用金付款清單";
            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName + ".xlsx");
        }
        /// <summary>
        /// 变更付款状态（点击OK）
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [HttpPost("paylist/update")]
        [Permission("ers.Payment.Edit")]
        public async Task<Result<string>> UpdatePaymentStatus([FromBody]List<UpdatePaymentDto> parameters)
        {
            return await _PaymentService.UpdatePaymentStatus(parameters);
        }
    }
}