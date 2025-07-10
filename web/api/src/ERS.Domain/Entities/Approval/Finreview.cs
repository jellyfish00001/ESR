using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
namespace ERS.Entities
{
    [Table("finreview")]
    public class Finreview : BaseEntity
    {        
        [StringLength(20)]
        public string company_code { get; set; }
        [Required]
        [StringLength(20)]
        public string plant { get; set; } 
        [StringLength(100)]
        public string rv1 { get; set; }
        [StringLength(100)]
        public string rv2 { get; set; }
        [StringLength(100)]
        public string rv3 { get; set; }
        public int item { get; set; }
        [StringLength(20)]
        [Comment("单据类型，0：报销预支；1：薪资请款")]
        public int category { get; set; }
    }
}
