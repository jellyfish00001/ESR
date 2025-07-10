using System;
using System.ComponentModel.DataAnnotations;
namespace ERS.DTO.Application
{
    public class CashFileDto
    {
        [StringLength(20)]
        public string rno { get; set; }
        [Required]
        public int seq { get; set; }
        [Required]
        public int item { get; set; }
        [StringLength(50)]
        public string category { get; set; }
        [StringLength(20)]
        public string filetype { get; set; }
        [StringLength(200)]
        public string path { get; set; }
        [StringLength(200)]
        public string filename { get; set; }
        [StringLength(20)]
        public string formcode { get; set; }
        [StringLength(200)]
        public string tofn { get; set; }
        // F:附件  其他为发票
        [StringLength(1)]
        public string status { get; set; }
        [StringLength(1)]
        [Required]
        public string ishead { get; set; }
        public string key { get; set; }
        public string url { get; set; }
        // 票夹发票id
        public Guid? invoiceid { get; set; }
        public string invoiceno { get; set; }

        public DateTime? cdate { get; set; }
    }
}
