using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Application.Contracts.DTO.Invoice;
using ERS.Controllers;
using ExpenseApplication.Model.Models;
using Microsoft.AspNetCore.Mvc;
using ERS.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using ERS.DTO.Application;
using ERS.DTO.Invoice;
using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace ERS.HttpApi.Controllers
{
#if DEBUG
    [AllowAnonymous]
#endif
    public class InvoiceController : BaseController
    {
        private IInvoiceService _invoiceService;

        public InvoiceController(IInvoiceService invoiceService, IDIService iDIService, ILogger<InvoiceController> logger)
        {
            _invoiceService = invoiceService;
        }

        //查询发票信息
        [HttpGet("querPAInvoice")]
        public async Task<Result<PaInvoice>> querPAInvoice(string invcode, string invno)
        {
            return await _invoiceService.querPAInvoice(invcode, invno, this.token);
        }
        //查询发票异常原因
        [HttpPost("queryExpcode")]
        public async Task<List<InvException>> queryExpcode([FromBody] List<InvException> invoice)
        {
            return await _invoiceService.queryExpcode(invoice, userId);
        }
        //读取发票信息
        [HttpPost("readInvoice")]
        public async Task<InvoiceResult> readInvoice([FromForm] IFormCollection formCollection)
        {
            return await _invoiceService.readInvoice(formCollection, userId, this.token);
        }
        /// <summary>
        /// 读取档案获取发票信息(APP会使用到)
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns>
        ///
        /// </returns>
        [HttpPost("invoice/read")]
        public async Task<ERS.DTO.Result<List<InvoiceDto>>> ReadInvoiceByFile([FromForm] IFormCollection formCollection)
        {
            return await _invoiceService.ReadInvoiceInfoByFile(formCollection, userId, this.token);
        }
        /// <summary>
        /// 根据发票号码查询全电发票信息
        /// </summary>
        /// <param name="invno"></param>
        /// <returns></returns>
        [HttpPost("electricInvoice/read")]
        public async Task<ERS.DTO.Result<List<InvoiceDto>>> GetAllElectricInvoiceInfo(List<string> invno)
        {
            return await _invoiceService.GetInvoiceInfoByInvno(invno, this.token);
        }
        /// <summary>
        /// 發票上傳--wait to remove
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost("invoice/upload")]
        public async Task<DTO.Result<List<DTO.Result<string>>>> UploadInvoices(IFormCollection formCollection)
        {
            return await _invoiceService.UploadInvoices(formCollection, this.userId);
        }
        /// <summary>
        /// url检查
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("invoice/url/check")]
        public async Task<ERS.DTO.Result<bool>> UrlCheck([FromBody] ReadInvFileDto request)
        {
            return await _invoiceService.UrlCheck(request);
        }
        /// <summary>
        /// 根据url获取发票文件
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("invoice/url/read")]
        public async Task<FileResult> GetInvFileFromUrl([FromBody] ReadInvFileDto request)
        {
            byte[] data = (await _invoiceService.GetInvFileFromUrl(request)).data;
            return File(data, "application/pdf", "invoice.pdf");
        }
        /// <summary>
        /// 發票分享
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("invoice/share")]
        public async Task<ERS.DTO.Result<string>> ShareInvoice([FromBody] ShareInvoiceDto request)
        {
            return await _invoiceService.ShareInvoice(request);
        }
        [HttpPost("invoice/url/response")]
        public async Task<HttpResponseMessage> GetResponse([FromBody] ERS.DTO.Request<ReadInvFileDto> request)
        {
            return await _invoiceService.GetInvFileFromUrlResponse(request);
        }
        /// <summary>
        /// 下載發票清單
        /// </summary>
        /// <param name="rno"></param>
        /// <returns></returns>
        [HttpPost("invoice/list/download")]
        public async Task<FileResult> DownloadInvListExcel(string rno)
        {
            byte[] data = (await _invoiceService.GenerateInvListExcel(rno));
            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "InvoiceList.xlsx");
        }

        /// <summary>
        /// 根据发票号码/代码获取发票信息
        /// </summary>
        /// <param name="invno"></param>
        /// <param name="invcode"></param>
        /// <param name="invoiceArea"></param>
        /// <returns></returns>
        [HttpPost("invoice/manual/query")]
        public async Task<ERS.DTO.Result<List<InvoiceDto>>> QueryInvoiceByNo(string invno, string invcode,string invoiceArea)
        {
            return await _invoiceService.QueryInvoiceByNo(invno, invcode, invoiceArea, this.token);
        }

        /// <summary>
        /// 辨析档案获取发票信息(ERS2.0重构发票管理)
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns>
        ///
        /// </returns>
        [HttpPost("invoice/recognition")]
        public async Task<ERS.DTO.Result<List<InvoiceDto>>> RecognizeInvoice([FromForm] IFormCollection formCollection)
        {
            return await _invoiceService.RecognizeInvoice(formCollection, userId, this.token);
        }

        /// <summary>
        /// 添加發票
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost("invoice/add")]
        public async Task<DTO.Result<string>> AddInvoices(IFormCollection formCollection)
        {
            return await _invoiceService.AddInvoices(formCollection, this.userId);
        }

        /// <summary>
        /// 获取OCR辨析结果记录
        /// </summary>
        /// <param name="ocrid"></param>
        /// <returns></returns>
        [HttpGet("invoice/ocr-result")]
        public async Task<DTO.Result<InvoiceDto>> getOcrResult(string ocrid)
        {
            return await _invoiceService.getOcrResult(ocrid);
        }

        [HttpPost("invoice/updateInvoice")]
        public async Task<DTO.Result<string>> UpdateInvoice([FromBody] InvoiceDto invoiceDto) => await _invoiceService.UpdateInvoice(invoiceDto);
    }
}