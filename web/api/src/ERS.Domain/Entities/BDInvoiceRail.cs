using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    //发票字轨
    [Table("bd_invoice_rail")]
    public class BDInvoiceRail : BaseEntity
    {
        [NotMapped]//不映射到数据库
        public override string company { get; set; }
        [StringLength(30)]
        [Comment("期")]
        public string qi { get; set; }
        [Required]
        [StringLength(10)]
        [Comment("发票字轨")]
        public string invoicerail { get; set; }
        [Column(TypeName = "decimal")]
        [Comment("年份")]
        public decimal year { get; set; }
        [Column(TypeName = "decimal")]
        [Comment("月份")]
        public decimal month { get; set; }

        [Column(TypeName = "decimal")]
        [Comment("格式代码")]
        public decimal formatcode { get; set; }
        [StringLength(30)]
        [Comment("发票类型")]
        public string invoicetype { get; set; }
    }
}
