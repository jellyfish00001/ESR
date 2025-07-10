using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERS.Entities
{
    [Table("approval_assigned_approver")]
    [Comment("簽核流程主檔")]
    public class ApprovalAssignedApprover : BaseEntity
    {
        [Required]
        public Guid flow_id { get; set; } //簽核流程ID
        [StringLength(20)]
        public string approver_emplid { get; set; } //簽核人工號
        [StringLength(100)]
        public string approver_name { get; set; } //簽核人姓名
        [StringLength(10)]
        public string approver_deptid { get; set; } //簽核人部門
    }
}