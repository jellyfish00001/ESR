using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Attribute;
using ERS.DTO;
using ERS.DTO.Application;
using ERS.DTO.ApprovalFlow;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ERS.Controllers
{
#if DEBUG
    [AllowAnonymous]
#endif     
    [Route("api/[controller]")]
    public class ApprovalFlowController : BaseController
    {
        private IApprovalFlowService _ApprovalFlowService;
                
        public ApprovalFlowController(IApprovalFlowService iApprovalFlowService)
        {
            _ApprovalFlowService = iApprovalFlowService;
        }

        //根據ID查詢單一流程步驟
        [HttpPost("queryById")]
        [Permission("ers.BDExpenseDept.View")]
        public async Task<Result<ApprovalFlowDto>> GetApprovalFlowById(Guid id)
        {
            return await _ApprovalFlowService.GetApprovalFlowById(id);
        }
                
        //起單
        [HttpPost("applyFlow")]
        public async Task<Result<string>> ApplyApprovalFlow(IFormCollection formCollection)
        {
            string head = formCollection["head"];
            string detail = formCollection["detail"];
            CashHeadDto cashHead = JsonConvert.DeserializeObject<CashHeadDto>(head);
            List<CashDetailDto> cashDetails = JsonConvert.DeserializeObject<List<CashDetailDto>>(detail);
            return await _ApprovalFlowService.ApplyApprovalFlow(cashHead, cashDetails);
        }

        //取得當前簽核流程
        [HttpGet("getCurrentFlow")]
        //[Permission("ers.BDExpenseDept.View")]
        public async Task<Result<List<ApprovalFlowDto>>> GetCurrentApprovalFlow(string rno)
        {
            return await _ApprovalFlowService.GetCurrentApprovalFlow(rno);
        }

        //取得當前簽核流程列表
        [HttpGet("getCurrentFlowList")]
        public async Task<Result<List<ApprovalFlowDto>>> GetCurrentApprovalFlowList(string rno)
        {
            return await _ApprovalFlowService.GetCurrentApprovalFlowList(rno);
        }

        //轉單/邀簽
        [HttpPost("forwardFlow")]
        //[Permission("ers.BDExpenseDept.View")]
        public async Task<Result<string>> ForwardApprovalFlow(ProcessFlowDto request)
        {
           return null;
        }

        //刪單
        [HttpDelete()]
        [Permission("ers.BDExpenseDept.View")]
        public async Task<Result<string>> DeleteApprovalFlow(ProcessFlowDto request)
        {
           return null;
        }

        //同意
        [HttpPut("approveFlow")]
        //[Permission("ers.BDExpenseDept.View")]
        public async Task<Result<string>> ApproveApprovalFlow([FromBody] ProcessFlowDto request)
        {
            return await _ApprovalFlowService.ApproveApprovalFlow(request, this.userId);
        }

        //拒絕
        [HttpPut("rejectFlow")]
        //[Permission("ers.BDExpenseDept.View")]
        public async Task<Result<string>> RejectApprovalFlow(ProcessFlowDto request)
        {
            return await _ApprovalFlowService.RejectApprovalFlow(request);
        }

        //取得歷史簽核
        [HttpGet("getHistoryFlow")]
        //[Permission("ers.BDExpenseDept.View")]
        public async Task<Result<List<ApprovalFlowDto>>> GetHistoryApprovalFlow(string rno)
        {
            return await _ApprovalFlowService.GetHistoryApprovalFlow(rno);
        }

        
    }
}