using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.Application;
using ERS.DTO.ApprovalFlow;
using ERS.Entities;
using ERS.IDomainServices;
using Volo.Abp.Application.Services;
using Volo.Abp.ObjectMapping;

namespace ERS.Services
{
    public class ApprovalFlowService : ApplicationService, IApprovalFlowService
    {
        private IApprovalFlowDomainService _ApprovalFlowDomainService;
       private IObjectMapper _ObjectMapper;

        public ApprovalFlowService(IApprovalFlowDomainService iApprovalFlowDomainService,
                                IObjectMapper iObjectMapper)
        {
            _ApprovalFlowDomainService = iApprovalFlowDomainService;
            _ObjectMapper = iObjectMapper;
        }

        public async Task<Result<ApprovalFlowDto>> GetApprovalFlowById(Guid? id){
            return await _ApprovalFlowDomainService.GetApprovalFlowById(id);
        }

        public async Task<Result<List<ApprovalFlowDto>>> GetApprovalFlowByRno(string rno){
            return await _ApprovalFlowDomainService.GetApprovalFlowByRno(rno);
        }

        public async Task<Result<string>> ApplyApprovalFlow(CashHeadDto cashHead,  List<CashDetailDto> cashDetails){
            CashHead cashHeadEntity = _ObjectMapper.Map<CashHeadDto, CashHead>(cashHead);
            List<CashDetail> cashDetailEntities = _ObjectMapper.Map<List<CashDetailDto>, List<CashDetail>>(cashDetails);
            return await _ApprovalFlowDomainService.ApplyApprovalFlow(cashHeadEntity, cashDetailEntities);
        }

        public async Task<Result<List<ApprovalFlowDto>>> GetCurrentApprovalFlow(string rno)
        {
            return await _ApprovalFlowDomainService.GetCurrentApprovalFlow(rno);
        }

        public async Task<Result<List<ApprovalFlowDto>>> GetCurrentApprovalFlowList(string rno)
        {
            return await _ApprovalFlowDomainService.GetCurrentApprovalFlowList(rno);
        }

        public async Task<Result<string>> ForwardApprovalFlow(ProcessFlowDto request)
        {
            return await _ApprovalFlowDomainService.ForwardApprovalFlow(request);
        }

        public async Task<Result<string>> DeleteApprovalFlow(ProcessFlowDto request)
        {
            return await _ApprovalFlowDomainService.DeleteApprovalFlow(request);
        }
        public async Task<Result<string>> ApproveApprovalFlow(ProcessFlowDto request, string userId)
        {
            return await _ApprovalFlowDomainService.ApproveApprovalFlow(request, userId);
        }
        public async Task<Result<string>> RejectApprovalFlow(ProcessFlowDto request)
        {
            return await _ApprovalFlowDomainService.RejectApprovalFlow(request);
        }
        public async  Task<Result<List<ApprovalFlowDto>>> GetHistoryApprovalFlow(string rno)
        {
            return await _ApprovalFlowDomainService.GetHistoryApprovalFlow(rno);
        }
    }
}