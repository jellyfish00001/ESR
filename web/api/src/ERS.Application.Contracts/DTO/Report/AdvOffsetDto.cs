using System;
namespace ERS.DTO.Report
{
    public class AdvOffsetDto //预支冲账查询
    {
        public string company { get; set; } // 公司別
        public string cname { get; set; } // 申請人姓名
        public string cuser { get; set; } // 申請人工號
        public string payeename { get; set; } // 收款人姓名
        public string payeeId { get; set; } // 收款人工號
        public string rno { get; set; } // 預支金單號
        public DateTime? cdate { get; set; } // 申请日期
        public string expname { get; set; } // 預支（報銷）場景
        public string summary { get; set; } // 摘要
        public decimal amount { get; set; } // 申请金额
        public string reverserno { get; set; }// 沖賬ERS單號
        public decimal reverseamt { get; set; } // 已沖賬金額
        public decimal payamt { get; set; } // 已繳款金額
        public decimal unreversamt { get; set; } // 未沖賬金額
        public DateTime? applyextdate { get; set; } // 申請延期日期
        public DateTime? delayexpdate { get; set; } // 延期截止日
        public string recipientid { get; set; } // 接收人工號
        public DateTime? transferdate { get; set; } // 轉單日期
    }
}