using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [Table("cash_carryhead")]
    [Index(nameof(carryno), Name = "carryno_idx")]
    public class CashCarryhead : BaseEntity
    {
        [Required]
        [StringLength(20)]
        public string carryno { get; set; }
        public DateTime? postdate { get; set; }
        [StringLength(20)]
        [Comment("Accountant")]
        public string acctant { get; set; }
        [Required]
        [StringLength(20)]
        public string companycode { get; set; }
        [StringLength(50)]
        [Comment("Bank name")]
        public string bank { get; set; }
        [Required]
        [StringLength(1)]
        [Comment("Status; Y: available, N: Not available")]
        public string stat { get; set; }
    }
}
