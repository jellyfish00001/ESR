using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [Table("auth_menu")]
    public class AuthMenu : BaseEntity
    {
        [Comment("菜单key")]
        [StringLength(100)]
        public string menukey { get; set; }
   
        [Comment("菜单名称")]
        [StringLength(100)]
        public string menuname { get; set; }
        [Comment("是否为公共菜单")]
        public bool ispublic { get; set; }
    }
}
