using System;
namespace ERS.DTO.Payment
{
    public class PaymentDetailDto
    {
        public int sysitem { get; set; }//序号
        public string deptid { get; set; }//部门代码
        public string scname { get; set; }//户名
        public string bank { get; set; }//银行别
        public decimal amt { get; set; }//金额
        public string usage { get; set; }//用途
        public string docno { get; set; }//传票号
        public DateTime? payment { get; set; }//入账日期
        public string rno { get; set; }//申请单号
        public string contnt { get; set; }//内容
        public string paymentmethod { get; set; }//支付方式
    }
}