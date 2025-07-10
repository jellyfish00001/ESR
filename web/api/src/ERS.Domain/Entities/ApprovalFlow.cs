using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERS.Entities
{
    [Table("approval_flow")]
    [Comment("簽核流程主檔")]
    public class ApprovalFlow : BaseEntity
    {
        public void SetId(Guid id)
        {
          Id = id;
        }

        [Required]
        [StringLength(20)]
        public string rno { get; set; } // 報銷單號

        [Required]
        [Column(TypeName = "decimal(8, 0)")]
        public decimal step { get; set; } // 步驟順序

        [StringLength(100)]
        public string stepname { get; set; } // 步驟名稱

        [StringLength(100)]
        public string approveremplid { get; set; } // 實際簽核人工號

        [StringLength(100)]
        public string approvername { get; set; } // 實際簽核人姓名

        [StringLength(100)]
        public string approverdeptid { get; set; } // 實際簽核人部門代碼
        
         [Required]
        [StringLength(100)]
        public string aassignedemplid { get; set; } // 可簽核人工號

        [Required]
        [StringLength(100)]
        public string assignedapprovername { get; set; } // 可簽核人姓名

        [StringLength(100)]
        public string assigneddeptid { get; set; } // 可簽核人部門代碼

        [Required]
        [StringLength(1)]
        public string status { get; set; } // 簽核狀態

        public Guid nextflowid { get; set; } // 下一步驟簽核流程ID

        [StringLength(200)]
        public string comment { get; set; } // 簽核意見

        public DateTime? approvedate { get; set; } // 簽核時間
    }
}