using System;
using System.Collections.Generic;
using ERS.DTO.Application;
namespace ERS.Application.Contracts.DTO.PapreSign
{
    public class PaperDto
    {
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
        /// 步骤
        /// </summary>
        /// <value></value>
        public decimal? step { get; set; }
        /// <summary>
        /// 步骤名
        /// </summary>
        /// <value></value>
        public string stepname { get; set; }
        public string apid { get; set; }
        /// <summary>
        /// 发票清单
        /// </summary>
        /// <value></value>
        public List<CashFileDto> invlist { get; set; }
        /// <summary>
        /// 付款时间
        /// </summary>
        /// <value></value>
        public DateTime? payment { get; set; }
        public string status { get; set; }
        public string emplid { get; set; }
    }
}