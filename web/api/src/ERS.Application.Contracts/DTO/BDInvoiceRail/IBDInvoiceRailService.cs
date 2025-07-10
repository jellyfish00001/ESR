using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ERS.DTO.BDInvoiceRail
{
    public interface IBDInvoiceRailService
    {
        Task<Result<List<BDInvoiceRailDto>>> GetPageBDInvoiceRails(Request<QueryBDInvoiceRailDto> request);
        Task<Result<string>> DeleteBDInvoiceRails(List<Guid> Ids);
        Task<Result<List<AddBDInvoiceRailDto>>> BatchUploadBDInvoiceRail(IFormFile excelFile, string userId);

    }
}