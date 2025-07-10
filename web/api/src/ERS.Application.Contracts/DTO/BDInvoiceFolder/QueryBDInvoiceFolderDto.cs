using System;
namespace ERS.DTO.BDInvoiceFolder
{
    public class QueryBDInvoiceFolderDto
    {
        /// <summary>
        /// 开始日期
        /// </summary>
        /// <value></value>
        public DateTime? startdate { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        /// <value></value>
        public DateTime? enddate { get; set; }
        // /// <summary>
        // /// 申请人
        // /// </summary>
        // /// <value></value>
        // public string emplid { get; set; }
        /// <summary>
        /// 发票号码
        /// </summary>
        /// <value></value>
        public string invno { get; set; }
        /// <summary>
        /// 发票类型
        /// </summary>
        /// <value></value>
        public string invtype { get; set; }
        /// <summary>
        /// 核验状态
        /// </summary>
        /// <value></value>
        public string verifytype { get; set; }
        /// <summary>
        /// 请款状态
        /// </summary>
        /// <value></value>
        public string paytype { get; set; }
        /// <summary>
        /// 是否为手机版
        /// </summary>
        /// <value></value>
        public bool isphone { get; set; }
    }
}