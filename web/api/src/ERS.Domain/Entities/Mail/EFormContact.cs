using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [Table("e_form_contact")]
    public class EFormContact : BaseEntity
    {
        [StringLength(50)]
        public string location { get; set; }
        [Required]
        [StringLength(100)]
        public string chinesecontact { get; set; }
        [Required]
        [StringLength(100)]
        public string englisthcontact { get; set; }
    }
}
