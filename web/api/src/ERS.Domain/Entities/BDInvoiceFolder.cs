using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
namespace ERS.Entities
{
    [Table("bd_invoice_folder")]
    [Index(nameof(invno), Name = "bd_invoice_folder_idx_invno_idx")]
    [Index(nameof(ocrid), Name = "bd_invoice_folder_idx_ocrid")]
    [Index(nameof(identificationno), Name = "bd_invoice_folder_idx_identificationno")]
    public class BDInvoiceFolder : BaseEntity
    {
        [StringLength(100)]
        [Comment("購貨方名稱")]
        public string buyername { get; set; }
        [StringLength(100)]
        [Comment("銷售方名稱")]
        public string sellername { get; set; }
        [StringLength(100)]
        [Comment("購買方稅號")]
        public string buyertaxid { get; set; }
        [StringLength(100)]
        [Comment("銷售方稅號")]
        public string sellertaxid { get; set; }
        [StringLength(50)]
        [Comment("發票號碼")]
        public string invno { get; set; }
        [StringLength(50)]
        [Comment("發票代碼")]
        public string invcode { get; set; }
        [StringLength(50)]
        [Comment("發票類型名稱")]
        public string invtype { get; set; }
        [Comment("發票日期")]
        public DateTime? invdate { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        [Comment("不含稅金額")]
        public decimal untaxamount { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        [Comment("稅額")]
        public decimal taxamount { get; set; }
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        [Comment("含稅總金額")]
        public decimal amount { get; set; }
        [Comment("發票核驗狀態")]
        [StringLength(50)]
        public string verifytype { get; set; }
        [Comment("發票請款狀態")]
        [StringLength(50)]
        public string paytype { get; set; }
        [Comment("異常原因")]
        [StringLength(200)]
        public string abnormalreason { get; set; }
        [Comment("備註")]
        [StringLength(300)]
        public string remark { get; set; }
        [Comment("發票路徑")]
        [StringLength(200)]
        public string filepath { get; set; }
        [StringLength(20)]
        public string rno { get; set; }
        /// <summary>
        /// ERS2.0棄用
        /// </summary>
        [Required]
        [Comment("是否填補發票信息")]
        public bool isfill { get; set; }
        [Required]
        [Comment("發票歸屬人")]
        [StringLength(20)]
        public string emplid { get; set; }
        /// <summary>
        /// ERS2.0棄用
        /// </summary>
        [Required]
        [Comment("1.全電票 2.鏈接 3.上傳檔案 4.手動輸入")]
        public int uploadmethod { get; set; }
        [StringLength(20)]
        [Comment("付款清單號")]
        public string docno { get; set; }
        [Comment("付款時間")]
        public DateTime? postingdate { get; set; }
        [Comment("幣別")]
        [StringLength(20)]
        public string currency { get; set; }
        [Comment("申请过单据后是否再次编辑过")]
        public bool isedit { get; set; }
        [Comment("税率")]
        public decimal? taxrate { get; set; }
        [Comment("attachment category")]
        [StringLength(100)]
        public string category { get; set; }
        /// <summary>
        /// ERS2.0棄用
        /// </summary>
        [Comment("是否存在发票池")]
        public bool existautopa { get; set; }
        [Comment("OCR識別記錄表ID")]
        [StringLength(100)]
        public string ocrid { get; set; }
        [Comment("票據來源")]
        [StringLength(30)]
        public string source { get; set; }
        [Comment("票據識別碼")]
        [StringLength(100)]
        public string identificationno { get; set; }
        [Comment("發票標題")]
        [StringLength(100)]
        public string invoicetitle { get; set; }
        [Comment("營業/增值税基")]
        [Column("taxbase", TypeName = "numeric(18,2)")]
        public decimal? taxbase { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        [Comment("進口稅")]
        public decimal? importtaxamount { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        [Comment("服務費/推廣貿易服務費")]
        public decimal? servicefee { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        [Comment("運輸費")]
        public decimal? shippingfee { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        [Comment("手續費")]
        public decimal? transactionfee { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        [Comment("數量")]
        public decimal? quantity { get; set; }
        [Comment("商品資訊")]
        [StringLength(100)]
        public string productinfo { get; set; }
        [Comment("出發站")]
        [StringLength(500)]
        public string startstation { get; set; }
        [Comment("抵達站")]
        [StringLength(500)]
        public string endstation { get; set; }
        [Comment("備註")]
        [StringLength(500)]
        public string remarks { get; set; }
        [Comment("承擔方")]
        [StringLength(100)]
        public string responsibleparty { get; set; }
        [Comment("課稅別")]
        [StringLength(20)]
        public string taxtype { get; set; }

        [StringLength(10)]
        [Comment("發票區域")]
        public string region { get; set; }
        [StringLength(50)]
        [Comment("發票類型代碼")]
        public string invtypecode { get; set; }
    }

}