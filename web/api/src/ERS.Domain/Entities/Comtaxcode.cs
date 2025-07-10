using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [Table("comtaxcode")]
    public class Comtaxcode : BaseEntity
    {
        [StringLength(20)]
        public string taxcode { get; set; }
        [StringLength(20)]
        [Comment("稅金的Taxcode")]
        public string taxexpense { get; set; }
        [StringLength(50)]
        [Comment("稅金的科目")]
        public string taxcourse { get; set; }
        [StringLength(20)]
        public string paymentmethod { get; set; }
    }
}
