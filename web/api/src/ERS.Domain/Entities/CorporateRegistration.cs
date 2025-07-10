using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
namespace ERS.Entities
{
    [Table("corporate_registration")]
    [Index(nameof(unifiedNo), Name = "unifiedNo_idx")]
    public class CorporateRegistration : BaseEntity
    {
        [Required]
        [StringLength(100)]
        [Comment("統一編號")]
        public string unifiedNo { get; set; }

        [StringLength(100)]
        [Comment("營業人名稱")]
        public string name { get; set; }

        [StringLength(1)]
        [Comment("使用統一發票")]
        public string usesInvoice { get; set; }

        [StringLength(500)]
        [Comment("資料更新時間")]
        public string updateTime { get; set; }

        // 构造函数
        public CorporateRegistration()
        {
            Id = Guid.NewGuid(); // 在初始化时为Id分配一个新的唯一Guid
        }
    }
}