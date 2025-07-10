using System;
namespace ERS.DTO.Account
{
    public class QueryPostingDto
    {
        public string rno { get; set; }// 申请单号
        public string formname { get; set; }// 单据类型（以单据名呈现）
        public string cuser { get; set; }// 申请人工号
        public string cname { get; set; }// 申请人中文名
        public DateTime? cdate { get; set; }// 申请日期
        public decimal actamt { get; set; }// 金额
        public string currency { get; set; }// 币别
        public string company { get; set; }// 公司别
        public string cemplid { get; set; }// 签核会计
        public string bank { get; set; }// 银行
        public string apid { get; set; }
    }
}