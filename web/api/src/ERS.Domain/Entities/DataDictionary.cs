using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERS.Entities
{
    [Table("data_dictionary")]
    [Index(nameof(Category), Name = "data_dictionary_idx_category")]
    public class DataDictionary : SuperBaseEntity
    {
        [Required]
        [StringLength(100)]
        [Comment("類別")]
        [Column("category")]
        public string Category { get; set; }

        [Required]
        [StringLength(100)]
        [Comment("值")]
        [Column("value")]
        public string Value { get; set; }

        [Required]
        [StringLength(100)]
        [Comment("名稱")]
        [Column("name")]
        public string Name { get; set; }

        [StringLength(100)]
        [Comment("名稱(繁中)")]
        [Column("name_zhtw")]
        public string NameZhtw { get; set; }

        [StringLength(100)]
        [Comment("名稱(簡中)")]
        [Column("name_zhcn")]
        public string NameZhcn { get; set; }

        [StringLength(100)]
        [Comment("名稱(越語)")]
        [Column("name_vn")]
        public string NameVn { get; set; }

        [StringLength(100)]
        [Comment("名稱(西語)")]
        [Column("name_es")]
        public string NameEs { get; set; }

        [StringLength(100)]
        [Comment("名稱(捷克語)")]
        [Column("name_cz")]
        public string NameCz { get; set; }

        [Required]
        [Comment("排序序號")]
        [Column("sort_order")]
        public int SortOrder { get; set; }

        [StringLength(100)]
        [Comment("說明")]
        [Column("description")]
        public string Description { get; set; }
    }
}