using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ERS.Entities
{
    [Table("bd_expense")]
    [Index(nameof(ExpenseCode), Name = "bd_expense_idx_expense_code")]
    [Index(nameof(ExpenseName), Name = "bd_expense_idx_expense_name")]
    public class BDExpense : BaseEntity
    {
        [Required]
        [StringLength(10)]
        [Comment("費用類別代碼")]
        [Column("expense_code")]
        public string ExpenseCode { get; set; }

        [Required]
        [StringLength(300)]
        [Comment("費用類別名稱")]
        [Column("expense_name")]
        public string ExpenseName { get; set; }

        [StringLength(300)]
        [Comment("費用類別名稱(簡中)")]
        [Column("expense_name_zhcn")]
        public string ExpenseNameZhcn { get; set; }

        [StringLength(300)]
        [Comment("費用類別名稱(繁中)")]
        [Column("expense_name_zhtw")]
        public string ExpenseNameZhtw { get; set; }

        [StringLength(300)]
        [Comment("費用類別名稱(越文)")]
        [Column("expense_name_vn")]
        public string ExpenseNameVn { get; set; }

        [StringLength(300)]
        [Comment("費用類別名稱(西文)")]
        [Column("expense_name_es")]
        public string ExpenseNameEs { get; set; }

        [StringLength(300)]
        [Comment("費用類別名稱(捷克文)")]
        [Column("expense_name_cz")]
        public string ExpenseNameCz { get; set; }

        [StringLength(300)]
        [Comment("費用類別說明")]
        [Column("expense_description")]
        public string ExpenseDescription { get; set; }
    }
}
