using System;
using System.Collections.Generic;
namespace ERS.Model
{
    public class SignQuery
    {
        public IList<FormMsg> li { get; set; }
    }
    public class FormMsg
    {
        public string formNo { get; set; }
        public string systemCode { get; set; } = "ers";
        public string formCode { get; set; }
    }
    public class SignForm
    {
        public string formno { get; set; }
        public string form_code { get; set; }
        public decimal seq { get; set; }
        public string step { get; set; }
        // 应签核人
        public string emplid { get; set; }
        public string deptid { get; set; }
        // Finish Current
        public string status { get; set; }
        //实际签核人
        public string signer_emplid { get; set; }
        public string signer_deptid { get; set; }
        public string signer_ename { get; set; }
        public string signer_cname { get; set; }
        public string step_activity { get; set; }
        public string step_emplid { get; set; }
        public string cuser { get; set; }
        public string muser { get; set; }
        // Approve
        public string sign_result { get; set; }
        public string sign_remark { get; set; }
        public decimal sortNum { get; set; }
        public DateTime? sign_date { get; set; }
    }
}
