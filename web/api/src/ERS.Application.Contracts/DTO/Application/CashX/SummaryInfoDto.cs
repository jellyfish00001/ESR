namespace ERS.DTO.Application.CashX
{
    /// <summary>
    /// 薪资请款生成摘要提交参数
    /// </summary>
    public class SummaryInfoDto
    {
        /// <summary>
        /// 薪资所属期（年月 YYMM）
        /// </summary>
        /// <value></value>
        public string summary { get; set; }
        /// <summary>
        /// 薪资请款费用情景中的keyword
        /// </summary>
        /// <value></value>
        public string keyword { get; set; }
        /// <summary>
        /// 汇款方式 现金：现金，汇款：银行别 对应detail当中的payname
        /// </summary>
        /// <value></value>
        public string payname { get; set; }
        /// <summary>
        /// 公司别代码
        /// </summary>
        /// <value></value>
        public string companycode { get; set; }
    }
}