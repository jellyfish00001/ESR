using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [Table("bd_paper_sign")]
    public class BDPaperSign : BaseEntity
    {
        [StringLength(20)]
        public string company_code { get; set; }
        [StringLength(20)]
        public string plant { get; set; }
        [StringLength(20)]
        public string emplid { get; set; }
    }
}
