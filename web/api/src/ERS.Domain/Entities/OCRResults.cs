using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ERS.Entities
{
    [Table("ocr_results")]
    [Index(nameof(IdentificationNo), Name = "ocr_results_idx_identification_no")]
    public class OCRResults : SuperBaseEntity
    {
        [StringLength(100)]
        [Comment("存票據上唯一值識別碼")]
        [Column("identification_no")]
        public string IdentificationNo { get; set; }

        [StringLength(50)]
        [Comment("發票號碼")]
        [Column("invoice_no")]
        public string InvoiceNo { get; set; }

        [StringLength(50)]
        [Comment("發票代碼")]
        [Column("invoice_code")]
        public string InvoiceCode { get; set; }

        [StringLength(100)]
        [Comment("發票標題")]
        [Column("invoice_title")]
        public string InvoiceTitle { get; set; }

        [Comment("發票日期")]
        [Column("invoice_date")]
        public DateTime? InvoiceDate { get; set; }

        [StringLength(50)]
        [Comment("發票類型名稱")]
        [Column("invoice_type")]
        public string InvoiceType { get; set; }

        [Comment("不含稅金額")]
        [Column("un_tax_amount", TypeName = "numeric(18,2)")]
        public decimal? UnTaxAmount { get; set; }

        [Comment("營業/增值稅額")]
        [Column("tax_amount", TypeName = "numeric(18,2)")]
        public decimal? TaxAmount { get; set; }

        [Comment("發票金額")]
        [Column("amount", TypeName = "numeric(18,2)")]
        public decimal? Amount { get; set; }

        [StringLength(20)]
        [Comment("幣別")]
        [Column("currency")]
        public string Currency { get; set; }

        [StringLength(100)]
        [Comment("購買方稅號")]
        [Column("buyer_tax_id")]
        public string BuyerTaxId { get; set; }

        [StringLength(100)]
        [Comment("購貨方名稱")]
        [Column("buyer_name")]
        public string BuyerName { get; set; }

        [StringLength(100)]
        [Comment("銷售方稅號")]
        [Column("seller_tax_id")]
        public string SellerTaxId { get; set; }

        [StringLength(100)]
        [Comment("銷售方名稱")]
        [Column("seller_name")]
        public string SellerName { get; set; }

        [Comment("營業/增值稅率")]
        [Column("tax_rate", TypeName = "numeric")]
        public decimal? TaxRate { get; set; }

        [Comment("營業/增值税基")]
        [Column("tax_base", TypeName = "numeric(18,2)")]
        public decimal? TaxBase { get; set; }

        [Comment("進口稅")]
        [Column("import_tax_amount", TypeName = "numeric(18,2)")]
        public decimal? ImportTaxAmount { get; set; }

        [Comment("服務費/推廣貿易服務費")]
        [Column("service_fee", TypeName = "numeric(18,2)")]
        public decimal? ServiceFee { get; set; }

        [Comment("運輸費")]
        [Column("shipping_fee", TypeName = "numeric(18,2)")]
        public decimal? ShippingFee { get; set; }

        [Comment("手續費")]
        [Column("transaction_fee", TypeName = "numeric(18,2)")]
        public decimal? TransactionFee { get; set; }

        [Comment("數量")]
        [Column("quantity", TypeName = "numeric(18,2)")]
        public decimal? Quantity { get; set; }

        [StringLength(500)]
        [Comment("商品資訊")]
        [Column("product_info")]
        public string ProductInfo { get; set; }

        [StringLength(100)]
        [Comment("出發站")]
        [Column("start_station")]
        public string StartStation { get; set; }

        [StringLength(100)]
        [Comment("抵達站")]
        [Column("end_station")]
        public string EndStation { get; set; }

        [StringLength(500)]
        [Comment("備註")]
        [Column("remarks")]
        public string Remarks { get; set; }

        [StringLength(100)]
        [Comment("流水號/編碼")]
        [Column("sereial_no_a")]
        public string SereialNoA { get; set; }

        [StringLength(100)]
        [Comment("流水號/編碼")]
        [Column("sereial_no_b")]
        public string SereialNoB { get; set; }

        [StringLength(100)]
        [Comment("流水號/編碼")]
        [Column("sereial_no_c")]
        public string SereialNoC { get; set; }

        [Comment("金額")]
        [Column("amount_a", TypeName = "numeric(18,2)")]
        public decimal? AmountA { get; set; }

        [Comment("金額")]
        [Column("amount_b", TypeName = "numeric(18,2)")]
        public decimal? AmountB { get; set; }

        [Comment("金額")]
        [Column("amount_c", TypeName = "numeric(18,2)")]
        public decimal? AmountC { get; set; }

        [StringLength(10)]
        [Comment("發票區域")]
        [Column("region")]
        public string Region { get; set; }

        [StringLength(50)]
        [Comment("發票類型代碼")]
        [Column("invoice_type_code")]
        public string InvoiceTypeCode { get; set; }

    }
}