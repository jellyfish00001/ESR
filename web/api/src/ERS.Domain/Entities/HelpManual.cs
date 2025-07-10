using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [Table("help_manual")]
    public class HelpManual : BaseEntity
    {
        [StringLength(200)]
        public override string company { get; set; }
        [StringLength(200)]
        public string name { get; set; }
        [StringLength(200)]
        public string path { get; set; }
        [StringLength(500)]
        public string url { get; set; }
    }
}
