using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [Table("bd_taxrate")]
    public class BDTaxRate : BaseEntity
    {
        /// <summary>
        /// 税率
        /// </summary>
        /// <value></value>
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? taxrate { get; set; }
        /// <summary>
        /// sap代码
        /// </summary>
        /// <value></value>
        public string sapcode { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        /// <value></value>
        public string description { get; set; }
    }
}