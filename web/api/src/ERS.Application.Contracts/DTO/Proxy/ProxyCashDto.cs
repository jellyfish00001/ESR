using System;
namespace ERS.DTO.Proxy
{
    /// <summary>
    /// 查詢結果Dto
    /// </summary>
    public class ProxyCashDto
    {
        /// <summary>
        /// id
        /// </summary>
        /// <value></value>
        public Guid Id { get; set; }
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
        /// <summary>
        /// 创建人
        /// </summary>
        /// <value></value>
        public string cuser { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        /// <value></value>
        public DateTime? cdate { get; set; }
        /// <summary>
        /// 修改人
        /// </summary>
        /// <value></value>
        public string muser { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        /// <value></value>
        public DateTime? mdate { get; set; }
    }
}