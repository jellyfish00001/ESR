using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [Table("compant_area_info")]
    public class CompanyAreaInfo : BaseEntity
    {
        [Required]
        [StringLength(20)]
        [Comment("公司别")]
        public override string company { get; set; }

        [Required]
        [StringLength(20)]
        [Comment("公司代码，与员工所属公司代码串，确认此公司代码下的员工可以填哪些公司别")]
        [Column("company_code")]
        public  string companyCode { get; set; }

        [Required]
        [StringLength(20)]
        [Comment("工作地，TW相同公司代码下工作地可能不一样，可选公司别也要不一样")]
        public string site { get; set; }

    }
}
