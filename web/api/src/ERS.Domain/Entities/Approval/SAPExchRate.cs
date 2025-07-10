using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
namespace ERS.Entities
{
    [Table("sap_exch_rate")]
    public class SAPExchRate : BaseEntity
    {
        [StringLength(20)]
        [Required]
        public string ccurtype { get; set; }
        public DateTime ccurdate { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal ratiofrom { get; set; }
        [StringLength(20)]
        [Required]
        public string ccurfrom { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal ratioto { get; set; }
        [StringLength(20)]
        [Required]
        public string ccurto { get; set; }
        [Column(TypeName = "decimal(10, 5)")]
        public decimal ccurrate { get; set; }
        public DateTime timestamp { get; set; }
        [Comment("有效起始日")]
        public DateTime? date_fm { get; set; }
        [Comment("有效终止日")]
        public DateTime? date_to { get; set; }
    }
}
