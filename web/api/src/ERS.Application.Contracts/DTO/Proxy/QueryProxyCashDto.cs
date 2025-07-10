namespace ERS.DTO.Proxy
{
    /// <summary>
    /// 查詢條件Dto
    /// </summary>
    public class QueryProxyCashDto
    {
        /// <summary>
        /// 报销人工号
        /// </summary>
        /// <value></value>
        public string aemplid { get; set; }
        /// <summary>
        /// 代报销人工号
        /// </summary>
        /// <value></value>
        public string remplid { get; set; }
    }
}