using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace ERS.DTO.Application
{
    public class CashDetailDto
    {
        public Guid? Id { get; set; }
        [StringLength(20)]
        public string formcode { get; set; }//CASH_2默认
        [StringLength(20)]
        public string rno { get; set; }//单号
        public string advancerno { get; set; } // 预支单号
        public int seq { get; set; }//item
        public DateTime? rdate { get; set; }//宴客时间、需款日期（cash3）
        public Guid? senarioid { get; set; }//報銷情景id
        [StringLength(30)]
        public string senarioname { get; set; }//報銷情景名稱
        [StringLength(20)]
        public string currency { get; set; }//币别、币别（cash3）
        /// <summary>
        /// 实际报销
        /// </summary>
        public decimal? amount { get; set; }
        public decimal? amount1 { get; set; }//依人数预算之总预算、申请金额（cash3）
        [StringLength(20)]
        public string deptid { get; set; }//费用挂账部门
        [StringLength(500)]
        public string summary { get; set; }//null、摘要（cash3）
        [StringLength(100)]
        public string @object { get; set; }//宴客名称
        [StringLength(100)]
        public string keep { get; set; }//公司最高主管
        [StringLength(500)]
        public string remarks { get; set; }//null、备注（cash3）
        public decimal? amount2 { get; set; }//null
        [StringLength(10)]
        public string basecurr { get; set; }//币别
        // 实际支出
        public decimal? baseamt { get; set; }
        public decimal? rate { get; set; }//客户参加人数
        [StringLength(20)]
        public string expcode { get; set; }//EXP02默认/预支场景code（cash3）
        [StringLength(100)]
        public string expname { get; set; }//交际费、预支场景（cash3）
        [StringLength(20)]
        public string acctcode { get; set; }//71412000
        [StringLength(50)]
        public string acctname { get; set; }//交際費-其他
        public DateTime? revsdate { get; set; }//null、预定冲账日期（cash3）
        [StringLength(20)]
        public string paytyp { get; set; }//null
        [StringLength(50)]
        public string payname { get; set; }//null、请款方式（cash3）
        [StringLength(20)]
        public string payeeid { get; set; }//null
        [StringLength(200)]
        public string payeename { get; set; }//null
        [StringLength(200)]
        public string payeeaccount { get; set; }//null
        [StringLength(100)]
        public string bank { get; set; }//银行信息
        [StringLength(1)]
        public string stat { get; set; }//null
        [StringLength(20)]
        public string payeedeptid { get; set; }//null
        public decimal? advanceamount { get; set; }//null
        [StringLength(20)]
        public string advancecurrency { get; set; }//null
        public decimal? advancebaseamount { get; set; }//null
        [StringLength(50)]
        public string invoice { get; set; }//null
        public decimal? pretaxamount { get; set; }//null
        public decimal? taxexpense { get; set; }
        // 宴客地點
        [StringLength(200)]
        public string treataddress { get; set; }
        // 客户参加其他成员
        [StringLength(200)]
        public string otherobject { get; set; }
        // 客户参与人数
        public decimal? objectsum { get; set; }
        // 公司最高主管所屬类别
        [StringLength(50)]
        public string keepcategory { get; set; }
        // 公司其他成员
        [StringLength(200)]
        public string otherkeep { get; set; }
        // 公司参与人数
        public decimal? keepsum { get; set; }
        public string isaccordnumber { get; set; }//是否符合我方人數規範 Y/N
        [StringLength(200)]
        public string notaccordreason { get; set; }//是否符合我方人數規範 若否请说明原因
        //实际支出
        public decimal? actualexpense { get; set; }
        public decimal? lastexpense { get; set; }
        // 是否符合費用規範 Y/N
        public string isaccordcost { get; set; }
        // 超出預算金額
        public decimal? overbudget { get; set; }
        // 不符合費用規範的选项 （0/1）
        [StringLength(100)]
        public string processmethod { get; set; }
        // 客戶/賓客最高主管姓名/職稱
        [StringLength(200)]
        public string custsuperme { get; set; }
        // 餐飲宴客時間段(0/1/2)
        public int flag { get; set; }
        [StringLength(50)]
        public string unifycode { get; set; }
        [StringLength(200)]
        public string treattime { get; set; }//餐飲宴客時間段内容
        public decimal? taxbaseamount { get; set; }
        [StringLength(50)]
        public string requestrno { get; set; }
        // 时间差异原因(201a)
        [StringLength(300)]
        public string datediffreason { get; set; }
        // 实际报销(201a)
        public decimal? paymentexpense { get; set; }
        [StringLength(100)]
        public string assignment { get; set; }
        public DateTime? hospdate { get; set; }
        [StringLength(1)]
        public string istemp { get; set; }
        public List<InvoiceDto> invList { get; set; }
        public List<CashFileDto> fileList { get; set; }
        public DateTime? cdate { get; set; } //申請時間
        public int delaydays { get; set; }//延期天數
        public string delayreason { get; set; }//延期原因
        public List<departCost> deptList { get; set; }
        //[Comment("城市(公出誤餐費)")]
        public string city { get; set; }
        //[Comment("出发时间(公出誤餐費)")]
        public DateTime? gotime { get; set; }
        //[Comment("回到时间(公出誤餐費)")]
        public DateTime? backtime { get; set; }
        //[Comment("起始目的地 (自駕油費）")]
        public string location { get; set; }
        //[Comment("车型 (自駕油費）")]
        public int cartype { get; set; }
        //[Comment("路程数 (自駕油費）")]
        public decimal? journey { get; set; }
        [StringLength(20)]
        public string companycode { get; set; }
        public string company { get; set; }
    }
    public class departCost
    {
        public string deptId { get; set; }
        public int percent { get; set; }
        public decimal? amount { get; set; }
        public decimal? baseamount { get; set; }
    }
    public class signCost
    {
        public string deptid { get; set; }
        public decimal? cost { get; set; }
    }
}
