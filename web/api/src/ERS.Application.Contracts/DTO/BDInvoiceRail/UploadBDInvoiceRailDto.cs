namespace ERS.DTO.BDInvoiceRail
{
    /// <summary>
    /// 批量上传费用情景结果Dto
    /// </summary>
    public class UploadBDInvoiceRailDto
    {
        /// <summary>
        /// 上传结果 1成功 2失败
        /// </summary>
        /// <value></value>
        public int status { get; set; }
        /// <summary>
        /// 成功失败都会有提示信息
        /// </summary>
        /// <value></value>
        public string uploadmsg { get; set; }
        /// <summary>
        /// 第幾行數據
        /// </summary>
        /// <value></value>
        public int seq { get; set; }
        /// <summary>
        /// 上传的发票字轨信息
        /// </summary>
        /// <value></value>
        public AddBDInvoiceRailDto data { get; set; }
    }
}