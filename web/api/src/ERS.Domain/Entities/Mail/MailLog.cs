using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [Table("maillog")]
    public class MailLog : BaseEntity
    {
        [StringLength(20)]
        public string formcode { get; set; }
        [StringLength(20)]
        public string emplid { get; set; }
        [StringLength(1)]
        public string status { get; set; }
        [StringLength(100)]
        public string mail { get; set; }
        [StringLength(20)]
        public string rno { get; set; }
    }
}
