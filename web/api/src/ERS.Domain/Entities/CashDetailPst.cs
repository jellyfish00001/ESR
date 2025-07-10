using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [Table("cash_detail_pst")]
    [Index(nameof(rno), Name = "rno_idx")]
    public class CashDetailPst : BaseEntity
    {
        [Required]
        [StringLength(20)]
        public string formcode { get; set; }
        [Required]
        [StringLength(20)]
        public string rno { get; set; }
        public int detailitem { get; set; }
        [Required]
        [StringLength(20)]
        [Comment("Base currency (Local curr.)")]
        public string basecurr { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        [Comment("Base amount (Local)")]
        public decimal baseamt { get; set; }
        [StringLength(20)]
        [Comment("Posting key")]
        public string postkey { get; set; }
        [StringLength(20)]
        [Comment("Acct. code (GL Acct)")]
        public string acctcode { get; set; }
        [StringLength(20)]
        [Comment("Tax code")]
        public string txtcode { get; set; }
        [StringLength(20)]
        [Comment("Cost center")]
        public string costcenter { get; set; }
        [StringLength(100)]
        [Comment("Line Text")]
        public string lintext { get; set; }
        [StringLength(50)]
        [Comment("Assignment")]
        public string asinmnt { get; set; }
        public int pstitem { get; set; }
        [StringLength(1)]
        [Comment("Status; Y: available, N: Not available")]
        public string stat { get; set; }
        [Comment("Document date")]
        public DateTime? ddate { get; set; }
        [StringLength(20)]
        [Comment("payee employee id")]
        public string payeeid { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        [Comment(" tax base amount")]
        public decimal? taxbase { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        [Comment(" LC tax base amount")]
        public decimal? lctaxbase { get; set; }
        [StringLength(50)]
        public string invoice { get; set; }
        [StringLength(50)]
        public string rdate { get; set; }
        [StringLength(50)]
        public string unifycode { get; set; }
        [StringLength(100)]
        public string certificate { get; set; }
        [StringLength(20)]
        [Comment("Reference UnifyCode")]
        public string ref1 { get; set; }
        [StringLength(20)]
        [Comment("Expense category code")]
        public string expcode { get; set; }
    }
}
