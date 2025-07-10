using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.BDInvoiceRail;
using Microsoft.AspNetCore.Http;
namespace ERS.IDomainServices
{
    public interface IBDInvoiceRailDomainService
    {
        Task<Result<List<BDInvoiceRailDto>>> GetPageBDInvoiceRails(Request<QueryBDInvoiceRailDto> request);
        Task<Result<string>> DeleteBDInvoiceRails(List<Guid> Ids);
        Task<Result<List<AddBDInvoiceRailDto>>> BatchUploadBDInvoiceRail(IFormFile excelFile, string userId);
    }
}