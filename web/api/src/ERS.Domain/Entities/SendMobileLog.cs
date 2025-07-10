using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities;
[Table("send_mobile_log")]
public class SendMobileLog : BaseEntity
{
    [Required]
    [StringLength(20)]
    public string rno { get; set; }
    [StringLength(10)]
    public string formcode { get; set; }
    [StringLength(20)]
    public decimal seq { get; set; }
    [StringLength(20)]
    public string step { get; set; }
    [StringLength(20)]
    public string emplid { get; set; }
    [StringLength(20)]
    public string deptid { get; set; }
    [StringLength(10)]
    public string signstatus { get; set; }
    [StringLength(20)]
    public string messageid { get; set; }
    [StringLength(20)]
    public string sendreturncode { get; set; }
    [StringLength(100)]
    public string sendreturnmessage { get; set; }
    [StringLength(10)]
    public string sendstatus { get; set; }
    [StringLength(20)]
    public string recallcode { get; set; }
    [StringLength(100)]
    public string recallmessage { get; set; }
    [StringLength(10)]
    public string recallstatus { get; set; }
}
