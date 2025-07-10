using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities;
namespace ERS.Entities
{
    [Table("bd_treelevel")]
    public class BDTreelevel : Entity<Guid>
    {
        [Required]
        [StringLength(100)]
        [Comment("簽核層級名稱(英文)")]
        public string levelname { get; set; }

        [Required]
        [StringLength(100)]
        [Comment("簽核層級名稱(繁中)")]
        public string leveltwname { get; set; }

        [Required]
        [StringLength(100)]
        [Comment("簽核層級名稱(簡中)")]
        public string levelcnname { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(2, 0)")]
        [Comment("簽核層級編號")]
        public decimal levelnum { get; set; }        
    }
}
