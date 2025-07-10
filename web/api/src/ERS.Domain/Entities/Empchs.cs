using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [Table("empchs")]
    public class Empchs : BaseEntity
    {
        [StringLength(20)]
        public string emplid { get; set; }
        [StringLength(100)]
        public string scname { get; set; }
        [StringLength(1)]
        public string stat { get; set; }
    }
}
