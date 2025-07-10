using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.Payment;
using ERS.IDomainServices;
using Microsoft.AspNetCore.Http;
using Volo.Abp.Application.Services;
namespace ERS.Services.Payment
{
    public class PaymentService : ApplicationService, IPaymentService
    {
        private IPaymentDomainService _PaymentDomainService;
        public PaymentService(IPaymentDomainService PaymentDomainService)
        {
            _PaymentDomainService = PaymentDomainService;
        }
        public async Task<Result<string>> GeneratePaylist(IFormCollection formCollection, string userId, string token)
        {
            return await _PaymentDomainService.GeneratePaylist(formCollection, userId, token);
        }
        public async Task<Result<List<PaymentListDto>>> GetPagePayList(Request<PayParamsDto> request)
        {
            return await _PaymentDomainService.QueryPagePayList(request);
        }
        public async Task<List<PaymentDetailDto>> GetPaylistDetails(string sysno, string bank)
        {
            return await _PaymentDomainService.QueryPaylistDetails(sysno,bank);
        }
        public async Task<Result<string>> DeletePaylist(List<string> sysno, string token)
        {
            return await _PaymentDomainService.DeletePaylist(sysno, token);
        }
        public async Task<Result<List<PaymentListDto>>> GetPageUnpaidList(Request<string> request, string userId)
        {
            return await _PaymentDomainService.QueryPageUnpaidList(request,userId);
        }
        public async Task<Byte[]> GetRemittanceExcel(RemittanceDto parameters)
        {
            var result =  await _PaymentDomainService.GenerateRemittanceExcel(parameters);
            Byte[] data = null;
            if(result.status == 1 && result.data != null)
            {
                data = result.data;
            }
            return data;
        }
        public async Task<Byte[]> GetPettyListExcel(UpdatePaymentDto request)
        {
            var result = await _PaymentDomainService.GeneratePettyListExcel(request);
            Byte[] data = null;
            if(result.status == 1 && result.data != null)
            {
                data = result.data;
            }
            return data;
        }
        public async Task<Result<string>> UpdatePaymentStatus(List<UpdatePaymentDto> parameters)
        {
            return await _PaymentDomainService.UpdatePaymentStatus(parameters);
        }
        public async Task SendMailToUserForPayment(string emplid, string sysno)
        {
            await _PaymentDomainService.SendMailToUserForPayment(emplid, sysno);
        }
    }
}