using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
namespace ERS.DTO.Payment
{
    public interface IPaymentService
    {
        Task<Result<string>> GeneratePaylist(IFormCollection formCollection, string userId, string token);
        Task<Result<List<PaymentListDto>>> GetPagePayList(Request<PayParamsDto> request);
        Task<List<PaymentDetailDto>> GetPaylistDetails(string sysno, string bank);
        Task<Result<string>> DeletePaylist(List<string> sysno, string token);
        Task<Result<List<PaymentListDto>>> GetPageUnpaidList(Request<string> request, string userId);
        Task<Byte[]> GetRemittanceExcel(RemittanceDto parameters);
        Task<Byte[]> GetPettyListExcel(UpdatePaymentDto request);
        Task<Result<string>> UpdatePaymentStatus(List<UpdatePaymentDto> parameters);
        Task SendMailToUserForPayment(string emplid, string sysno);
    }
}