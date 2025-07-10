using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/**************************************************************************
 * Author: Andy YA Chung
 * Date: 2025-05-12
 * Name: 報銷情景 Entity
 * Description:
 **************************************************************************/

namespace ERS.Entities
{
    [Table("bd_expense_senario")]
    [Index(nameof(senarioname), Name = "bd_expense_senario_idx_senarioname")]
    [Index(nameof(expensecode), Name = "bd_expense_senario_idx_expensecode")]
    public class BDExpenseSenario : SuperBaseEntity
    {
        [Required]
        [StringLength(10)]
        [Comment("公司別")]
        public string companycategory { get; set; }

        [Required]
        [StringLength(30)]
        [Comment("報銷情景名稱")]
        public string senarioname { get; set; }

        [StringLength(300)]
        [Comment("關鍵字")]
        public string keyword { get; set; }

        [Required]
        [StringLength(10)]
        [Comment("費用類別代碼")]
        public string expensecode { get; set; }

        [Required]
        [StringLength(10)]
        [Comment("會計科目")]
        public string accountcode { get; set; }

        [Required]
        [StringLength(3)]
        [Comment("核決權限代碼")]
        public string auditlevelcode { get; set; }

        [StringLength(300)]
        [Comment("摘要提示")]
        public string descriptionnotice { get; set; }

        [StringLength(300)]
        [Comment("上傳附件提示")]
        public string attachmentnotice { get; set; }

        [StringLength(300)]
        [Comment("上傳附件名稱")]
        public string attachmentname { get; set; }

        [Required]
        [Comment("是否需繳交紙本附件")]
        public bool requirespaperattachment { get; set; }

        [Required]
        [Comment("是否需附上發票憑證")]
        public bool requiresinvoice { get; set; }

        [Required]
        [Comment("是否可抵扣稅金")]
        public bool isvatdeductable { get; set; }

        [Required]
        [Comment("是否可跳過會計簽核")]
        public bool canbypassfinanceapproval { get; set; }

        [Required]
        [Comment("是否需上傳附件")]
        public bool requiresattachment { get; set; }

        [Required]
        [Comment("報銷模塊")]
        public int extraformcode { get; set; } = 0;

        [StringLength(50)]
        [Comment("Assignment")]
        public string assignment { get; set; }

        [StringLength(50)]
        [Comment("Cost center")]
        public string costcenter { get; set; }

        [StringLength(50)]
        [Comment("Project code")]
        public string projectcode { get; set; }

        [Required]
        [StringLength(30)]
        [Comment("分類 reimbursement/ advance/ payroll/ mass")]
        public string category { get; set; }

        [Comment("计算方式：（1：今天-预支付款日期，2：今天-预期冲账日期）")]
        public int calmethod { get; set; }

        [Comment("部门逾期天数")]
        public int departday { get; set; }

        [Comment("处级逾期天数")]
        public int sectionday { get; set; }

        [StringLength(1)]
        [Comment("预支冲账日期是否卡关（Y/N）預支沖帳日期是否需限制於一個月內")]
        public string datelevel { get; set; }

        [StringLength(50)]
        [Comment("被授权人")]
        public string authorized { get; set; }

        [StringLength(50)]
        [Comment("授权人")]
        public string authorizer { get; set; }

        [Comment("授权结束日期")]
        public DateTime? edate { get; set; }

        [Comment("授权开始日期")]
        public DateTime? sdate { get; set; }


    }
}
