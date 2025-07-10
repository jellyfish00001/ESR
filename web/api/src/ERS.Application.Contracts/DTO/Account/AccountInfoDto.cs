using System;
namespace ERS.DTO.Account
{
    public class AccountInfoDto
    {
        public int psitem { get; set; }
        public string postkey { get; set; }
        public string acctcode { get; set; }
        public Decimal? baseamt { get; set; }
        public string txtcode { get; set; }
        public string costcenter { get; set; }
        public string asinmnt { get; set; }
        public string payeeid { get; set; }
        public string invoice { get; set; }
        public Decimal taxbase { get; set; }
        public Decimal lctaxbase { get; set; }
        public DateTime? rdate { get; set; }
        public string unifycode { get; set; }
        public string certificate { get; set; }
        public int item { get; set; }
        public string ref1 { get; set; }
        public string expcode { get; set; }
        public string lintext { get; set; }
        public string expname { get; set; }
        public string deptid { get; set; }
    }
}