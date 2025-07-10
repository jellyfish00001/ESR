using System.Collections.Generic;
namespace ERS.DTO.Account
{
    /// <summary>
    /// 入账清单DTO(QueryForm)
    /// </summary>
    public class QueryFormDto
    {
        /// <summary>
        /// 申请单号
        /// </summary>
        /// <value></value>
        public List<string> rno { get; set; }
        /// <summary>
        /// 公司别
        /// </summary>
        /// <value></value>
        public string company { get; set; }
        /// <summary>
        /// 签核会计
        /// </summary>
        /// <value></value>
        public string cemplid { get; set; }
        /// <summary>
        /// 银行
        /// </summary>
        /// <value></value>
        public string bank { get; set; }
    }
}