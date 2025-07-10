using System.Collections.Generic;
namespace ERS.DTO.Account
{
    /// <summary>
    /// 入账清单查询参数Dto
    /// </summary>
    public class QueryFormParamDto
    {
        public List<string> companyList { get; set; }// 公司别
        public string cemplid { get; set; }// 签核会计
        public string bank { get; set; }//银行
    }
}