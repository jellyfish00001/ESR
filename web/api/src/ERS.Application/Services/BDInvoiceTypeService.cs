using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.BDInvoiceType;
using ERS.IDomainServices;
using Volo.Abp.Application.Services;
namespace ERS.Services
{
    public class BDInvoiceTypeService : ApplicationService, IBDInvoiceTypeService
    {
        private IBDInvoiceTypeDomainService _BDInvoiceTypeDomainService;
        public BDInvoiceTypeService(
            IBDInvoiceTypeDomainService BDInvoiceTypeDomainService
        )
        {
            _BDInvoiceTypeDomainService = BDInvoiceTypeDomainService;
        }
        public async Task<Result<List<BDInvoiceTypeDto>>> GetPageInvTypes(Request<QueryBDInvTypeDto> request)
        {
            return await _BDInvoiceTypeDomainService.GetPageInvTypes(request);
        }
        public async Task<Result<string>> AddInvTypes(AddBDInvTypeDto request, string userId)
        {
            return await _BDInvoiceTypeDomainService.AddInvTypes(request, userId);
        }
        public async Task<Result<string>> EditInvTypes(EditBDInvTypeDto request, string userId)
        {
            return await _BDInvoiceTypeDomainService.EditInvTypes(request, userId);
        }
        public async Task<Result<string>> DeleteInvTypes(List<Guid> Ids)
        {
            return await _BDInvoiceTypeDomainService.DeleteInvTypes(Ids);
        }
        public async Task<Result<List<InvoiceTypeDto>>> GetInvTypesByUserCompany(string company)
        {
            return await _BDInvoiceTypeDomainService.GetInvTypesByUserCompany(company);
        }
        public async Task<Result<List<InvoiceTypeDto>>> GetAllInvTypes()
        {
            return await _BDInvoiceTypeDomainService.GetAllInvTypes();
        }
    }
}