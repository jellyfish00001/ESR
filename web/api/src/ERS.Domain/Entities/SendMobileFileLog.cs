using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities;
[Table("send_mobile_file_log")]
public class SendMobileFileLog : BaseEntity
{
    [Required]
    [StringLength(20)]
    public string rno { get; set; }
    [StringLength(20)]
    public string messageid { get; set; }
    [StringLength(100)]
    public string fileId { get; set; }
    [StringLength(100)]
    public string filename { get; set; }
    [StringLength(20)]
    public string filesize { get; set; }
}
