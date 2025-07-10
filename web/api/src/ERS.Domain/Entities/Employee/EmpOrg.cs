using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [Table("emp_org")]
    [Index(nameof(deptid), Name = "deptid_idx")]
    public class EmpOrg : BaseEntity
    {
        [Required]
        [StringLength(20)]
        public string deptid { get; set; }
        [StringLength(100)]
        public string descr { get; set; }
        [StringLength(100)]
        public string descr_d { get; set; }
        [StringLength(20)]
        public string manager_id { get; set; }
        [StringLength(20)]
        public int tree_level_num { get; set; }
        [StringLength(20)]
        public string uporg_code_a { get; set; }
        [StringLength(20)]
        public string company { get; set; }
        [StringLength(20)]
        public string plant_id_a { get; set; }
        [StringLength(20)]
        public string location { get; set; }
        [StringLength(20)]
        public string sal_location_a { get; set; }
        public int flag { get; set; }
    }
}
