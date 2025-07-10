using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [Table("extra_steps")]
    [Index(nameof(SenarioId), Name = "extra_steps_idx_senarioid")]
    public class ExtraSteps : SuperBaseEntity
    {
        [Required]
        [StringLength(10)]
        [Column("name")]
        [Comment("步驟名稱")]
        public string Name { get; set; }

        [Required]
        [StringLength(10)]
        [Column("position")]
        [Comment("步驟位置")]
        public string Position { get; set; }

        [Required]
        [StringLength(11)]
        [Column("approver_emplid")]
        [Comment("簽核人員工號")]
        public string ApproverEmplid { get; set; }

        [StringLength(50)]
        [Column("approver_name")]
        [Comment("簽核人員姓名")]
        public string ApproverName { get; set; }

        [StringLength(50)]
        [Column("approver_name_a")]
        [Comment("簽核人員別名")]
        public string ApproverNameA { get; set; }

        [Column("senario_id")]
        [Comment("報銷情景ID")]
        public Guid SenarioId { get; set; }
    }
}
