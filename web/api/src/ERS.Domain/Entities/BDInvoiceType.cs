using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/**************************************************************************
 * Author: Andy YA Chung
 * Date: 2025-05-15
 * Name: 發票類型 Entity
 * Description:
 **************************************************************************/

namespace ERS.Entities
{
    [Table("bd_invoice_type")]
    public class BDInvoiceType : BaseEntity
    {
        [Required]
        [StringLength(10)]
        [Column("inv_type_code")]
        [Comment("发票類型代码")]
        public string InvTypeCode { get; set; }

        [Required]
        [StringLength(100)]
        [Column("inv_type")]
        [Comment("发票类型名稱")]
        public string InvType { get; set; }

        [StringLength(30)]
        [Column("seller_tax_id")]
        [Comment("賣方統編/稅號")]
        public string SellerTaxId { get; set; }

        [Required]
        [StringLength(5)]
        [Column("area")]
        [Comment("区域")]
        public string Area { get; set; }

        [Required]
        [StringLength(3)]
        [Column("category")]
        [Comment("分类")]
        public string Category { get; set; }
    }
}