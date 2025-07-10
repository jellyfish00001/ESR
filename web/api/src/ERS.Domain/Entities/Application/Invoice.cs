using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
namespace ERS.Entities
{
    [Table("invoice")]
    [Index(nameof(rno), Name = "rno_idx")]
    public class Invoice : BaseEntity
    {
        [Required]
        [StringLength(20)]
        public string rno { get; set; }
        [Required]
        [Comment("明细的item")]
        public int seq { get; set; }
        [Required]
        [Comment("明细item中的附件item")]
        public int item { get; set; }
        //发票代码
        [StringLength(20)]
        public string invcode { get; set; }
        //发票号码
        [StringLength(200)]
        public string invno { get; set; }
        //开票金额
        [Column(TypeName = "decimal(18, 2)")]
        public decimal amount { get; set; }
        //开票日期
        public DateTime? invdate { get; set; }
        //税额
         [Column(TypeName = "decimal(18, 2)")]
        public decimal taxamount { get; set; }
        //价税合计金额
        [Column(TypeName = "decimal(18, 2)")]
        public decimal oamount { get; set; }
        //发票状态
        [StringLength(20)]
        public string  invstat { get; set; }
        //异常报销金额
         [Column(TypeName = "decimal(18, 2)")]
        public decimal abnormalamount { get; set; }
         //税金损失
         [Column(TypeName = "decimal(18, 2)")]
        public decimal taxloss { get; set; }
        //币别
         [StringLength(20)]
        public string curr { get; set; }
        //承担方
        [StringLength(20)]
        public string undertaker { get; set; }
        [StringLength(100)]
        [Comment("承担人")]
        public string underwriter { get; set; }
        [StringLength(200)]
        public string reason { get; set; }
        [StringLength(1)]
        [Comment("是否异常报销上传的（Y/N）")]
        public string abnormal { get; set; }
        [StringLength(300)]
        [Comment("异常信息")]
        public string abnormalmsg { get; set; }
        [StringLength(200)]
        [Comment("异常原因")]
        public string abnormalreason { get; set; }
        [StringLength(200)]
        [Comment("发票异常原因")]
        public string invabnormalreason { get; set; }
        [StringLength(50)]
        [Comment("发票类型")]
        public string invtype { get; set; }
        [Comment("票夹发票id")]
        public Guid? invoiceid { get; set; }
        public void SetRno(string rno) => this.rno = rno;
        [StringLength(50)]
        [Comment("销售方税号")]
        [Column("seller_tax_id")]
        public string sellerTaxId { get; set; }
        [Comment("票據來源")]
        [StringLength(30)]
        public string source { get; set; }
    }
}