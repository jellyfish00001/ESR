using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ERS.Entities
{
    [Table("cash_uber_head")]
    [Index(nameof(Rno), nameof(FormCode), Name = "cash_uber_head_pk")]
    public class CashUberHead : BaseEntity
    {
        [StringLength(20)]
        [Column("emplid")]
        public string Emplid { get; set; }

        [StringLength(20)]
        [Column("rno")]
        public string Rno { get; set; }

        [StringLength(20)]
        [Column("form_code")]
        public string FormCode { get; set; }

        [StringLength(50)]
        [Column("name")]
        public string Name { get; set; }

        [StringLength(100)]
        [Column("projectcode")]
        public string ProjectCode { get; set; }

        [StringLength(50)]
        [Column("business_trip_no")]
        public string BusinessTripNo { get; set; }

        [StringLength(20)]
        [Column("program")]
        public string Program { get; set; }

        [Comment("状态")]
        [StringLength(50)]
        [Column("status")]
        public string? Status { get; set; }
    }
}