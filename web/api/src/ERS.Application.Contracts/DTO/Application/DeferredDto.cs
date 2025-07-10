using ERS.DTO.Application;
using System;
using System.Collections.Generic;
namespace ERS.Application.Contracts.DTO.Application
{
    public class DeferredDto
    {
        //延期冲账明细
        public string expcode { get; set; }
        public string expname { get; set; }
        public string company { get; set; }
        public string cname { get; set; }
        public string cuser { get; set; }
        public string payeeid { get; set; }
        public string payeename { get; set; }
        public string rno { get; set; }
        public string remark { get; set; }
        public decimal amount { get; set; }
        public decimal actamt { get; set; }
        public int opendays { get; set; }
        /// <summary>
        /// 逾期次数
        /// </summary>
        /// <value></value>
        public int delay { get; set; }
        /// <summary>
        /// 逾期天数
        /// </summary>
        /// <value></value>
        public int delaydays { get; set; }
        public DateTime revsdate { get; set; }
        public DateTime cdate { get; set; }
        public List<CashFileDto> file { get; set; }
    }
}