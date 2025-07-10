using System;
namespace ERS.DTO.BDInvoiceType
{
    public class BDInvoiceTypeDto
    {
        public Guid Id { get; set; }
        public string invtypecode { get; set; }
        public string invtype { get; set; }
        public string area { get; set; }
        public string category { get; set; }
        public DateTime? cdate { get; set; }
        public string cuser { get; set; }
        public DateTime? mdate { get; set; }
        public string muser { get; set; }
        public string company { get; set; }
    }
}