using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERS.Entities
{
    [Table("employeeinfo")]
    public class EmployeeInfo : BaseEntity
    {
        [Key] // 添加主键属性
        [Required]
        [StringLength(11)]
        public string emplid { get; set; }

        [Required]
        [StringLength(6)]
        public string bu { get; set; }

        [Required]
        [StringLength(4)]
        public string bg { get; set; }

        [Required]
        [StringLength(6)]
        public string site { get; set; }

        [Required]
        [StringLength(4)]
        public string plant { get; set; }

        [Required]
        [StringLength(10)]
        public string location { get; set; }

        [Required]
        [StringLength(50)]
        public string name { get; set; }

        [Required]
        [StringLength(50)]
        public string name_a { get; set; }

        [Required]
        public DateTime hire_dt { get; set; }

        [Required]
        [StringLength(10)]
        public string sal_location_a { get; set; }

        [Required]
        [StringLength(3)]
        public string company { get; set; }

        [Required]
        [StringLength(6)]
        public string deptid { get; set; }

        [Required]
        [StringLength(60)]
        public string jobtitle_descr { get; set; }

        [Required]
        [StringLength(70)]
        public string emailid { get; set; }

        [Required]
        [StringLength(50)]
        public string email_address_a { get; set; }

        [Required]
        [StringLength(15)]
        public string phone_a { get; set; }

        [Required]
        public decimal officer_level_a { get; set; }

        [Required]
        [StringLength(11)]
        public string supervisor_id { get; set; }

        [Required]
        public decimal tree_level_num { get; set; }

        public DateTime? termination_dt { get; set; }

        [Required]
        [StringLength(6)]
        public string labor_type { get; set; }

        [Required]
        [StringLength(6)]
        public string job_family { get; set; }

        [Required]
        public DateTime last_updt_dt { get; set; }

        [StringLength(6)]
        public string jobcode { get; set; }

        public decimal? supv_lvl_id { get; set; }

        public decimal? tree_level { get; set; }

        [StringLength(6)]
        public string sal_location { get; set; }

        public DateTime? last_hire_dt { get; set; }
    }
}
