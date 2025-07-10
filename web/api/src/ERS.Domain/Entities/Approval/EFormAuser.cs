using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [Table("e_form_auser")]
    public class EFormAuser : BaseEntity
    {
        [Required]
        [StringLength(20)]
        public string formcode { get; set; }
        [Required]
        [StringLength(20)]
        public string rno { get; set; }
        [Required]
        [StringLength(20)]
        public string cemplid { get; set; }
        [Column(TypeName = "decimal(8, 2)")]
        public decimal? seq { get; set; }
        [StringLength(1)]
        public string used { get; set; } = "N";
    }
}
