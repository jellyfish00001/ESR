using System;
using System.Collections.Generic;
namespace ERS.DTO.Report
{
    /// <summary>
    /// 已簽核表單查詢參數DTO
    /// </summary>
    public class QuerySignedReportDto
    {
        /// <summary>
        /// 開始日期（簽核日期）
        /// </summary>
        /// <value></value>
        public DateTime? sdate { get; set; }
        /// <summary>
        /// 結束日期（簽核日期）
        /// </summary>
        /// <value></value>
        public DateTime? edate { get; set; }
        /// <summary>
        /// 申請單號
        /// </summary>
        /// <value></value>
        public string rno { get; set; }
        /// <summary>
        /// 申請人
        /// </summary>
        /// <value></value>
        public string cuser { get; set; }
        /// <summary>
        /// 簽核人
        /// </summary>
        /// <value></value>
        public string aemplid { get; set; }
        /// <summary>
        /// 状态（EFormHead）
        /// </summary>
        /// <value></value>
        public List<string> status { get; set; }
        /// <summary>
        /// 憑證類別
        /// </summary>
        /// <value></value>
        public List<string> category { get; set; }
        /// <summary>
        /// 單據類型
        /// </summary>
        /// <value></value>
        public List<string> formcode { get; set; }
        /// <summary>
        /// 公司別
        /// </summary>
        /// <value></value>
        public List<string> company { get; set; }
    }
}