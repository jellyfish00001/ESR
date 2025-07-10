using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [Table("mailtemple")]
    public class Mailtemplate : BaseEntity
    {
        public int mailtype { get; set; }
        [StringLength(500)]
        [Required]
        public string subject { get; set; }
        [StringLength(4000)]
        [Required]
        public string mailmsg { get; set; }
        public int seq { get; set; }
        [StringLength(20)]
        public string apid { get; set; }
        [StringLength(200)]
        public string helpdesk { get; set; }
    }
}
