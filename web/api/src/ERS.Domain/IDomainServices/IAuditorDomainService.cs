using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.Auditor;
namespace ERS.IDomainServices
{
    /// <summary>
    /// 簽核主管Auditor維護（BD04）
    /// </summary>
    public interface IAuditorDomainService
    {
        Task<Result<List<AuditorDto>>> GetPageAuditors(Request<AuditorParamsDto> request);
        Task<Result<string>> AddAuditor(AuditorParamsDto request, string userId);
        Task<Result<string>> EditAuditor(AuditorParamsDto request, string userId);
        Task<Result<string>> DeleteAuditors(List<Guid?> Ids);
        Task<Result<List<BDFormDto>>> GetEFormCodeName();
    }
}