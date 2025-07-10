using System.Collections.Generic;
namespace ERS.DTO.BDAccount
{
    public class BDAccountParamDto
    {
        public string acctcode { get; set; }
        public string acctname { get; set; }
        public List<string> companyList { get; set; }
        public string company { get; set; }
    }
}