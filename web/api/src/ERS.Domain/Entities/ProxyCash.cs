using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    /// <summary>
    /// 报销代填人
    /// </summary>
    [Table("proxy_cash")]
    public class ProxyCash : BaseEntity
    {
        [StringLength(20)]
        [Required]
        [Comment("报销人工号")]
        public string aemplid { get; set; }
        [StringLength(20)]
        [Required]
        [Comment("代报销人工号")]
        public string remplid { get; set; }
    }
}
