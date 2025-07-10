using System;
namespace ERS.DTO.BDInvoiceFolder
{
    public class UnpaidInvInfoDto
    {
        /// <summary>
        /// 票夾發票id
        /// </summary>
        /// <value></value>
        public Guid? Id { get; set; }
        /// <summary>
        /// 發票代碼
        /// </summary>
        /// <value></value>
        public string invcode { get; set; }
        /// <summary>
        /// 發票號碼
        /// </summary>
        /// <value></value>
        public string invno { get; set; }
        /// <summary>
        /// 發票日期
        /// </summary>
        /// <value></value>
        public DateTime? invdate { get; set; }
        /// <summary>
        /// 開票金額（不含稅）
        /// </summary>
        /// <value></value>
        public decimal amount { get; set; }
        /// <summary>
        /// 稅額
        /// </summary>
        /// <value></value>
        public decimal taxamount { get; set; }
        /// <summary>
        /// 含稅總金額
        /// </summary>
        /// <value></value>
        public decimal oamount { get; set; }
        /// <summary>
        /// 税金损失
        /// </summary>
        /// <value></value>
        public decimal taxloss { get; set; }
        /// <summary>
        /// 幣別
        /// </summary>
        /// <value></value>
        public string curr { get; set; }
        /// <summary>
        /// 本位幣金額（總金額）
        /// </summary>
        /// <value></value>
        public decimal? baseamt { get; set; }
        /// <summary>
        /// 發票類型
        /// </summary>
        /// <value></value>
        public string invtype { get; set; }
        /// <summary>
        /// 校驗狀態
        /// </summary>
        /// <value></value>
        public string verifyStateDesc { get; set; }
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
        /// 請款狀態
        /// </summary>
        /// <value></value>
        public bool? paymentStat { get; set; }
        /// <summary>
        /// 異常原因
        /// </summary>
        /// <value></value>
        public string expdesc { get; set; }
        /// <summary>
        /// 發票文件url
        /// </summary>
        /// <value></value>
        public string fileurl { get; set; }
        /// <summary>
        /// 發票文件路徑
        /// </summary>
        /// <value></value>
        public string filepath { get; set; }
        /// <summary>
        /// 異常描述？
        /// </summary>
        /// <value></value>
        public string expcode { get; set; }
        /// <summary>
        /// 發票狀態
        /// </summary>
        /// <value></value>
        public string invstat { get; set; }
        /// <summary>
        /// 發票描述
        /// </summary>
        /// <value></value>
        public string invdesc { get; set; }
        /// <summary>
        /// 请款状态描述
        /// </summary>
        /// <value></value>
        public string paymentStatDesc { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        /// <value></value>
        public string remark { get; set; }

        /// <summary>
        /// 发票来源
        /// </summary>
        public string source { get; set; }
        /// <summary>
        /// 纳税人识别号
        /// </summary>
        public string identificationno { get; set; }

        /// <summary>
        /// 承担方
        /// </summary>
        public string responsibleparty { get; set; }
    }   
}