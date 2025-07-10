using System;
using System.Collections.Generic;

namespace ERS.DTO.ApprovalFlow
{
    public class ApprovalFlowDto
    {
        public Guid Id { get; set; }
        public string rno { get; set; } // 報銷單號
        public decimal step { get; set; } // 步驟順序
        public string stepname { get; set; } // 步驟名稱
        public string approveremplid { get; set; } // 實際簽核人工號
        public string approvername { get; set; } // 實際簽核人姓名
        public string approverdeptid { get; set; } // 實際簽核人部門代碼
        public List<string> assignedEmplids { get; set; } = new List<string>(); // 被指派的簽核人員工號
        public string status { get; set; } // 簽核狀態
        public Guid nextflowid { get; set; } // 下一步驟簽核流程ID
        public string company{ get; set; } //公司別
        public DateTime cdate { get; set; } // 簽核日期
        public string comment { get; set; } // 簽核意見
        public decimal importTax { get; set; }
        public decimal tradePromotionFee { get; set; }
        public decimal totalTaxAndFee { get; set; }
        public string agentEmplid { get; set; }
        public DateTime approvedate { get; set; } // 簽核日期
    }
}