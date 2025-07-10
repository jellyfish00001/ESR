using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [Table("bd_signlevel")]
    public class BDSignlevel : BaseEntity
    {
        [Required]
        [StringLength(5)]
        [Comment("呈核項目")]
        public string item { get; set; }

        [Required]
        [StringLength(100)]
        [Comment("簽核層級")]
        public string signlevel { get; set; }

        [Required]
        [Column(TypeName = "decimal(20, 4)")]
        [Comment("簽核金額")]
        public decimal money { get; set; }
        
        [Required]
        [StringLength(100)]
        [Comment("幣別")]
        public string currency { get; set; }
    }
}
