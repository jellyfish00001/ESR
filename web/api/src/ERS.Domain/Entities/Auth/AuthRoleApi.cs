using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [Table("auth_role_api")]
    public class AuthRoleApi : BaseEntity
    {
        [Comment("角色key")]
        [StringLength(30)]
        public string rolekey { get; set; }
        [Comment("apikey")]
        [StringLength(100)]
        public string apikey { get; set; }
    }
}
