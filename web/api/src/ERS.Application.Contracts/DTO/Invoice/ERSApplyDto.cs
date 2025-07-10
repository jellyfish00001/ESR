using System.Collections.Generic;
namespace ERS.DTO.Invoice
{
    public class ERSApplyDto
    {
        public string Paymentstat { get; set; }
        public string ERSRno { get; set; }
        public string ERSUser { get; set; }
        public string ERSComCode { get; set; }
        public List<ERSInvDto> Invoices { get; set; }
    }
}