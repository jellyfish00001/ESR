using System;

namespace ERS.DTO
{
    public class OcrResultDto
    {
        public string Id { get; set; }
        public string IdentificationNo { get; set; }
        public string InvoiceNo { get; set; }
        public string InvoiceCode { get; set; }
        public string InvoiceTitle { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string InvoiceType { get; set; }
        public decimal? UnTaxAmount { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? Amount { get; set; }
        public string Currency { get; set; }
        public string BuyerTaxId { get; set; }
        public string BuyerName { get; set; }
        public string SellerTaxId { get; set; }
        public string SellerName { get; set; }
        public decimal? TaxRate { get; set; }
        public string TaxBase { get; set; }
        public decimal? ImportTaxAmount { get; set; }
        public decimal? ServiceFee { get; set; }
        public decimal? ShippingFee { get; set; }
        public decimal TransactionFee { get; set; }
        public decimal? Quantity { get; set; }
        public string ProductInfo { get; set; }
        public string StartStation { get; set; }
        public string EndStation { get; set; }
        public string Remarks { get; set; }
        public string SereialNoA { get; set; }
        public string SereialNoB { get; set; }
        public string SereialNoC { get; set; }
        public decimal AmountA { get; set; }
        public decimal AmountB { get; set; }
        public decimal AmountC { get; set; }
    }
}
