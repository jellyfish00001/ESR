using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [Table("bd_vender")]
    [Index(nameof(UnifyCode), Name = "bd_vender_idx_unifyCode")]
    public class BDVender : BaseEntity
    {
        [Required]
        [StringLength(100)]
        [Comment("统一编号")]
        [Column("unify_code")]
        public string UnifyCode { get; set; }

        [Required]
        [StringLength(100)]
        [Comment("供应商编号")]
        [Column("vender_code")]
        public string VenderCode { get; set; }

        [Required]
        [StringLength(100)]
        [Comment("供应商名称")]
        [Column("vender_name")]
        public string VenderName { get; set; }
    }
}