using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO.Application;
namespace ERS.DTO.BDInvoiceFolder
{
    public interface IBDInvoiceFolderService
    {
        Task<Result<List<BDInvoiceFolderDto>>> GetPageInvInfo(Request<QueryBDInvoiceFolderDto> request, string userId);
        Task<Result<List<string>>> GetInvPayTypes();
        Task<Result<string>> DeleteInvInfo(Guid id,string userId);
        Task<Result<string>> EditInvInfo(InvoiceDto request, string userId);
        Task<Result<List<UnpaidInvInfoDto>>> GetUnpaidInvInfo(string userId, string company);
    }
}