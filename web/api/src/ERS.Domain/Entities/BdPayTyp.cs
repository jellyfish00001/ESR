using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [Table("bdpaytyp")]
    public class BdPayTyp : BaseEntity
    {
        [Required]
        [StringLength(20)]
        public string paytyp { get; set; }
        [Required]
        [StringLength(50)]
        public string payname { get; set; }
        public int seq { get; set; }
        [Required]
        [StringLength(1)]
        [Comment("Status; Y: available, N: Not available")]
        public string stat { get; set; }
    }
}
