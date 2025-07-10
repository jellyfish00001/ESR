using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [Table("approval_paper")]
    [Index(nameof(rno), Name = "rno_idx")]
    public class ApprovalPaper : BaseEntity
    {
        [Required]
        [StringLength(20)]
        public string rno { get; set; }
        [StringLength(1)]
        [Comment("A（同意）R（拒签）C（取消）P（待签）")]
        public string status { get; set; }
        [StringLength(20)]
        [Comment("签核人")]
        public string emplid { get; set; }
        [StringLength(20)]
        [Comment("实际签核人")]
        public string aemplid { get; set; }
        public string formcode { get; set; }
    }
}
