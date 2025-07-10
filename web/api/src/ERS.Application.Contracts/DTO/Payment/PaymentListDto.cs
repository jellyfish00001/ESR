using System;
namespace ERS.DTO.Payment
{
    public class PaymentListDto
    {
        public string sysno { get; set; }//系统单号
        public string cuser { get; set; }//上传人
        public decimal amt { get; set; }//金额
        public string bank { get; set; }//银行
        public DateTime? payment { get; set; }//付款日期
        public string company { get; set; }//公司别
        public string identification { get; set; }
        public string no { get; set; }
    }
}