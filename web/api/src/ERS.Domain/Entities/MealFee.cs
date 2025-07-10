using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [Table("mealfee")]
    public class MealFee : BaseEntity
    {
        [StringLength(100)]
        public string area { get; set; }
        [StringLength(100)]
        public string city { get; set; }
        [StringLength(20)]
        public string gotime { get; set; }
        [StringLength(20)]
        public string gotime2 { get; set; }
        [StringLength(1)]
        public string gotimestate { get; set; }
        [StringLength(20)]
        public string backtime { get; set; }
        [StringLength(20)]
        public string backtime2 { get; set; }
        [StringLength(1)]
        public string backtimestate { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal amount { get; set; }
        [StringLength(20)]
        public string currency { get; set; }
    }
}
