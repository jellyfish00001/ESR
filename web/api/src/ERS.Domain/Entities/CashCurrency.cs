using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [Table("cash_currency")]
    public class CashCurrency : BaseEntity
    {
        [Required]
        [StringLength(20)]
        public string currency { get; set; }
        [StringLength(20)]
        public string currency_desc { get; set; }
        public int seq { get; set; }
    }
}
