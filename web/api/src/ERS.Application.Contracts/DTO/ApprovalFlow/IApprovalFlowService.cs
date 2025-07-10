using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO.Application;

namespace ERS.DTO.ApprovalFlow
{
    public interface IApprovalFlowService
    {
         Task<Result<ApprovalFlowDto>> GetApprovalFlowById(Guid? id);
         Task<Result<List<ApprovalFlowDto>>> GetApprovalFlowByRno(string rno);
         Task<Result<string>> ApplyApprovalFlow(CashHeadDto cashHead, List<CashDetailDto> cashDetails);
         Task<Result<List<ApprovalFlowDto>>> GetCurrentApprovalFlow(string rno);
         Task<Result<List<ApprovalFlowDto>>> GetCurrentApprovalFlowList(string rno);
         Task<Result<string>> ForwardApprovalFlow(ProcessFlowDto request);
         Task<Result<string>> DeleteApprovalFlow(ProcessFlowDto request);
         Task<Result<string>> ApproveApprovalFlow(ProcessFlowDto request, string userId);
         Task<Result<string>> RejectApprovalFlow(ProcessFlowDto request);
         Task<Result<List<ApprovalFlowDto>>> GetHistoryApprovalFlow(string rno);
         
    }
}