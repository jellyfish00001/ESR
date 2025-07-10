using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [Table("cash_file")]
    [Index(nameof(rno), Name = "rno_idx")]
    public class CashFile : BaseEntity
    {
        [Required]
        [StringLength(20)]
        public string rno { get; set; }
        [Required]
        [Comment("明细的item")]
        public int seq { get; set; }
        [Required]
        [Comment("明细item中的附件item")]
        public int item { get; set; }
        [StringLength(50)]
        [Comment("attachment category")]
        public string category { get; set; }
        [StringLength(100)]
        [Comment("attachment file type")]
        public string filetype { get; set; }
        [StringLength(200)]
        [Comment("path")]
        public string path { get; set; }
        [StringLength(200)]
        [Comment("attachment file name")]
        public string filename { get; set; }
        [StringLength(20)]
        public string formcode { get; set; }
        [StringLength(200)]
        [Comment("original filename")]
        public string tofn { get; set; }
        [StringLength(1)]
        public string status { get; set; }
        [StringLength(1)]
        [Required]
        [Comment("是否head附件")]
        // Y 为head附件，N为发票，F为情景文件
        public string ishead { get; set; }
        [NotMapped]
        public string key { get; set; }
        [Comment("票夹发票id")]
        [NotMapped]
        public Guid? invoiceid { get; set; }
        public void SetRno(string rno) => this.rno = rno;
        public void SetOriginalFileName(string name) => this.tofn = name;
        public void SetFileType(string fileType) => this.filetype = fileType;
        public void SetSavePath(string path) => this.path = path;
        public void SetFormCode(string formcode) => this.formcode = formcode;
    }
}
