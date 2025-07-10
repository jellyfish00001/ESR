using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [Table("doa")]
    public class Doa : BaseEntity
    {
        [StringLength(20)]
        [Comment("DOA type")]
        public string dtype { get; set; }
        [Comment("org tree level")]
        public int treeLevelnum { get; set; }
        [StringLength(20)]
        public string currency { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        [Comment("Start amount")]
        public decimal? samt { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        [Comment("End amount")]
        public decimal? eamt { get; set; }
        [Comment("effective date")]
        public DateTime effdate { get; set; }
    }
}
