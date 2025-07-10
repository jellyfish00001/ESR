using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
namespace ERS.Entities
{
    [Table("cash_account_ps")]
    public class CashAccountPs : BaseEntity
    {
        [Required]
        [StringLength(20)]
        public string emplid { get; set; }
        [Required]
        [StringLength(200)]
        public string account { get; set; }
        [Comment("户名")]
        [StringLength(240)]
        public string accountname { get; set; }
        [StringLength(20)]
        public string uuser { get; set; }
        public DateTime? udate { get; set; }
        [StringLength(50)]
        public string bank { get; set; }
        public DateTime? effdt { get; set; }
    }
}
