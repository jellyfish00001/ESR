using System.Collections.Generic;
namespace ERS.DTO.BDExp
{
    public class BDExpParamDto
    {
        public string expcode { get; set; }
        public string senarioname { get; set; }
        public string acctcode { get; set; }
        public string company { get; set; }
        public List<string> companyList { get; set; }
    }
}