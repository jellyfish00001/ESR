namespace ERS.DTO.Application.BcfpInvoice
{
    public class BillRecordDto
    {
        public string bill_code { get; set; }
        public string bill_num { get; set; }
        public string tx_hash { get; set; }
        public decimal amount { get; set; }
        public decimal total_amount { get; set; }
        public long time { get; set; }
        public decimal tax_amount { get; set; }
        public string seller_name { get; set; }
        public string seller_taxpayer_id { get; set; }
        public string buyer_name { get; set; }
        public int status { get; set; }
        public int invalid_mark { get; set; }
    }
}