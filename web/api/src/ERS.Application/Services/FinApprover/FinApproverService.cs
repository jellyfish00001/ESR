using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.FinApprover;
using ERS.IDomainServices;
using Volo.Abp.Application.Services;
namespace ERS.Services.FinApprover
{
    public class FinApproverService : ApplicationService, IFinApproverService
    {
        private IFinApproverDomainService _FinApproverDomainService;
        public FinApproverService(IFinApproverDomainService FinApproverDomainService)
        {
            _FinApproverDomainService = FinApproverDomainService;
        }
        public async Task<Result<List<FinApproverDto>>> QueryPageFinApprover(Request<FinApproverParamsDto> request)
        {
            return await _FinApproverDomainService.QueryPageFinApprover(request);
        }
        public async Task<Result<string>> AddFinApprover(AddFinApproverDto request, string userId)
        {
            return await _FinApproverDomainService.AddFinApprover(request, userId);
        }
        public async Task<Result<string>> EditFinApprover(FinApproverDto request, string userId)
        {
            return await _FinApproverDomainService.EditFinApprover(request, userId);
        }
        public async Task<Result<string>> DeleteFinApprover(List<Guid?> ids)
        {
            return await _FinApproverDomainService.DeleteFinApprover(ids);
        }
    }
}