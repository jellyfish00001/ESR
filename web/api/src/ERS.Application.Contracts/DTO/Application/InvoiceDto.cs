using System;
using System.ComponentModel.DataAnnotations;
namespace ERS.DTO.Application
{
    public class InvoiceDto
    {
        public Guid Id { get; set; }
        public string rno { get; set; }
        [Required]
        public int seq { get; set; }
        [Required]
        public int item { get; set; }
        /// <summary>
        /// 发票代码
        /// </summary>
        /// <value></value>
        [StringLength(20)]
        public string invcode { get; set; }
        [StringLength(200)]
        /// <summary>
        /// 发票号码
        /// </summary>
        /// <value></value>
        public string invno { get; set; }
        /// <summary>
        /// 开票金额（不含税）
        /// </summary>
        /// <value></value>
        public decimal amount { get; set; }
        /// <summary>
        /// 开票日期
        /// </summary>
        /// <value></value>
        public DateTime? invdate { get; set; }
        /// <summary>
        /// 税额/營業稅/增值稅
        /// </summary>
        /// <value></value>
        public decimal taxamount { get; set; }
        /// <summary>
        /// 价税合计金额
        /// </summary>
        /// <value></value>
        public decimal oamount { get; set; }
        /// <summary>
        /// 发票状态
        /// </summary>
        /// <value></value>
        [StringLength(20)]
        public string invstat { get; set; }
        /// <summary>
        /// 异常报销金额
        /// </summary>
        /// <value></value>
        public decimal abnormalamount { get; set; }
        /// <summary>
        /// 税金损失
        /// </summary>
        /// <value></value>
        public decimal taxloss { get; set; }
        /// <summary>
        /// 币别
        /// </summary>
        /// <value></value>
        [StringLength(20)]
        public string curr { get; set; }
        /// <summary>
        /// 承担方
        /// </summary>
        /// <value></value>
        [StringLength(20)]
        public string undertaker { get; set; }
        public string underwriter { get; set; }
        [StringLength(200)]
        public string abnormalreason { get; set; }
        /// <summary>
        /// 付款方名称
        /// </summary>
        /// <value></value>
        public string paymentName { get; set; }
        /// <summary>
        /// 付款方证件号
        /// </summary>
        /// <value></value>
        public string paymentNo { get; set; }
        /// <summary>
        /// 收款方名称
        /// </summary>
        /// <value></value>
        public string collectionName { get; set; }
        /// <summary>
        /// 收款方证件号
        /// </summary>
        /// <value></value>
        public string collectionNo { get; set; }
        /// <summary>
        /// 异常信息
        /// </summary>
        /// <value></value>
        public string expdesc { get; set; }
        /// <summary>
        /// 发票异常原因
        /// </summary>
        /// <value></value>
        public string expcode { get; set; }
        /// <summary>
        /// 发票名称显示
        /// </summary>
        /// <value></value>
        public string invdesc { get; set; }
        /// <summary>
        /// 請款狀態 true為可請款
        /// </summary>
        /// <value></value>
        public bool? paymentStat { get; set; }
        /// <summary>
        /// 文件與發票信息對應flag
        /// </summary>
        /// <value></value>
        public string flag { get; set; }
        /// <summary>
        /// 發票類型名稱
        /// </summary>
        /// <value></value>
        public string invtype { get; set; }
        /// <summary>
        /// 銷售方稅號
        /// </summary>
        /// <value></value>
        public string salestaxno { get; set; }
        /// <summary>
        /// 銷售方名稱
        /// </summary>
        /// <value></value>
        public string salesname { get; set; }
        public string batchno { get; set; }
        /// <summary>
        /// 購買方稅號
        /// </summary>
        /// <value></value>
        public string buyertaxno { get; set; }
        /// <summary>
        /// 購貨方名稱
        /// </summary>
        /// <value></value>
        public string buyername { get; set; }
        /// <summary>
        /// 價稅合計
        /// </summary>
        /// <value></value>
        //public decimal? tlprice { get; set; }
        /// <summary>
        /// 校驗碼
        /// </summary>
        /// <value></value>
        public string verifycode { get; set; }
        /// <summary>
        /// 銷售方地址
        /// </summary>
        /// <value></value>
        public string salesaddress { get; set; }
        /// <summary>
        /// 銷售方開戶行及賬號
        /// </summary>
        /// <value></value>
        public string salesbank { get; set; }
        /// <summary>
        /// 購買方地址及電話
        /// </summary>
        /// <value></value>
        public string buyeraddress { get; set; }
        /// <summary>
        /// 購買方開戶行及賬號
        /// </summary>
        /// <value></value>
        public string buyerbank { get; set; }
        //public string pwarea { get; set; }
        /// <summary>
        /// 備註
        /// </summary>
        /// <value></value>
        public string remark { get; set; }
        public string drawer { get; set; }
        public string payee { get; set; }
        public string reviewer { get; set; }
        public string machinecode { get; set; }
        public string machineno { get; set; }
        public string verifystat { get; set; }
        public string verifyStateDesc { get; set; }
        public string paymentStatDesc { get; set; }
        public DateTime? padatetime { get; set; }
        /// <summary>
        /// 是否需要补全信息,true为需编辑
        /// </summary>
        /// <value></value>
        //public bool? isfill { get; set; }
        /// <summary>
        /// 在发票池是否有数据
        /// </summary>
        /// <value></value>
        //public bool? existautopa { get; set; }
        /// <summary>
        /// 上傳方式1.全電票 2.鏈接 3.上傳檔案 4.手動輸入
        /// </summary>
        /// <value></value>
        //public int uploadmethod { get; set; }
        /// <summary>
        /// 識別結果消息，無異常則為空
        /// </summary>
        /// <value></value>
        public string msg { get; set; }
        /// <summary>
        /// 税率
        /// </summary>
        /// <value></value>
        public decimal? taxrate { get; set; }
        /// <summary>
        /// 票夹发票id
        /// </summary>
        public Guid? invoiceid { get; set; }
        /// <summary>
        /// 發票異常原因
        /// </summary>
        /// <value></value>
        public string invabnormalreason { get; set; }
        /// <summary>
        /// 文件url
        /// </summary>
        /// <value></value>
        public string fileurl { get; set; }
        /// <summary>
        /// 异常信息描述
        /// </summary>
        /// <value></value>
        public string expinfo { get; set; }
        /// <summary>
        /// 本位幣金額（總金額）
        /// </summary>
        /// <value></value>
        public decimal? baseamt { get; set; }
        /// <summary>
        /// OCR模型model_id
        /// </summary>
        public string? modelid { get; set; }
        /// <summary>
        /// 出發地
        /// </summary>
        public string? startstation { get; set; }
        /// <summary>
        /// 目的地
        /// </summary>
        public string? endstation { get; set; }
        /// <summary>
        /// OCR識別記錄表ID
        /// </summary>
        public string? ocrid { get; set; }
        /// <summary>
        /// invoice pool/ocr/manual
        /// </summary>
        public string? source { get; set; }
        /// <summary>
        /// 存票據上唯一值識別碼
        /// </summary>
        public string? identificationno { get; set; }
        /// <summary>
        /// 發票標題
        /// </summary>
        public string? invoicetitle { get; set; }
        /// <summary>
        /// 營業/增值税基
        /// </summary>
        public decimal? taxbase { get; set; }
        /// <summary>
        /// 進口稅
        /// </summary>
        /// <value></value>
        public decimal? importtaxamount { get; set; }
        /// <summary>
        /// 服務費/推廣貿易服務費
        /// </summary>
        /// <value></value>
        public decimal? servicefee { get; set; }
        /// <summary>
        /// 運輸費
        /// </summary>
        /// <value></value>
        public decimal? shippingfee { get; set; }
        /// <summary>
        /// 手續費
        /// </summary>
        /// <value></value>
        public decimal? transactionfee { get; set; }
        /// <summary>
        /// 數量
        /// </summary>
        /// <value></value>
        public decimal? quantity { get; set; }
        /// <summary>
        /// 商品資訊
        /// </summary>
        public string? productinfo { get; set; }
        /// <summary>
        /// 發票類型
        /// </summary>
        public string? invoicecategory { get; set; }
        /// <summary>
        /// 備註
        /// </summary>
        public string? remarks { get; set; }

        /// <summary>
        /// 課稅別
        /// </summary>
        public string? taxtype { get; set; }
        /// <summary>
        /// 備註
        /// </summary>
        public string? responsibleparty { get; set; }

        /// <summary>
        /// 發票類型代碼
        /// </summary>
        public string? invtypecode { get; set; }

        public string sellerTaxId { get; set; }

        public string filePath { get; set; }

    }
}
