using System;
namespace ERS.DTO.Payment
{
    //付款清单下载Excel所需属性
    public class RemittanceExcelDto
    {
        //付款清单点击download下载Excel所需属性
        public string seq { get; set; }
        public string payeeaccount { get; set; }
        public string bank { get; set; }
        public string scname { get; set; }
        public decimal amt { get; set; }
        public string usage { get; set; }
        public string emplid { get; set; }
        public DateTime? payment { get; set; }
        //以下为点击sysno下载Excel所需额外属性
        public string deptid { get; set; }
        public string payeeid { get; set; }
        public string docno { get; set; }
        public DateTime? postdate { get; set; }
        public string contnt { get; set; }
        public string identification { get; set; }
        public string company { get; set; }
        public string ename { get; set; }
    }
}