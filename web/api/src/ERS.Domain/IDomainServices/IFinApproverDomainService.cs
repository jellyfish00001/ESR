using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.FinApprover;
namespace ERS.IDomainServices
{
    public interface IFinApproverDomainService
    {
        Task<Result<List<FinApproverDto>>> QueryPageFinApprover(Request<FinApproverParamsDto> request);
        Task<Result<string>> AddFinApprover(AddFinApproverDto request, string userId);
        Task<Result<string>> EditFinApprover(FinApproverDto request, string userId);
        Task<Result<string>> DeleteFinApprover(List<Guid?> ids);
        Task<bool> IsExistFinApprover(AddFinApproverDto input);
    }
}