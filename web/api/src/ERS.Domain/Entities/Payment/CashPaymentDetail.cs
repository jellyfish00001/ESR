using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities.Payment
{
    [Table("cash_payment_detail")]
    [Index(nameof(SysNo), Name = "sysno_idx")]
    public class CashPaymentDetail : BaseEntity
    {
        [Required]
        [StringLength(20)]
        [Column("sysno")]
        [Comment("付款流水号")]
        public string SysNo { get; set; }

        [Required]
        [Comment("sysitem")]
        [Column("sysitem")]
        public int SysItem { get; set; }

        [Required]
        [StringLength(20)]
        [Column("seq")]
        [Comment("seq")]
        public string Seq { get; set; }

        [Required]
        [StringLength(20)]
        [Column("deptid")]
        [Comment("部门代码")]
        public string DeptId { get; set; }

        [Required]
        [StringLength(20)]
        [Column("payeeid")]
        [Comment("收款人")]
        public string PayeeId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("scname")]
        [Comment("收款人名字")]
        public string ScName { get; set; }

        [Required]
        [StringLength(200)]
        [Column("payeeaccount")]
        [Comment("银行卡号")]
        public string PayeeAccount { get; set; }

        [Required]
        [StringLength(50)]
        [Column("bank")]
        [Comment("银行名称")]
        public string Bank { get; set; }

        [Column("amt", TypeName = "decimal(18, 2)")]
        [Comment("金额")]
        public decimal Amt { get; set; }

        [StringLength(50)]
        [Column("usage")]
        [Comment("类别")]
        public string Usage { get; set; }

        [StringLength(50)]
        [Column("docno")]
        [Comment("传票号")]
        public string DocNo { get; set; }

        [Column("postdate")]
        [Comment("PostDate")]
        public DateTime PostDate { get; set; }

        [StringLength(10)]
        [Column("formcode")]
        [Comment("单据类型代码")]
        public string FormCode { get; set; }

        [StringLength(20)]
        [Column("rno")]
        [Comment("申请单号")]
        public string Rno { get; set; }

        [StringLength(50)]
        [Column("contnt")]
        [Comment("摘要")]
        public string Contnt { get; set; }

        [StringLength(10)]
        [Column("basecurr")]
        [Comment("币别")]
        public string BaseCurr { get; set; }

        [StringLength(1)]
        [Column("stat")]
        [Comment("状态")]
        public string Stat { get; set; }

        [Column("payment_date")]
        [Comment("付款日期")]
        public DateTime? PaymentDate { get; set; }

        [StringLength(100)]
        [Column("identification")]
        [Comment("会计")]
        public string Identification { get; set; }

        [StringLength(100)]
        [Column("paymentmethod")]
        [Comment("付款方式")]
        public string PaymentMethod { get; set; }

        [StringLength(1)]
        [Column("temsign")]
        [Comment("temsign")]
        public string TemSign { get; set; }

        [StringLength(50)]
        [Column("paymentid")]
        [Comment("付款人")]
        public string PaymentId { get; set; }

        [StringLength(50)]
        [Column("paymentcurr")]
        [Comment("付款币别")]
        public string PaymentCurr { get; set; }
    }
}
