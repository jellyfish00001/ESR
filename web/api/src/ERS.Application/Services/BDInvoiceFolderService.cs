using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.Application;
using ERS.DTO.BDInvoiceFolder;
using ERS.IDomainServices;
using Volo.Abp.Application.Services;
namespace ERS.Services
{
    public class BDInvoiceFolderService : ApplicationService, IBDInvoiceFolderService
    {
        private IBDInvoiceFolderDomainService _BDInvoiceFolderDomainService;
        public BDInvoiceFolderService(
            IBDInvoiceFolderDomainService BDInvoiceFolderDomainService
        )
        {
            _BDInvoiceFolderDomainService = BDInvoiceFolderDomainService;
        }
        public async Task<Result<List<BDInvoiceFolderDto>>> GetPageInvInfo(Request<QueryBDInvoiceFolderDto> request, string userId)
        {
            return await _BDInvoiceFolderDomainService.GetPageInvInfo(request, userId);
        }
        public async Task<Result<List<string>>> GetInvPayTypes()
        {
            return await _BDInvoiceFolderDomainService.GetInvPayTypes();
        }
        public async Task<Result<string>> DeleteInvInfo(Guid id,string userId)
        {
            return await _BDInvoiceFolderDomainService.DeleteInvInfo(id,userId);
        }
        public async Task<Result<string>> EditInvInfo(InvoiceDto request, string userId)
        {
            return await _BDInvoiceFolderDomainService.EditInvInfo(request, userId);
        }
        public async Task<Result<List<UnpaidInvInfoDto>>> GetUnpaidInvInfo(string userId, string company)
        {
            return await _BDInvoiceFolderDomainService.GetUnpaidInvInfo(userId, company);
        }
    }
}