using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [NotMapped]
    public class Queryposting : BaseEntity
    {
        public int item { get; set; }
        [StringLength(20)]
        public string rno { get; set; }
        [StringLength(20)]
        public string cemplid { get; set; }
        [StringLength(20)]
        public string formcode { get; set; }
        [StringLength(20)]
        public string formname { get; set; }
        [StringLength(200)]
        public string cname { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal actamt { get; set; }
        [StringLength(20)]
        public string currency { get; set; }
        [StringLength(20)]
        public string bank { get; set; }
        [StringLength(20)]
        public string apid { get; set; }
        public DateTime? cdate { get; set; }
    }
}