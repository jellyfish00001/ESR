
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Entities;

namespace ERS.Entities
{
    [Table("bd_company_site")]
    [Index(nameof(CompanyCategory), Name = "bd_company_site_idx_companyCategory")]
    public class BDCompanySite : Entity<Guid>//BasicAggregateRoot<Guid>
    {
        [Required]
        [StringLength(10)]
        [Comment("公司别")]
        [Column("company_category")]
        public string CompanyCategory { get; set; }

        [Required]
        [StringLength(3)]
        [Comment("公司")]
        [Column("company")]
        public string Company { get; set; }

        [Required]
        [StringLength(6)]
        [Comment("工作地")]
        [Column("site")]
        public string Site { get; set; }

        [Required]
        [Comment("主要公司别")]
        [Column("primary")]
        public bool Primary { get; set; }
    }
}