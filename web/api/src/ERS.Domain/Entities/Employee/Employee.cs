using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    /// <summary>
    /// employee表已弃用，改用employeeinfo
    /// </summary>
    [Table("employee")]
    [Index(nameof(emplid), Name = "emplid_idx")]
    public class Employee : BaseEntity
    {
        [Required]
        [StringLength(44)]
        public string emplid { get; set; }
        [StringLength(240)]
        public string cname { get; set; }
        [StringLength(200)]
        public string ename { get; set; }
        [StringLength(20)]
        public string plant { get; set; }
        [StringLength(280)]
        public string mail { get; set; }
        [StringLength(40)]
        public string deptid { get; set; }
        [StringLength(20)]
        public string upper_dept { get; set; }
        [StringLength(20)]
        public string empl_category { get; set; }
        [StringLength(44)]
        public string supervisor { get; set; }
        public int officer_level { get; set; }
        [StringLength(20)]
        public string cardid { get; set; }
        [StringLength(20)]
        public string proxy { get; set; }
        [StringLength(20)]
        public string l1 { get; set; }
        [StringLength(20)]
        public string tdate { get; set; }
        [StringLength(20)]
        public string treason { get; set; }
        public DateTime? udate { get; set; }
        [StringLength(20)]
        public string userid { get; set; }
        [StringLength(100)]
        public string deptn { get; set; }
        public DateTime? hdate { get; set; }
        [StringLength(20)]
        public string calendar { get; set; }
        [StringLength(20)]
        public string atvalid { get; set; }
        [StringLength(180)]
        public string descrshort { get; set; }
        public int otlimit { get; set; }
        [StringLength(20)]
        public string otvalid { get; set; }
        [StringLength(20)]
        public string nxtapprove { get; set; }
        public DateTime? rehire_dt { get; set; }
        [StringLength(20)]
        public string proxytime { get; set; }
        public DateTime? ot_stime { get; set; }
        public DateTime? ot_etime { get; set; }
        [StringLength(20)]
        public string adult { get; set; }
        [StringLength(96)]
        public string phone { get; set; }
    }
}
