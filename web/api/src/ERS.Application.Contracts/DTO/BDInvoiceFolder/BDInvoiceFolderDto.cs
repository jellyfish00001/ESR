using System;
namespace ERS.DTO.BDInvoiceFolder
{
    public class BDInvoiceFolderDto
    {
        public Guid Id { get; set; }

        /// <summary>
        /// 買方名稱
        /// </summary>
        public string buyername { get; set; }
        
        /// <summary>
        /// 購買方稅號
        /// </summary>
        /// <value></value>
        public string buyertaxno { get; set; }
        /// <summary>
        /// 销售方税号
        /// </summary>
        /// <value></value>
        public string salestaxno { get; set; }
        /// <summary>
        /// 发票代码
        /// </summary>
        /// <value></value>
        public string invcode { get; set; }
        /// <summary>
        /// 发票号码
        /// </summary>
        /// <value></value>
        public string invno { get; set; }
        /// <summary>
        /// 发票类型
        /// </summary>
        /// <value></value>
        public string invtype { get; set; }
        /// <summary>
        /// 发票日期
        /// </summary>
        /// <value></value>
        public DateTime? invdate { get; set; }
        /// <summary>
        /// 不含税金额
        /// </summary>
        /// <value></value>
        public decimal amount { get; set; }
        /// <summary>
        /// 税额
        /// </summary>
        /// <value></value>
        public decimal taxamount { get; set; }
        /// <summary>
        /// 含税总金额
        /// </summary>
        /// <value></value>
        public decimal oamount { get; set; }
        /// <summary>
        /// 发票核验状态
        /// </summary>
        /// <value></value>
        public string verifytype { get; set; }
        /// <summary>
        /// 发票请款状态
        /// </summary>
        /// <value></value>
        public string paytype { get; set; }
        /// <summary>
        /// 异常原因
        /// </summary>
        /// <value></value>
        public string abnormalreason { get; set; }
        /// <summary>
        /// ERS报销单号
        /// </summary>
        /// <value></value>
        public string rno { get; set; }
        /// <summary>
        /// 发票路径
        /// </summary>
        /// <value></value>
        public string filepath { get; set; }
        public string emplid { get; set; }
        /// <summary>
        /// 申请人
        /// </summary>
        /// <value></value>
        public string cuser { get; set; }
        /// <summary>
        /// 是否补全信息
        /// </summary>
        /// <value></value>
        public bool isfill { get; set; }
        /// <summary>
        /// 发票文件url
        /// </summary>
        /// <value></value>
        public string url { get; set; }
        /// <summary>
        /// 幣別
        /// </summary>
        /// <value></value>
        public string curr { get; set; }
        /// <summary>
        /// 創建時間
        /// </summary>
        /// <value></value>
        public DateTime? cdate { get; set; }
        /// <summary>
        /// 税率
        /// </summary>
        /// <value></value>
        public decimal? taxrate { get; set; }
        /// <summary>
        /// 備註
        /// </summary>
        /// <value></value>
        public string remark { get; set; }

        /// <summary>
        /// 發票類型：A,B,C,D,E
        /// </summary>
        public string invoicecategory { get; set; }

        /// <summary>
        /// 出發站
        /// </summary>
        public string startstation { get; set; }
        /// <summary>
        /// 抵達站
        /// </summary>
        public string endstation { get; set; }

        /// <summary>
        /// 發票標題
        /// </summary>
        public string invoicetitle { get; set; }

        /// <summary>
        /// 營業/增值税基
        /// </summary>
        public decimal? taxbase { get; set; }

        /// <summary>
        /// 進口稅
        /// </summary>
        public decimal? importtaxamount { get; set; }

        /// <summary>
        /// 服務費/推廣貿易服務費
        /// </summary>
        public decimal? servicefee { get; set; }

        /// <summary>
        /// 運輸費
        /// </summary>
        public decimal? shippingfee { get; set; }

        /// <summary>
        /// 手續費
        /// </summary>
        public decimal? transactionfee { get; set; }

        /// <summary>
        /// 數量
        /// </summary>
        public decimal? quantity { get; set; }
        public string productinfo { get; set; }

        public string remarks { get; set; }

        /// <summary>
        /// 承擔方
        /// </summary>
        public string responsibleparty { get; set; }

        /// <summary>
        /// 課稅別
        /// </summary>
        public string taxtype { get; set; }
        public string ocrid { get; set; }
        public string source { get; set; }

        public string region { get; set; }
    }
}