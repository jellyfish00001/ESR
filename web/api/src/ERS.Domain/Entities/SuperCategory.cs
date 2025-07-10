using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
namespace ERS.Entities
{
    [Table("supercategory")]
    public class SuperCategory : BaseEntity
    {
        public int categoryid { get; set; }
        [StringLength(100)]
        public string categoryname { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal budget { get; set; }
        [StringLength(1)]
        public string isreimbursable { get; set; }
        [StringLength(10)]
        [Comment("in: 园区内；out: 园区外")]
        public string area { get; set; }
    }
}
