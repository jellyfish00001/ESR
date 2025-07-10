using System.Collections.Generic;
namespace ERS.DTO.Application
{
    public class ApplicationDto
    {
        public CashHeadDto head { get; set; }
        public List<CashDetailDto> detail { get; set; }
        public CashAmountDto amount { get; set; }
        public List<CashFileDto> file { get; set; }
        public List<InvoiceDto> invList { get; set; }
    }
}