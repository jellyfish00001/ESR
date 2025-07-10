using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [Table("bd_company_category")]
    [Index(nameof(CompanyCategory), Name = "bd_company_category_idx")]
    public class BDCompanyCategory : BaseEntity
    {
        [Required]
        [StringLength(10)]
        [Comment("公司别")]
        [Column("company_category")]
        public string CompanyCategory { get; set; }

        [StringLength(50)]
        [Comment("公司名称")]
        [Column("company_desc")]
        public string CompanyDesc { get; set; }

        [StringLength(10)]
        [Comment("Sap公司代码")]
        [Column("company_sap")]
        public string CompanySap { get; set; }

        [StringLength(6)]
        [Comment("缩写")]
        [Column("stwit")]
        public string Stwit { get; set; }

        [StringLength(6)]
        [Comment("本位币")]
        [Column("base_currency")]
        public string BaseCurrency { get; set; }

        [StringLength(20)]
        [Comment("纳税人识别号")]
        [Column("identification_no")]
        public string IdentificationNo { get; set; }

        [Required]
        [Comment("所得税税率")]
        [Column("income_tax_rate")]
        public decimal IncomeTaxRate { get; set; }

        [Comment("增值税率")]
        [Column("vatrate")]
        public decimal Vatrate { get; set; }

        [StringLength(1)]
        [Comment("状态")]
        [Column("status")]
        public string Status { get; set; }

        [StringLength(3)]
        [Comment("地区")]
        [Column("area")]
        public string Area { get; set; }

        [Required]
        [Comment("时区")]
        [Column("time_zone")]
        public int TimeZone { get; set; }
    }
}
