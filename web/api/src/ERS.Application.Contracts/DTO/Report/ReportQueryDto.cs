using System;
using System.Collections.Generic;
namespace ERS.DTO.Report
{
    public class ReportQueryDto
    {
        public DateTime? startdate { get; set; }// 开始日期
        public DateTime? enddate { get; set; }// 结束日期
        public string rno { get; set; }// 申请单号
        public string cemplid { get; set; }// 申请人
        public List<string> status { get; set; }// 表单状态
        public List<string> category { get; set; } // 凭证类别
        public List<string> formcode { get; set; }// 单据类型
        public List<string> company { get; set; }// 公司
    }
}