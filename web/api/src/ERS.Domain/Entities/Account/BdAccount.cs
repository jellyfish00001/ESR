using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [Table("bdaccount")]
    [Index(nameof(acctcode), Name = "acctcode_idx")]
    public class BdAccount : BaseEntity
    {
        [Required]
        [StringLength(50)]
        public string acctcode { get; set; }
        [Required]
        [StringLength(50)]
        public string acctname { get; set; }
        public int seq { get; set; }
        [StringLength(1)]
        [Comment("Status; Y: available, N: Not available")]
        public string stat { get; set; }
        //[Required]
        //[StringLength(20)]
        //public string companycode { get; set; }
    }
}
