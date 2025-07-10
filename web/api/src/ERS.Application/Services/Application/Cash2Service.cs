using ERS.DTO;
using ERS.DTO.Application;
using ERS.IDomainServices;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using ERS.Domain.IDomainServices;
using ERS.Application.Contracts.DTO.Application;
using System.Collections.Generic;

namespace ERS.Services
{
    public class Cash2Service : ApplicationService, ICash2Service
    {
        private ICash2DomainService _Cash2DomainService;
        private IApplicationDomainService _ApplicationDomainService;
        public Cash2Service(ICash2DomainService Cash2DomainService, IApplicationDomainService applicationDomainService)
        {
            _Cash2DomainService = Cash2DomainService;
            _ApplicationDomainService = applicationDomainService;
        }
        public async Task<Result<CashResult>> Submit(IFormCollection formCollection, string user, string token) => await _Cash2DomainService.Submit(formCollection, user, token, "P");
        public async Task<Result<CashResult>> Keep(IFormCollection formCollection, string user, string token) => await _Cash2DomainService.Submit(formCollection, user, token, "T");
        public async Task<Result<ApplicationDto>> GetQueryApplications(string rno) => await _ApplicationDomainService.QueryApplications(rno);

        public async Task<Result<string>> UpdateCashDetailRemark(CashDetailDto cashDetailDto) => await _ApplicationDomainService.UpdateCashDetailRemark(cashDetailDto);

        public async Task<Result<string>> UpdateCashDetailRequestAmount(IFormCollection formCollection) => await _ApplicationDomainService.UpdateCashDetailRequestAmount(formCollection);

        public async Task<Result<string>> UpdateInvoiceData(IFormCollection formCollection) => await _ApplicationDomainService.UpdateInvoice(formCollection);

    }
}
