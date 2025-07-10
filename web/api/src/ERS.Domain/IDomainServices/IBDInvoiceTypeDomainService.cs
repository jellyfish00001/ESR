using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.BDInvoiceType;
namespace ERS.IDomainServices
{
    public interface IBDInvoiceTypeDomainService
    {
        Task<Result<List<BDInvoiceTypeDto>>> GetPageInvTypes(Request<QueryBDInvTypeDto> request);
        Task<Result<string>> AddInvTypes(AddBDInvTypeDto request, string userId);
        Task<Result<string>> EditInvTypes(EditBDInvTypeDto request, string userId);
        Task<Result<string>> DeleteInvTypes(List<Guid> Ids);
        Task<Result<List<InvoiceTypeDto>>> GetInvTypesByUserCompany(string company);
        Task<Result<List<InvoiceTypeDto>>> GetAllInvTypes();
    }
}