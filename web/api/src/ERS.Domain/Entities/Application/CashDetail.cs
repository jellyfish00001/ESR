using ERS.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [Table("cash_detail")]
    [Index(nameof(rno), Name = "rno_idx")]
    public class CashDetail : BaseEntity
    {
        [StringLength(20)]
        public string formcode { get; set; }
        [StringLength(20)]
        public string rno { get; set; }
        [StringLength(20)]
        [Comment("预支单号")]
        public string advancerno { get; set; }
        public int seq { get; set; }
        [Comment("Reimburse date")]
        public DateTime? rdate { get; set; }
        [StringLength(20)]
        public string currency { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        [Comment("user key in amount")]
        public decimal? amount1 { get; set; }
        [StringLength(1000)]
        public string deptid { get; set; }
        [StringLength(500)]
        public string summary { get; set; }
        [StringLength(100)]
        public string @object { get; set; }
        [StringLength(100)]
        public string keep { get; set; }
        [StringLength(500)]
        public string remarks { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        [Comment("Reimbursement amount(实际报销)")]
        public decimal? amount { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        [Comment("accountant key in amount")]
        public decimal? amount2 { get; set; }
        [StringLength(10)]
        [Comment("base currency")]
        public string basecurr { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        [Comment("base amount")]
        public decimal? baseamt { get; set; }
        [Column(TypeName = "decimal(10, 5)")]
        public decimal? rate { get; set; }
        public Guid? senarioid { get; set; } // 報銷情景id
        [StringLength(30)]
        public string? senarioname { get; set; }

        [StringLength(20)]
        public string expcode { get; set; }
        [StringLength(100)]
        public string expname { get; set; }
        [StringLength(20)]
        public string acctcode { get; set; }
        [StringLength(50)]
        public string acctname { get; set; }
        [Comment("Scheduled reversal date")]
        public DateTime? revsdate { get; set; }
        [StringLength(20)]
        [Comment("Pay type")]
        public string paytyp { get; set; }
        [StringLength(50)]
        public string payname { get; set; }
        [StringLength(20)]
        public string payeeid { get; set; }
        [StringLength(200)]
        public string payeename { get; set; }
        [StringLength(200)]
        public string payeeaccount { get; set; }
        [StringLength(100)]
        public string bank { get; set; }
        [StringLength(1)]
        [Comment("Status; Y: available, N: Not available")]
        public string stat { get; set; }
        [StringLength(20)]
        public string payeedeptid { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        [Comment("Batch Pay Advance Amount")]
        public decimal? advanceamount { get; set; }
        [StringLength(20)]
        public string advancecurrency { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        [Comment("ADVANCEBaseAMOUNT")]
        public decimal? advancebaseamount { get; set; }
        [StringLength(50)]
        public string invoice { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        [Comment("PreTaxAmount")]
        public decimal? pretaxamount { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        [Comment("Taxexpense")]
        public decimal? taxexpense { get; set; }
        [StringLength(200)]
        [Comment("Treat  Address")]
        public string treataddress { get; set; }
        [StringLength(200)]
        [Comment("Treat other object")]
        public string otherobject { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        [Comment("customer  how many people join")]
        public decimal? objectsum { get; set; }
        [StringLength(50)]
        [Comment("Supreme head of the category")]
        public string keepcategory { get; set; }
        [StringLength(200)]
        [Comment("Treat other keep")]
        public string otherkeep { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        [Comment("wistron how many people  join")]
        public decimal? keepsum { get; set; }
        [StringLength(20)]
        [Comment("是否符合我方人數規範 Y/N")]
        public string isaccordnumber { get; set; }
        [StringLength(200)]
        [Comment("不符合我方人數規範的原因")]
        public string notaccordreason { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        [Comment("实际支出 Actual Expense")]
        public decimal? actualexpense { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        [Comment("Actual Expense")]
        public decimal? lastexpense { get; set; }
        [StringLength(20)]
        [Comment("是否符合費用規範 Y/N")]
        public string isaccordcost { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        [Comment("超出預算金額")]
        public decimal? overbudget { get; set; }
        [StringLength(100)]
        [Comment("不符合費用規範的选项")]
        public string processmethod { get; set; }
        [StringLength(200)]
        [Comment("customer superme")]
        public string custsuperme { get; set; }
        [Comment("0, other, 1.treat,2, treatcpbg")]
        public int flag { get; set; }
        [StringLength(50)]
        public string unifycode { get; set; }
        [StringLength(200)]
        [Comment("Treat  Time Bucket")]
        public string treattime { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        [Comment("TAX BASE AMOUNT(稅基)")]
        public decimal? taxbaseamount { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        [Comment("实际报销")]
        public decimal? paymentexpense { get; set; }
        [StringLength(100)]
        public string assignment { get; set; }
        [StringLength(300)]
        [Comment("时间差异原因")]
        public string datediffreason { get; set; }
        [Comment("Hospitality Date")]
        public DateTime? hospdate { get; set; }
        [StringLength(1)]
        [Comment("是否暂存(Y/N)")]
        public string istemp { get; set; }
        [Comment("延期天數")]
        public int delaydays {get; set;}
        [StringLength(400)]
        [Comment("延期原因")]
        public string delayreason {get; set;}
        [StringLength(100)]
        [Comment("城市(公出誤餐費)")]
        public string city { get; set; }
        [Comment("出发时间(公出誤餐費)")]
        public DateTime? gotime { get; set; }
        [Comment("回到时间(公出誤餐費)")]
        public DateTime? backtime { get; set; }
        [Comment("起始目的地 (自駕油費）")]
        [StringLength(200)]
        public string location { get; set; }
        [Comment("车型 (自駕油費）")]
        public int cartype { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        [Comment("路程数 (自駕油費）")]
        public decimal? journey { get; set; }
        [Comment("公司别代码")]
        [StringLength(20)]
        public string companycode { get; set; }
        [Column("origin")]
        [StringLength(100)]
        [Comment("起点")]
        public string Origin { get; set; }
        [Column("destination")]
        [StringLength(100)]
        [Comment("终点")]
        public string Destination { get; set; }
        [Column("passenger")]
        [StringLength(50)]
        [Comment("乘车人")]        
        public string Passenger { get; set; }
        
        [Column("importtax", TypeName = "decimal(18, 2)")]
        [Comment("進口稅/其他費用")]
        public decimal? importTax { get; set; }
        
        [Column("tradepromotionfee",TypeName = "decimal(18, 2)")]
        [Comment("推貿費")]
        public decimal? tradePromotionFee { get; set; }
        [Column("totaltaxandfee",TypeName = "decimal(18, 2)")]
        [Comment("稅費合計")]
        public decimal? totalTaxAndFee { get; set; }

        [Column("agentemplid")]
        [StringLength(20)]
        [Comment("承辦人工號")]
        public string? agentEmplid { get; set; }

        [Column("billnoandsummary")]
        [StringLength(100)]
        [Comment("提單號碼/費用摘要")]
        public string? billNoAndSummary { get; set; }

        public void SetRno(string rno) => this.rno = rno;
        public void SetDeptId(string deptid) => this.deptid = deptid;
        public void SetKeepStatus() => this.istemp = "Y";
        public void SetCash1(CashDetail data)
        {
            this.formcode = "CASH_1";
            this.amount2 = 0;
        }
        public void SetCash2(CashDetail data)
        {
            this.formcode = "CASH_2";
            this.expcode = "EXP02";
            this.flag = 0;
        }
        public void SetCash2A(CashDetail data)
        {
            this.formcode = "CASH_2";
            this.expcode = "EXP02";
        }
        public void SetCash3(CashDetail data)
        {
            this.formcode = "CASH_3";
            this.acctcode = acctcode;
            this.acctname = acctname;
            this.amount2 = 0;
            this.flag = 0;
        }
        public void SetCash3A(CashDetail data)
        {
            this.formcode = "CASH_3A";
        }
        public void SetCash4(CashDetail data)
        {
            this.formcode = "CASH_4";
            this.amount2 = 0;
        }
        public void SetCash5(CashDetail data)
        {
            this.formcode = "CASH_5";
        }
        public void SetCash6(CashDetail data)
        {
            this.formcode = "CASH_6";
        }
        public void SetCashX(CashDetail data)
        {
            this.formcode = "CASH_X";
        }
        public void SetExp(ExpAcctDto exp)
        {
            this.expname = exp.expname;
            this.acctcode = exp.acctcode;
            this.acctname = exp.acctname;
        }
        public void SetFormCode(string formcode) => this.formcode = formcode;
        public void SetCompanyCode(string companycode) => this.companycode = companycode;
    }
}
