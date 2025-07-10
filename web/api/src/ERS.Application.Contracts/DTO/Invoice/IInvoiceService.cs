using System.Collections.Generic;
using System.Threading.Tasks;
using ExpenseApplication.Model.Models;
using Microsoft.AspNetCore.Http;
using ERS.DTO.Application;
using ERS.DTO.Invoice;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
namespace ERS.Application.Contracts.DTO.Invoice
{
    public interface IInvoiceService
    {
        //上传发票附件读取发票信息
        Task<InvoiceResult> readInvoice(IFormCollection formCollection, string user, string token);
        Task<InvoiceDto> queryInvoice(string invcode, string invno);
        Task<string> invoiceStat(string invcode, string invno);
        Task<ERS.Common.Result<PaInvoice>> querPAInvoice(string invcode, string invno, string Token = "");
        Task<List<InvException>> queryExpcode(List<InvException> invoice, string user);
        //检查是否存在系统or发票池
        //Task<List<InvException>> invoiceExists(List<InvException> invoice);
        //计算税金损失
        decimal getInvoiceTax(decimal tax, decimal taxamount);
        Task<ERS.DTO.Result<string>> UpdatePaInvoiceStat(string invcode, string invno, string token = "");
        Task<ERS.DTO.Result<List<InvoiceDto>>> GetInvoiceInfoByInvno(List<string> invno, string Token = "");
        Task<ERS.DTO.Result<List<InvoiceDto>>> ReadInvoiceInfoByFile(IFormCollection formCollection, string user, string token);
        Task<ERS.DTO.Result<List<ERS.DTO.Result<string>>>> UploadInvoices(IFormCollection formCollection, string userId);
        Task<ERS.DTO.Result<byte[]>> GetInvFileFromUrl(ReadInvFileDto request);
        Task<ERS.DTO.Result<bool>> UrlCheck(ReadInvFileDto request);
        Task<ERS.DTO.Result<string>> ShareInvoice(ShareInvoiceDto request);
        Task<HttpResponseMessage> GetInvFileFromUrlResponse(ERS.DTO.Request<ReadInvFileDto> request);
        Task<byte[]> GenerateInvListExcel(string rno);
        Task<ERS.DTO.Result<List<InvoiceDto>>> QueryInvoiceByNo(string invno, string invcode, string invoiceArea, string token);
        Task<ERS.DTO.Result<List<InvoiceDto>>> RecognizeInvoice([FromForm] IFormCollection formCollection, string userId, string token);
        Task<ERS.DTO.Result<string>> AddInvoices(IFormCollection formCollection, string userId);

        Task<ERS.DTO.Result<InvoiceDto>> getOcrResult(string ocrid);

        Task<ERS.DTO.Result<string>> UpdateInvoice(InvoiceDto invoiceDto);
    }
}