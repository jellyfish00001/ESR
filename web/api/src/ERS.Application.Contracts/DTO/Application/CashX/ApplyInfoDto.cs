using System;
namespace ERS.DTO.Application.CashX
{
    public class ApplyInfoDto
    {
        /// <summary>
        /// 公司别
        /// </summary>
        /// <value></value>
        public string companycode { get; set; }
        /// <summary>
        /// 报销情景
        /// </summary>
        /// <value></value>
        public XZExpDto expinfo { get; set; }
        /// <summary>
        /// 需款日期
        /// </summary>
        /// <value></value>
        public DateTime? reqdate { get; set; }
        /// <summary>
        /// 薪资所属期
        /// </summary>
        /// <value></value>
        public string salarydate { get; set; }
        /// <summary>
        /// 银行
        /// </summary>
        /// <value></value>
        public string bank { get; set; }
        /// <summary>
        /// 请款方式
        /// </summary>
        /// <value></value>
        public PayWayDto payway { get; set; }
        /// <summary>
        /// 币别
        /// </summary>
        /// <value></value>
        public string currency { get; set; }
        /// <summary>
        /// 申请金额
        /// </summary>
        /// <value></value>
        public decimal? amount { get; set; }
        /// <summary>
        /// 生成的摘要
        /// </summary>
        /// <value></value>
        public string remarks { get; set; }
    }
}