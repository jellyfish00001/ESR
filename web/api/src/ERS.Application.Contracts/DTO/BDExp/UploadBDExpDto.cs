namespace ERS.DTO.BDExp
{
    /// <summary>
    /// 批量上传费用情景结果Dto
    /// </summary>
    public class UploadBDExpDto
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
        /// 上传的情景信息
        /// </summary>
        /// <value></value>
        public AddBDExpDto data { get; set; }
    }
}