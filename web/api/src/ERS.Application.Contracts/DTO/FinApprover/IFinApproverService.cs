using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ERS.DTO.FinApprover
{
    public interface IFinApproverService
    {
        Task<Result<List<FinApproverDto>>> QueryPageFinApprover(Request<FinApproverParamsDto> request);
        Task<Result<string>> AddFinApprover(AddFinApproverDto request, string userId);
        Task<Result<string>> EditFinApprover(FinApproverDto request, string userId);
        Task<Result<string>> DeleteFinApprover(List<Guid?> ids);
    }
}