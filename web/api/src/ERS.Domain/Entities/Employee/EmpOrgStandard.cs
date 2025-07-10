using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [Table("emp_org_standard")]
    public class EmpOrgStandard : BaseEntity
    {
        [Required]
        [StringLength(20)]
        public string deptid { get; set; }
        [StringLength(100)]
        public string descr { get; set; }
        [StringLength(100)]
        public string descr_a { get; set; }
        [StringLength(20)]
        public string manager_id { get; set; }
        public int tree_level_num { get; set; }
        [StringLength(20)]
        public string uporg_code_a { get; set; }
        [StringLength(20)]
        public string plant_id_a { get; set; }
        [StringLength(20)]
        public string location { get; set; }
        [StringLength(20)]
        public string sal_location_a { get; set; }
        public int flag { get; set; }
    }
}
