using System;
namespace ERS.DTO.DataSync
{
    public class OraCashHeadDto
    {
        public string FORM_CODE { get; set; }
        public string RNO { get; set; }
        public string CUSER { get; set; }
        public string CNAME { get; set; }
        public string DEPTID { get; set; }
        public string EXT { get; set; }
        public string COMPANY { get; set; }
        public string PROJECT_CODE { get; set; }
        public string CURRENCY { get; set; }
        public decimal? AMOUNT { get; set; }
        public string PAYEE_ID { get; set; }
        public string PAYEE_NAME { get; set; }
        public string PAYEE_ACCOUNT { get; set; }
        public DateTime? CDATE { get; set; }
        public string MUSER { get; set; }
        public DateTime? MDATE { get; set; }
        public string WELFARE { get; set; }
        public string BANK { get; set; }
        public DateTime? PAYMENT { get; set; }
        public string DTYPE { get; set; }
        public decimal? L2_L3_SEQ { get; set; }
        public string ACTUALAMOUNT { get; set; }
        public string COMPANY_SAP { get; set; }
        public string STAT { get; set; }
        public string ORIGINALCURRENCY { get; set; }
        public string WHETHERAPPROVE { get; set; }
        public string APPROVEREASON { get; set; }
        public string EXPENSETYPE { get; set; }
        public string VENDOR { get; set; }
        public DateTime? PAYMENTDATE { get; set; }
        public string EXPENSETYPEDESC { get; set; }
        public string VENDORDESC { get; set; }
        public string UNIFYCODE { get; set; }
        public string ISCLOUDINVOICE { get; set; }
    }
}