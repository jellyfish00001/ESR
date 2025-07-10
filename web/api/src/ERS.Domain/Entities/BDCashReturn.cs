using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [Table("bd_cash_return")]
    [Index(nameof(rno), Name = "rno_idx")]
    public class BDCashReturn : BaseEntity
    {
        [Required]
        [StringLength(50)]
        public string rno { get; set; }
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal amount { get; set; }
    }
}
