using System;

namespace ERS.DTO.ApprovalFlow
{
    public class ProcessFlowDto
    {   
        public string rno { get; set; } // 報銷單號
        public string approverEmplid { get; set; } // 實際簽核人工號
        public string? inviteEmplid { get; set; } // 邀簽簽核人工號
        public int? inviteMethod { get; set; } // -1:前置邀簽 1:後置邀簽
        public string comment { get; set; } // 簽核意見
    }
}