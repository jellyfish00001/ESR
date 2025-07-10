using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [Table("auth_user_company")]
    public class AuthUserCompany : BaseEntity
    {
        [Comment("用户key")]
        [StringLength(30)]
        public string userkey { get; set; }
        [Comment("适用模块,application申请单,finance财务权限用")]
        [StringLength(30)]
        public string module { get; set; }
    }
}
