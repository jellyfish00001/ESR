using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [Table("bd_car")]
    public class BDCar : BaseEntity
    {
        public int type { get; set; }
        [Comment("类型名称")]
        [StringLength(100)]
        public string name { get; set; }
        [Comment("补助金额（每公里）")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal amount { get; set; }
    }
}
