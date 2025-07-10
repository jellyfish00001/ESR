using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [Table("auth_user_role")]
    public class AuthUserRole : BaseEntity
    {
        [Comment("用户key")]
        [StringLength(30)]
        public string userkey { get; set; }
        [Comment("角色key")]
        [StringLength(30)]
        public string rolekey { get; set; }
    }
}
