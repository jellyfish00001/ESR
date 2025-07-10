using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
namespace ERS.Entities.Bank
{
    [Table("combank")]
    public class ComBank : BaseEntity
    {
        [Comment("銀行名稱")]
        [StringLength(50)]
        public string banid { get; set; }
        [StringLength(20)]
        public string vendercode { get; set; }
        [Comment("銀行簡稱")]
        [StringLength(20)]
        public string bank_abbr { get; set; }
    }
}