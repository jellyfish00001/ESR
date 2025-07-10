using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.Auditor;
using ERS.IDomainServices;
using Volo.Abp.Application.Services;
namespace ERS.Services.Auditor
{
    public class AuditorService : ApplicationService, IAuditorService
    {
        private IAuditorDomainService _AuditorDomainService;
        public AuditorService(IAuditorDomainService AuditorDomainService)
        {
            _AuditorDomainService = AuditorDomainService;
        }
        public async Task<Result<List<AuditorDto>>> GetPageAuditors(Request<AuditorParamsDto> request)
        {
            return await _AuditorDomainService.GetPageAuditors(request);
        }
        public async Task<Result<string>> AddAuditor(AuditorParamsDto request, string userId)
        {
            return await _AuditorDomainService.AddAuditor(request, userId);
        }
        public async Task<Result<string>> EditAuditor(AuditorParamsDto request, string userId)
        {
            return await _AuditorDomainService.EditAuditor(request, userId);
        }
        public async Task<Result<string>> DeleteAuditors(List<Guid?> Ids)
        {
            return await _AuditorDomainService.DeleteAuditors(Ids);
        }
        public async Task<Result<List<BDFormDto>>> GetEFormCodeName()
        {
            return await _AuditorDomainService.GetEFormCodeName();
        }
    }
}