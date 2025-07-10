using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [Table("doa_type")]
    public class DoaType : BaseEntity
    {
        [StringLength(20)]
        public string dtype { get; set; }
        [StringLength(100)]
        public string dtypedesc { get; set; }
    }
}
