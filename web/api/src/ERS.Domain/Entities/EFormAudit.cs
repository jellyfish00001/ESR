using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [Table("e_form_audit")]
    public class EFormAudit : BaseEntity
    {
        [Required]
        [StringLength(20)]
        public string formcode { get; set; }
        [Required]
        [StringLength(20)]
        public string emplid { get; set; }
        [Required]
        [StringLength(20)]
        public string deptid { get; set; }
        [Required]
        [StringLength(20)]
        public string auditid { get; set; }
        public DateTime sdate { get; set; }
        public DateTime edate { get; set; }
    }
}
