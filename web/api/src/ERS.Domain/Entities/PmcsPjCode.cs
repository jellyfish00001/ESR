using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [Table("pmcs_pjcode")]
    public class PmcsPjCode : BaseEntity
    {
        [StringLength(20)]
        public string code { get; set; }
        [StringLength(100)]
        public string description { get; set; }
        [StringLength(20)]
        public string ordertype { get; set; }
        [StringLength(20)]
        public string classification { get; set; }
        [StringLength(20)]
        public string pm { get; set; }
        [StringLength(20)]
        public string am { get; set; }
        [StringLength(40)]
        public string lifecycle { get; set; }
        [StringLength(20)]
        public string chargedept { get; set; }
        [StringLength(50)]
        public string custnickname { get; set; }
        [StringLength(20)]
        public string profitcenter { get; set; }
        public DateTime? budgetstartdate { get; set; }
        public DateTime? budgetenddate { get; set; }
        [StringLength(400)]
        public string remark { get; set; }
        [StringLength(20)]
        public string iscreatedbyprjpnl { get; set; }
        public DateTime? lastupdate { get; set; }
    }
}
