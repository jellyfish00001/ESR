using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities;
namespace ERS.Entities.Approval
{
    /// <summary>
    /// 汇率（新）
    /// </summary>
    [Table("sap_exrate")]
    public class SAPExRate : BasicAggregateRoot<Guid>
    {
        [Required]
        public string batch_id { get; set; }
        [Required]
        public int version_id { get; set; }
        /// <summary>
        /// 汇率类型
        /// </summary>
        /// <value></value>
        public string kurst { get; set; }
        /// <summary>
        /// 来源币别
        /// </summary>
        /// <value></value>
        public string fcurr { get; set; }
        /// <summary>
        /// 目的幣別
        /// </summary>
        /// <value></value>
        public string tcurr { get; set; }
        public string gdatu { get; set; }
        /// <summary>
        /// 有效起止日期
        /// </summary>
        /// <value></value>
        public DateTime? date_fm { get; set; }
        /// <summary>
        /// 有效终止日期
        /// </summary>
        /// <value></value>
        public DateTime? date_to { get; set; }
        /// <summary>
        /// 汇率
        /// </summary>
        /// <value></value>
        public decimal? ukurs { get; set; }
        public int ffact { get; set; }
        public int tfact { get; set; }
    }
}