using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [Table("e_form_alist_history")]
    public class EFormAlistHistory : BaseEntity
    {
        [Required]
        [StringLength(20)]
        public string formcode { get; set; }
        [Required]
        [StringLength(20)]
        public string rno { get; set; }
        [Required]
        [Column(TypeName = "decimal(8, 2)")]
        public decimal step { get; set; }
        [Required]
        [StringLength(20)]
        public string cemplid { get; set; }
        [Required]
        [StringLength(1)]
        public string status { get; set; }
        [StringLength(20)]
        public string deptid { get; set; }
        [StringLength(100)]
        public string deptn { get; set; }
        [StringLength(100)]
        public string stepname { get; set; }
        public int seq { get; set; }
    }
}
