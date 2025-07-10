using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [Table("bd_meal_area")]
    public class BDMealArea : BaseEntity
    {
        [Comment("地区")]
        [StringLength(100)]
        public string area { get; set; }
        [Comment("城市")]
        [StringLength(100)]
        public string city { get; set; }
        [Comment("-1：前；1：后")]
        public int gotype1 { get; set; }
        [Comment("-1：前；1：后")]
        public int gotype2 { get; set; }
        [Comment("出发时间")]
        public DateTime? gotime1 { get; set; }
        [Comment("出发时间")]
        public DateTime? gotime2 { get; set; }
        [Comment("-1：前；1：后")]
        public int backtype1 { get; set; }
        [Comment("-1：前；1：后")]
        public int backtype2 { get; set; }
        [Comment("返回时间")]
        public DateTime? backtime1 { get; set; }
        [Comment("返回时间")]
        public DateTime? backtime2 { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        [Comment("补助金额")]
        public decimal amount { get; set; }
        [StringLength(10)]
        [Comment("币别")]
        public string currency { get; set; }
    }
}
