using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    /// <summary>
    /// 已废弃
    /// </summary>
    [Table("bdexp")]
    [Index(nameof(company), nameof(expcode), Name = "company_expcode_idx")]
    public class BdExp : BaseEntity
    {
        [Required]
        [StringLength(20)]
        public string expcode { get; set; }
        [Required]
        [StringLength(50)]
        public string expname { get; set; }
        [StringLength(50)]
        public string acctcode { get; set; }
        [StringLength(2000)]
        public string keyword { get; set; }
        [StringLength(1)]
        [Comment("Status; Y: available, N: Not available")]
        public string stat { get; set; }
        [StringLength(10)]
        public string classno { get; set; }
        [StringLength(1)]
        [Comment("0 NONE Generate Invoice , 1 System Generate Invoice")]
        public string invoiceflag { get; set; }
        [StringLength(1)]
        [Comment("0: NON VaT 1 :VAT")]
        public string flag { get; set; }
        //[StringLength(50)]
        //public string companycode { get; set; }
        [StringLength(1)]
        [Comment("'0':除大量報銷外都可以見；‘1’：只有大量報銷可見；‘2’：所有類型都可見")]
        public string type { get; set; }
        [StringLength(50)]
        [Comment("invited by user")]
        public string invter { get; set; }
        [StringLength(200)]
        [Comment("費用說明")]
        public string description { get; set; }
        [StringLength(50)]
        public string wording { get; set; }
        [StringLength(1)]
        [Comment("Expense Type to Special control")]
        public string special { get; set; }
        [StringLength(1)]
        [Comment("The control invoice is zero tax rate and tax exemption rate. Check the box to indicate 'Y', no check to indicate 'N'.")]
        public string iszerotaxinvter { get; set; }
        [StringLength(1)]
        [Comment("是否发送提醒交件email")]
        public string issendremindemail { get; set; }
        [StringLength(50)]
        public string assignment { get; set; }
        [StringLength(50)]
        public string costcenter { get; set; }
        [StringLength(50)]
        public string pjcode { get; set; }
        [StringLength(50)]
        [Comment("附件类型")]
        public string filecategory { get; set; }
        [StringLength(1)]
        [Comment("附件是否必须上传(Y/N)")]
        public string isupload { get; set; }
        [StringLength(200)]
        [Comment("附件審核要點")]
        public string filepoints { get; set; }
        [StringLength(1)]
        [Comment("是否有發票(Y/N)")]
        public string isinvoice { get; set; }
        [StringLength(200)]
        [Comment("發票類型")]
        public string invoicecategory { get; set; }
        [StringLength(1)]
        [Comment("稅金抵扣(Y/N)")]
        public string isdeduction { get; set; }
        [StringLength(50)]
        [Comment("加簽步骤")]
        public string addsignstep { get; set; }
        [StringLength(50)]
        [Comment("加簽工号")]
        public string addsign { get; set; }
        [Comment("部门逾期天数")]
        public int departday { get; set; }
        [Comment("处级逾期天数")]
        public int sectionday { get; set; }
        [Comment("计算方式：（1：今天-预支付款日期，2：今天-预期冲账日期）")]
        public int calmethod { get; set; }
        [Comment("报销填写界面：（1：公出误餐费，2：自驾油费）")]
        public int expcategory { get; set; }
        [Comment("類型區分：1.報銷 2.預支 3.薪资请款")]
        public int category { get; set; }
        [Comment("预支冲账日期是否卡关（Y/N）")]
        [StringLength(1)]
        public string datelevel { get; set; }
        [StringLength(50)]
        [Comment("授权人")]
        public string authorizer { get; set; }
        [StringLength(50)]
        [Comment("被授权人")]
        public string authorized { get; set; }
        [Comment("授权开始日期")]
        public DateTime? sdate { get; set; }
        [Comment("授权结束日期")]
        public DateTime? edate { get; set; }
        [StringLength(200)]
        [Comment("費用描述​")]
        public string expdesc { get; set; }
        [StringLength(100)]
        [Comment("摘要提示")]
        public string descriptionnotice { get; set; }
    }
}
