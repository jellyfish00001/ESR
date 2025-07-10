using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [Table("compant_user")]
    public class CompanyUser : BaseEntity
    {
        [Required]
        [StringLength(20)]
        [Comment("工号")]
        public string emplid { get; set; }

        [Required]
        [StringLength(20)]
        [Comment("公司别")]
        public  override string  company { get; set; }

    }
}
