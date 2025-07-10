using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ERS.Application.Contracts.DTO.Application;
using ERS.DTO;
using ERS.DTO.Application;
using ERS.DTO.Invoice;
using ERS.Entities;
using ExpenseApplication.Model.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Domain.Services;
namespace ERS.Domain.IDomainServices
{
    public interface IInvoiceDomainService : IDomainService
    {
        Task<Invoice> queryInvoice(string invcode, string invno);
        Task<Result<CashResult>> checkInvoicePaid(CashHead head, string token);
        Task<Result<PaInvoice>> querPAInvoice(string invcode, string invno, string Token = "");
        //生成报销单更改发票池请款状态
        Task<Result<string>> UpdatePaInvoiceStat(string invcode, string invno, string Token);
        Task<Result<List<InvoiceDto>>> GetInvoiceInfoByInvno(List<string> invno, string Token = "");
        Task<Result<List<InvoiceDto>>> ReadInvoiceInfoByFile(IFormCollection formCollection, string user, string token);
        Task<Result<List<Result<string>>>> UploadInvoices(IFormCollection formCollection, string userId);
        Task<Result<byte[]>> GetInvFileFromUrl(ReadInvFileDto request);
        Task<Result<bool>> UrlCheck(ReadInvFileDto request);
        Task<Result<string>> ShareInvoice(ShareInvoiceDto request);
        Task<HttpResponseMessage> GetInvFileFromUrlResponse(Request<ReadInvFileDto> request);
        Task<Result<byte[]>> GenerateInvListExcel(string rno);
        Task<Result<string>> UpdateInvoiceToRequested(List<UpdatePayStatDto> invList, string token);
        Task<Result<string>> UpdateInvoiceToUnrequested(List<UpdatePayStatDto> invList, string token);
        Task<Result<string>> UpdateInvoiceToUnrequested(string rno, string token);
        Task<Result<List<InvoiceDto>>> QueryInvoiceByNo(string invno, string invcode, string invoiceArea, string token);
        Task<Result<string>> UpdateInvStatToRequested(ERSApplyDto eRSApplyDto, string token);
        Task<Result<string>> UpdateInvPayInfo(List<ERSPayDto> ersPayDtos, bool isCancel, string Token);
        Task<Result<string>> AddInvoices(IFormCollection formCollection, string userId);
        Task<ERS.DTO.Result<List<InvoiceDto>>> RecognizeInvoice([FromForm] IFormCollection formCollection, string userId, string token);
        Task<Result<InvoiceDto>> getOcrResult(string ocrid);
        Task<Result<string>> UpdateInvoice(InvoiceDto invoiceDto);
    }
}