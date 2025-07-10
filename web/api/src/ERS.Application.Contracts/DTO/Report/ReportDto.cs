using System;
using System.Collections.Generic;
using ERS.DTO.Application;
namespace ERS.Application.Contracts.DTO.Report
{
    public class ReportDto
    {
        /// <summary>
        /// 公司别
        /// </summary>
        /// <value></value>
        public string company { get; set; }
        /// <summary>
        /// 申请单号
        /// </summary>
        /// <value></value>
        public string rno { get; set; }
        /// <summary>
        /// 表单类型
        /// </summary>
        /// <value></value>
        public string formcode { get; set; }
        /// <summary>
        /// 表單名稱
        /// </summary>
        /// <value></value>
        public string formname { get; set; }
        /// <summary>
        /// 申请部门
        /// </summary>
        /// <value></value>
        public string deptid { get; set; }
        /// <summary>
        /// 申请人
        /// </summary>
        /// <value></value>
        public string cemplid { get; set; }
        /// <summary>
        /// 申请人名
        /// </summary>
        /// <value></value>
        public string cname { get; set; }
        /// <summary>
        /// 申请日期
        /// </summary>
        /// <value></value>
        public DateTime? cdate { get; set; }
        /// <summary>
        /// 费用类型
        /// </summary>
        /// <value></value>
        public string expname { get; set; }
        /// <summary>
        /// 费用类型
        /// </summary>
        /// <value></value>
        public string expensetype { get; set; }
        /// <summary>
        /// 费用归属部门
        /// </summary>
        /// <value></value>
        public string expdeptid { get; set; }
        /// <summary>
        /// 币别
        /// </summary>
        /// <value></value>
        public string currency { get; set; }
        /// <summary>
        /// 实际金额
        /// </summary>
        /// <value></value>
        public decimal? actamt { get; set; }
        /// <summary>
        /// 步骤
        /// </summary>
        /// <value></value>
        public decimal? step { get; set; }
        /// <summary>
        /// 步骤名
        /// </summary>
        /// <value></value>
        /// 
        public string stepname { get; set; }
        /// <summary>
        /// 摘要
        /// </summary>
        /// <value></value>
        public string summary { get; set; }
        /// <summary>
        /// 費用/憑證日期
        /// </summary>
        /// <value></value>
        /// 
        public DateTime? rdate { get; set; }
        /// <summary>
        /// 发票号码
        /// </summary>
        /// <value></value>
        public string invno { get; set; }
        /// <summary>
        /// 发票编码
        /// </summary>
        /// <value></value>
        /// 
        public string invcode { get; set; }
        /// <summary>
        /// 稅額
        /// </summary>
        /// <value></value>
        public decimal? taxamount { get; set; }
        /// <summary>
        /// 税前总额
        /// </summary>
        /// <value></value>
        ///// 
        public decimal? untaxamount { get; set; }
        /// <summary>
        /// 稅額
        /// </summary>
        /// <value></value>
        public string untaxamountDisplay { get; set; }
        /// <summary>
        /// 税前总额
        /// </summary>
        /// <value></value>
        ///// 
        public string taxamountDisplay  { get; set; }
        /// <summary>
        /// 收款人
        /// </summary>
        /// <value></value>
        ///// 
        public string payeeId { get; set; }
        /// <summary>
        /// 收款人姓名
        /// </summary>
        /// <value></value>
        ///// 
        public string payeename { get; set; }
        /// <summary>
        /// projectcode
        /// </summary>
        /// <value></value>
        /// 
        public string projectcode { get; set; }
        public DateTime? payment { get; set; }
        public string paymentdate { get; set; }// 付款日期
        public string apid { get; set; }
        public string status { get; set; }
        public List<CashFileDto> invlist { get; set; }// 发票清单
        public List<string> invoices { get; set; }
        public List<string> attachments { get; set; }
        public string isAbnormal { get; set; }
        public List<string> abnormalReson { get; set; }
    }
}
