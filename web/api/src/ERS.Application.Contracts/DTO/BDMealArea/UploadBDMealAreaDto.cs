namespace ERS.DTO.BDMealArea
{
    public class UploadBDMealAreaDto
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
        /// 上傳數據
        /// </summary>
        /// <value></value>
        public AddBDMealAreaDto data { get; set; }
    }
}