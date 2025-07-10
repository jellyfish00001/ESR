using System;
namespace ERS.DTO.DataSync
{
    public class OraCashDetailDto
    {
        public string FORM_CODE { get; set; }
        public string RNO { get; set; }
        public decimal ITEM { get; set; }
        public DateTime? RDATE { get; set; }
        public string CURRENCY { get; set; }
        public decimal AMOUNT1 { get; set; }
        public string DEPTID { get; set; }
        public string SUMMARY { get; set; }
        public string OBJECT { get; set; }
        public string KEEP { get; set; }
        public string REMARKS { get; set; }
        public decimal? AMOUNT2 { get; set; }
        public string BASE_CURR { get; set; }
        public decimal? BASE_AMT { get; set; }
        public decimal? RATE { get; set; }
        public string EXPCODE { get; set; }
        public string EXPNAME { get; set; }
        public string ACCTCODE { get; set; }
        public string ACCTNAME { get; set; }
        public DateTime? REVSDATE { get; set; }
        public string PAYTYP { get; set; }
        public string PAYNAME { get; set; }
        public string PAYEE_ID { get; set; }
        public string PAYEE_NAME { get; set; }
        public string PAYEE_ACCOUNT { get; set; }
        public string BANK { get; set; }
        public string STAT { get; set; }
        public string PAYEE_DEPTID { get; set; }
        public decimal? ADVANCEAMOUNT { get; set; }
        public string ADVANCECURRENCY { get; set; }
        public decimal? ADVANCEBASEAMOUNT { get; set; }
        public string INVOICE { get; set; }
        public decimal? PRETAXAMOUNT { get; set; }
        public decimal? TAXEXPENSE { get; set; }
        public string TREATADDRESS { get; set; }
        public string OTHEROBJECT { get; set; }
        public decimal? OBJECTSUM { get; set; }
        public string KEEPCATEGORY { get; set; }
        public string OTHERKEEP { get; set; }
        public decimal? KEEPSUM { get; set; }
        public string ISACCORDNUMBER { get; set; }
        public string NOTACCORDREASON { get; set; }
        public decimal? ACTUALEXPENSE { get; set; }
        public string ISACCORDCOST { get; set; }
        public decimal? OVERBUDGET { get; set; }
        public string PROCESSMETHOD { get; set; }
        public string CUSTSUPERME { get; set; }
        public decimal FLAG { get; set; }
        public string UNIFYCODE { get; set; }
        public string TREATTIME { get; set; }
        public decimal? TAXBASEAMOUNT { get; set; }
        public string ASSIGNMENT { get; set; }
        public DateTime? HOSPDATE { get; set; }
        public string CUSER { get; set; }
        public DateTime? CDATE { get; set; }
        public DateTime? MDATE { get; set; }
        public string MUSER { get; set; }
    }
}