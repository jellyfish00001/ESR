using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities;
[Table("appconfig")]
public class AppConfig : BaseEntity
{
    [Required]
    [StringLength(50)]
    public string key { get; set; }
    [StringLength(1000)]
    public string value { get; set; }
    [StringLength(100)]
    public string remark { get; set; }
}
