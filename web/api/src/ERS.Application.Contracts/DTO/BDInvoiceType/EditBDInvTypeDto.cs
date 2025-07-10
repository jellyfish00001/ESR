using System;
namespace ERS.DTO.BDInvoiceType
{
    public class EditBDInvTypeDto
    {
        public Guid Id { get; set; }
        public string invtypecode { get; set; }
        public string invtype { get; set; }
        public string area { get; set; }
        public string category { get; set; }
        public string company { get; set; }
    }
}