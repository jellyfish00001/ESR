using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.Payment;
using Microsoft.AspNetCore.Http;
namespace ERS.IDomainServices
{
    public interface IPaymentDomainService
    {
        Task<Result<string>> GeneratePaylist(IFormCollection formCollection, string userId, string token);
        Task<Result<List<PaymentListDto>>> QueryPagePayList(Request<PayParamsDto> request);
        Task<List<PaymentDetailDto>> QueryPaylistDetails(string sysno, string bank);
        Task<Result<string>> DeletePaylist(List<string> sysno, string token);
        Task<Result<List<PaymentListDto>>> QueryPageUnpaidList(Request<string> request, string userId);
        Task<Result<byte[]>> GenerateRemittanceExcel(RemittanceDto parameters);
        Task<Result<byte[]>> GeneratePettyListExcel(UpdatePaymentDto request);
        Task<Result<string>> UpdatePaymentStatus(List<UpdatePaymentDto> parameters);
        Task SendMailToUserForPayment(string emplid, string sysno);
    }
}
