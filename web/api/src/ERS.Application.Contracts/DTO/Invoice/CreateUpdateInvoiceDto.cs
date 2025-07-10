using System;
using System.ComponentModel.DataAnnotations;
namespace ERS.Application.Contracts.DTO.Invoice
{
    public class CreateUpdateInvoiceDto
    {
        [Required]
        public string rno { get; set; }
        [Required]
        public int seq { get; set; }
        [Required]
        public int item { get; set; }
        //发票代码
        public string invcode { get; set; }
        //发票号码
        public string invno { get; set; }
        //开票金额
        public decimal amount { get; set; }
        //开票日期
        public DateTime? invdate { get; set; }
        //税额
        public decimal taxamount { get; set; }
        //价税合计金额
        public decimal oamount { get; set; }
        //发票状态
        public string invstat { get; set; }
        //异常报销金额
        public decimal abnormalamount { get; set; }
        //税金损失
        public decimal taxloss { get; set; }
        //币别
        public string curr { get; set; }
        //承担方
        public string undertaker { get; set; }
        public string reason { get; set; }
        public string abnormal { get; set; }
    }
}