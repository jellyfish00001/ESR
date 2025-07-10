using System.Collections.Generic;
using ERS.DTO.Application;
namespace ERS.Application.Contracts.DTO.Invoice
{
    public class InvoiceResult
    {
        public List<InvoiceDto> list { get; set; }
        public decimal amount { get; set; }//税金损失加总
    }
}