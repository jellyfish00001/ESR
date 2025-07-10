using System;
namespace ERS.DTO.Application
{
    public class OvertimeMealDto
    {
        public string expname { get; set; }// 报销情景
        public string expcode { get; set; }// 报销情景代码
        public string deptid { get; set; }//费用归属部门
        public string businesscity { get; set; }//公出城市
        public DateTime cdate { get; set; }//费用发生日期
        public DateTime departuretime { get; set; }//出发时点 具体到分
        public DateTime backtime { get; set; }//回到时点 具体到分
        public string summary { get; set; }//摘要
        public string currency { get; set; }//币别
        public decimal amount { get; set; }//报销金额
        public decimal rate { get; set; }//汇率
        public decimal baseamt { get; set; }//本位币金额
        public string payeeid { get; set; }//收款人工號
        public string payeename { get; set; }//收款人姓名
        public string bank { get; set; }//银行别
        public string payeedept { get; set; }//收款人部門
    }
}