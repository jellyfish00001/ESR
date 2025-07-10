using System;
namespace ERS.DTO.Proxy
{
    public class EditProxyCashDto
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
    }
}