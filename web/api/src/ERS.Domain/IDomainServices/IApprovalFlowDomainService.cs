using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.ApprovalFlow;
using ERS.Entities;

namespace ERS.IDomainServices
{
    public interface IApprovalFlowDomainService
    {
        Task<Result<ApprovalFlowDto>> GetApprovalFlowById(Guid? id);
        Task<Result<List<ApprovalFlowDto>>> GetApprovalFlowByRno(string rno);
        Task<Result<string>> ApplyUberApprovalFlow(CashHead cashHead,  List<CashDetail> cashDetails);        
        Task<Result<string>> ApplyApprovalFlow(CashHead cashHead, List<CashDetail> cashDetails);
        Task<Result<List<ApprovalFlowDto>>> GetCurrentApprovalFlow(string rno);
        Task<Result<List<ApprovalFlowDto>>> GetCurrentApprovalFlowList(string rno);
        Task<Result<string>> ForwardApprovalFlow(ProcessFlowDto request);
        Task<Result<string>> DeleteApprovalFlow(ProcessFlowDto request);
        Task<Result<string>> ApproveApprovalFlow(ProcessFlowDto request, string userId);
        Task<Result<string>> RejectApprovalFlow(ProcessFlowDto request);
        Task<Result<List<ApprovalFlowDto>>> GetHistoryApprovalFlow(string rno);
    }
}