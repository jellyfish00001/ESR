using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [Table("bd_form")]
    public class BDForm : BaseEntity
    {
        [Required]
        [StringLength(20)]
        [Column("form_code")]
        [Comment("单据Code")]
        public string FormCode { get; set; }
        [Required]
        [StringLength(20)]
        [Column("form_name")]
        [Comment("单据名称")]
        public string FormName { get; set; }
        [StringLength(50)]
        [Column("application_menu_key")]
        [Comment("填单页面MenuKey")]
        public string ApplicationMenuKey { get; set; }
        [StringLength(50)]
        [Column("sign_menu_key")]
        [Comment("签核页面MenuKey")]
        public string SignMenuKey { get; set; }
        [StringLength(20)]
        [Column("no_format")]
        [Comment("单号格式")]
        public string NoFormat { get; set; }
        [StringLength(1)]
        [Column("valid")]
        [Comment("是否启用")]
        public string Valid { get; set; }
    }
}
