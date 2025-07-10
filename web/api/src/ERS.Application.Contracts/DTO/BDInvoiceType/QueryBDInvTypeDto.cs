using System.Collections.Generic;
namespace ERS.DTO.BDInvoiceType
{
    public class QueryBDInvTypeDto
    {
        public string invtypecode { get; set; }
        public string invtype { get; set; }
        public string area { get; set; }
        public string category { get; set; }
        public List<string> companylist { get; set; }
    }
}