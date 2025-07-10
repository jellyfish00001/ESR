using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ERS.Entities
{
    [Table("cash_uber_detail")]
    [Index(nameof(Rno), nameof(FormCode), nameof(Item), Name = "cash_uber_detail_pk")]
    public class CashUberDetail : BaseEntity
    {
        [Column("form_code")]
        [StringLength(10)]
        public string FormCode { get; set; }

        [Column("rno")]
        [StringLength(20)]
        public string Rno { get; set; }

        [Column("item")]
        [StringLength(100)]
        public string Item { get; set; }

        [Column("startdate")]
        public DateTime? StartDate { get; set; }

        [Column("destination")]
        [StringLength(100)]
        public string Destination { get; set; }

        [Column("origin")]
        [StringLength(100)]
        public string Origin { get; set; }

        [Column("status")]
        [StringLength(1)]
        public string Status { get; set; }

        [Column("amount")]
        public decimal? Amount { get; set; }

        [Column("reason")]
        [StringLength(500)]
        public string Reason { get; set; }

        [Column("expcode")]
        [StringLength(50)]
        public string ExpCode { get; set; }

        [Column("emplid")]
        [StringLength(20)]
        public string Emplid { get; set; }

        [Column("name")]
        [StringLength(50)]
        public string Name { get; set; }

        [Column("deptid")]
        [StringLength(10)]
        public string DeptId { get; set; }
    }
}