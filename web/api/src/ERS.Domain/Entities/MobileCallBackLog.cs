using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities;
[Table("mobile_call_back_log")]
public class MobileCallBackLog : BaseEntity
{
    [Required]
    [StringLength(20)]
    public string rno { get; set; }
    [StringLength(20)]
    public string messageid { get; set; }
    [StringLength(20)]
    public string emplid { get; set; }
    [StringLength(20)]
    public string signresult { get; set; }
}
