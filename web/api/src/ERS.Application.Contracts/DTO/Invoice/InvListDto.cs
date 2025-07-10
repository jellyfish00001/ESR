namespace ERS.DTO.Invoice
{
    public class InvListDto
    {
        public string expcode { get; set; }
        public string expname { get; set; }
        public string seq { get; set; }
        public string invcode { get; set; }
        public string invno { get; set; }
        public string invtype { get; set; }
        public string invdate { get; set; }
        public decimal? oamount { get; set; }//含税总金额
        public decimal? taxamount { get; set; }//税额
        public decimal? amount { get; set; }//扣除税额后的金额
        public string curr { get; set; }
        // public List<InvoiceDetailDto> invoices { get; set; }
    }
}