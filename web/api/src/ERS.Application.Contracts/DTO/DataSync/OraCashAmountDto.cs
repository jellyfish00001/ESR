using System;
namespace ERS.DTO.DataSync
{
    public class OraCashAmountDto
    {
        public string FORM_CODE { get; set; }
        public string RNO { get; set; }
        public string CURRENCY { get; set; }
        public decimal AMOUNT { get; set; }
        public string ACTPAY { get; set; }
        public decimal ACTAMT { get; set; }
        public string CUSER { get; set; }
        public DateTime? CDATE { get; set; }
        public string MUSER { get; set; }
        public DateTime? MDATE { get; set; }
    }
}