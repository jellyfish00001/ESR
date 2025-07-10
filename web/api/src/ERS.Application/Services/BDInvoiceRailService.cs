using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.BDInvoiceRail;
using ERS.IDomainServices;
using Microsoft.AspNetCore.Http;
using Volo.Abp.Application.Services;
namespace ERS.Services
{
    public class BDInvoiceRailService : ApplicationService, IBDInvoiceRailService
    {
        private IBDInvoiceRailDomainService _BDInvoiceRailDomainService;
        public BDInvoiceRailService(
            IBDInvoiceRailDomainService BDInvoiceRailDomainService
        )
        {
            _BDInvoiceRailDomainService = BDInvoiceRailDomainService;
        }

        public async Task<Result<List<AddBDInvoiceRailDto>>> BatchUploadBDInvoiceRail(IFormFile excelFile, string userId)
        {
            return await _BDInvoiceRailDomainService.BatchUploadBDInvoiceRail(excelFile, userId);
        }

        public async Task<Result<string>> DeleteBDInvoiceRails(List<Guid> Ids)
        {
            return await _BDInvoiceRailDomainService.DeleteBDInvoiceRails(Ids);
        }

        public async Task<Result<List<BDInvoiceRailDto>>> GetPageBDInvoiceRails(Request<QueryBDInvoiceRailDto> request)
        {
            return await _BDInvoiceRailDomainService.GetPageBDInvoiceRails(request);
        }
    }
}