using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [Table("expense_senario_extra_steps")]
    public class ExpenseSenarioExtraSteps : BaseEntity
    {
        [Required]
        [StringLength(20)]
        [Comment("費用代碼")]
        public string exp_code { get; set; }

        [Required]
        [StringLength(10)]
        [Comment("步驟名稱")]
        public string name { get; set; }

        [Required]
        [StringLength(10)]
        [Comment("步驟位置")]
        public string position { get; set; }

        [Required]
        [StringLength(11)]
        [Comment("簽核人員工號")]
        public string approver_emplid { get; set; }

        [StringLength(50)]
        [Comment("簽核人員姓名")]
        public string approver_name { get; set; }

        [StringLength(50)]
        [Comment("簽核人員別名")]
        public string approver_name_a { get; set; }

        [Comment("bdexp table FK")]
        public Guid bdexp_id { get; set; }
    }
}
