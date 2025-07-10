using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    /// <summary>
    /// 冲账
    /// </summary>
    [Table("charge_against")]
    public class ChargeAgainst : BaseEntity
    {
        [StringLength(20)]
        [Comment("预支金单号")]
        public string rno { get; set; }
        [StringLength(20)]
        [Comment("报销单号")]
        public string rerno { get; set; }
    }
}
