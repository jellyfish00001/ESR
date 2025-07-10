using System;
using System.Collections.Generic;
namespace ERS.DTO.FinApprover
{
    public class FinApproverParamsDto
    {
        public Guid? Id { get; set; }
        public List<string> companyList { get; set; }
        public string company { get; set; }
        public string company_code { get; set; }
        public string plant { get; set; }
        public string rv1 { get; set; }
        public string rv2 { get; set; }
        public string rv3 { get; set; }
        public decimal category { get; set; }
        public decimal signStep { get; set; }
    }
}