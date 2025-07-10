using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ERS.DTO.Auditor
{
    public interface IAuditorService
    {
        Task<Result<List<AuditorDto>>> GetPageAuditors(Request<AuditorParamsDto> request);
        Task<Result<string>> AddAuditor(AuditorParamsDto request, string userId);
        Task<Result<string>> EditAuditor(AuditorParamsDto request, string userId);
        Task<Result<string>> DeleteAuditors(List<Guid?> Ids);
        Task<Result<List<BDFormDto>>> GetEFormCodeName();
    }
}