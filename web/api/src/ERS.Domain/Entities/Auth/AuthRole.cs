using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [Table("auth_role")]
    public class AuthRole : BaseEntity
    {
        [Comment("角色key")]
        [StringLength(30)]
        public string rolekey { get; set; }
        [Comment("角色名称")]
        [StringLength(30)]
        public string rolename { get; set; }
    }
}
