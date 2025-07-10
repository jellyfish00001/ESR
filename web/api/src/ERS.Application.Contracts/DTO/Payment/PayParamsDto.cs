using System;
using System.Collections.Generic;
namespace ERS.DTO.Payment
{
    public class PayParamsDto
    {
        public List<string> companyList { get; set; }
        public string company { get; set; }
        public DateTime? paymentdate { get; set; }
        public string identification { get; set; }
        public string sysno { get; set; }
        public string bank { get; set; }
        public string cuser { get; set; }
    }
}