using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [Table("emp_org_cohead")]
    public class EmpOrgCohead : BaseEntity
    {
        [Required]
        [StringLength(20)]
        public string deptid { get; set; }
        [Required]
        [StringLength(20)]
        public string manager_id { get; set; }
        public int seq { get; set; }
        public int flag { get; set; }
    }
}
