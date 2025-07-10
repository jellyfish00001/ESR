using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities.Payment
{
    [Table("cash_payment_head")]
    [Index(nameof(SysNo), Name = "sysno_idx")]
    public class CashPaymentHead : BaseEntity
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
        [StringLength(50)]
        [Column("bank")]
        [Comment("银行名称")]
        public string Bank { get; set; }

        [Column("payment_date")]
        [Comment("付款日期")]
        public DateTime? PaymentDate { get; set; }

        [Column("amt", TypeName = "decimal(18, 2)")]
        [Comment("金额")]
        public decimal Amt { get; set; }

        [StringLength(100)]
        [Column("identification")]
        [Comment("Payment Run會計")]
        public string Identification { get; set; }

        [StringLength(50)]
        [Column("payment_status")]
        [Comment("Payment状态")]
        public string PaymentStatus { get; set; }

        [StringLength(50)]
        [Column("payment_docno")]
        [Comment("付款传票号")]
        public string PaymentDocNo { get; set; }

        [StringLength(1)]
        [Column("stutus")]
        [Comment("状态")]
        public string Status { get; set; }

        [StringLength(20)]
        [Comment("应签核人工号")]
        [Column("assigned_emplid")]
        public string AssignedEmplid { get; set; }

        [StringLength(20)]
        [Comment("签核人名字")]
        [Column("assigned_name")]
        public string AssignedName { get; set; }

        [Column("approver_emplid")]
        [StringLength(100)]
        [Comment("實際簽核人工號")]
        public string ApproverEmplid { get; set; }

        [Column("approver_name")]
        [Comment("實際簽核人姓名")]
        [StringLength(100)]
        public string ApproverName { get; set; }

        [Column("approver_date")]
        [Comment("签核时间")]
        public DateTime? ApproverDate { get; set; }

        [Column("approver_remark")]
        [StringLength(200)]
        [Comment("签核意见")]
        public DateTime? ApproverRemark { get; set; }
    }
}
