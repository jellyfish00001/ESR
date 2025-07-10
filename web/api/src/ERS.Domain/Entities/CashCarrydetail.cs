using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [Table("cash_carrydetail")]
    [Index(nameof(carryno), Name = "carryno_idx")]
    public class CashCarrydetail : BaseEntity, ICloneable
    {
        [Required]
        [StringLength(20)]
        public string carryno { get; set; }
        public int carritem { get; set; }
        public int seq { get; set; }
        [Comment("Document date,費用發生日期")]
        public DateTime docdate { get; set; }
        [Comment("Posting date, 生成入帳清單日期")]
        public DateTime postdate { get; set; }
        [Required]
        [StringLength(20)]
        [Comment("SAP company code,公司別")]
        public string companysap { get; set; }
        [Required]
        [StringLength(20)]
        [Comment("base currency, 本位幣")]
        public string basecurr { get; set; }
        [Column(TypeName = "decimal(10, 5)")]
        [Comment("exchange rate, 默認為1")]
        public decimal rate { get; set; } = 1;
        [StringLength(50)]
        [Comment("Reference 3, ERS申請單號")]
        public string @ref { get; set; }
        [StringLength(20)]
        [Comment("Document Header Text")]
        public string dochead { get; set; }
        [Required]
        [StringLength(20)]
        [Comment("Document Type, 默認為KR")]
        public string doctyp { get; set; } = "KR";
        [Required]
        [StringLength(20)]
        [Comment("Posting key, vendor貸方默認為31，洽談借方默認為40，貸方默認為50")]
        public string postkey { get; set; }
        [Required]
        [StringLength(20)]
        [Comment("Acct. code (GL Acct), 默認vendor為Z800003")]
        public string acctcode { get; set; }
        [StringLength(20)]
        [Comment("Special G/L Indicator")]
        public string specgl { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        [Comment("Actual pay amount,Amount In Document Currency (include tax amount), 實際支付金額")]
        public decimal actamt1 { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        [Comment("Amount In Local Currency (include tax amount), 實際支付金額")]
        public decimal actamt2 { get; set; }
        [StringLength(20)]
        [Comment("Payment Term")]
        public string payterm { get; set; }
        [StringLength(20)]
        [Comment("Pay type,Payment Method, 默認為T")]
        public string paytyp { get; set; } = "T";
        [Comment("Base line date, 生成入帳清單日期")]
        public DateTime baslindate { get; set; }
        [StringLength(20)]
        [Comment("Tax code")]
        public string txtcode { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        [Comment("Tax Base Amount")]
        public decimal? taxamt1 { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        [Comment("LC Tax Base Amount")]
        public decimal? taxamt2 { get; set; }
        [StringLength(20)]
        [Comment("Withholding tax type")]
        public string wtaxtyp { get; set; }
        [StringLength(20)]
        [Comment("Withholding tax code")]
        public string wtaxcode { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        [Comment("Withholding tax base amount")]
        public decimal? wtaxamt1 { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        [Comment("Withholding tax amount")]
        public decimal? wtaxamt2 { get; set; }
        [StringLength(20)]
        [Comment("Cost center, ZS+掛賬部門")]
        public string costcenter { get; set; }
        [StringLength(20)]
        [Comment("Order")]
        public string order { get; set; }
        [Required]
        [StringLength(500)]
        [Comment("Line Text, 摘要")]
        public string linetext { get; set; }
        [StringLength(50)]
        [Comment("Assignment number")]
        public string asinmnt { get; set; }
        [StringLength(20)]
        [Comment("Profit Center")]
        public string proficenter1 { get; set; }
        [StringLength(20)]
        [Comment("Partner Profit Center")]
        public string proficenter2 { get; set; }
        [StringLength(20)]
        [Comment("Customer Code (ie. Bill-to Party)")]
        public string custercode { get; set; }
        [StringLength(20)]
        public string plant { get; set; }
        [StringLength(20)]
        [Comment("Business Type")]
        public string busityp { get; set; }
        [StringLength(20)]
        [Comment("End customer")]
        public string ecuster { get; set; }
        [StringLength(20)]
        [Comment("Material Division")]
        public string mtrldiv { get; set; }
        [StringLength(20)]
        [Comment("Sales Division")]
        public string salsdiv { get; set; }
        [StringLength(20)]
        [Required]
        [Comment("Reference 1, 收款人工號")]
        public string ref1 { get; set; }
        [StringLength(50)]
        [Comment("Reference 2")]
        public string ref2 { get; set; }
        [StringLength(50)]
        public string ref3 { get; set; }
        [StringLength(10)]
        [Comment("Form code")]
        public string formcode { get; set; }
        [StringLength(20)]
        [Required]
        public string rno { get; set; }
        public int pstitem { get; set; }
        [StringLength(1)]
        [Comment("Status; Y: available, N: Not available")]
        public string stat { get; set; } = "Y";
        [StringLength(1)]
        [Comment("'Y': Edit , 'N': can not Edit")]
        public string rnostatus { get; set; } = "N";
        [StringLength(20)]
        [Comment("Excel Line Item No")]
        public string excelline { get; set; }
        [StringLength(20)]
        [Comment("Trading Partner (at header level)")]
        public string tradingpartner { get; set; }
        [StringLength(20)]
        [Comment("Accountant")]
        public string acctant { get; set; }
        [StringLength(50)]
        [Comment("Bank name")]
        public string bank { get; set; }
        [StringLength(50)]
        [Comment("UnifyCode")]
        public string unifycode { get; set; }
        [StringLength(50)]
        [Comment("certificate")]
        public string certificate { get; set; }
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
