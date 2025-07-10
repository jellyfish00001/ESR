using System;
namespace ERS.DTO.BDInvoiceRail
{
    public class BDInvoiceRailDto
    {
        public Guid Id { get; set; }
        public string qi { get; set; }
        public string invoicerail { get; set; }
        public decimal year { get; set; }
        public decimal month { get; set; }
        public decimal formatcode { get; set; }
        public string invoicetype { get; set; }
        public DateTime? cdate { get; set; }
        public string cuser { get; set; }
        public DateTime? mdate { get; set; }
        public string muser { get; set; }
        public string company { get; set; }
    }
}