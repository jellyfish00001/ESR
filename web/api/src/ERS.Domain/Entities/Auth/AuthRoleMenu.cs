using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [Table("auth_role_menu")]
    public class AuthRoleMenu : BaseEntity
    {
        [Comment("角色key")]
        [StringLength(30)]
        public string rolekey { get; set; }
        [Comment("菜单key")]
        [StringLength(100)]
        public string menukey { get; set; }
    }
}
