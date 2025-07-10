namespace ERS.DTO.Nickname
{
    public class UploadNickNameDto
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
        /// 上傳的數據
        /// </summary>
        /// <value></value>
        public NickNameCommonDto data { get; set; }
    }
}