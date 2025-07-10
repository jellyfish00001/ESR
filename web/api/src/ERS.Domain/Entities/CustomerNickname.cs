using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [Table("customer_nickname")]
    public class CustomerNickname : BaseEntity
    {
        [StringLength(100)]
        [Comment("客戶暱稱")]
        public string nickname { get; set; }
        [StringLength(100)]
        [Comment("客戶名稱")]
        public string name { get; set; }
        [StringLength(1)]
        [Comment("是否帶暱稱(Y/N)")]
        public string iscarry { get; set; }
    }
}
