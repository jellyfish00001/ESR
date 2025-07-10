using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [Table("emp_org_lv")]
    public class EmpOrgLv : BaseEntity
    {
        public int tree_level_num { get; set; }
        [StringLength(100)]
        public string tree_level { get; set; }
        [StringLength(100)]
        public string descr { get; set; }
        [StringLength(100)]
        public string descr_a { get; set; }
        [StringLength(20)]
        public string stat { get; set; }
        public int seq { get; set; }
        public string officer_level { get; set; }
    }
}
