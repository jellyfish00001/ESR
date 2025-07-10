using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities;

namespace ERS.Entities
{
    [Table("data_dictionary_criteria")]
    [Index(nameof(DictionaryId), Name = "data_dictionary_criteria_idx_dictionary_id")]
    public class DataDictionaryCriteria : BasicAggregateRoot<Guid>
    {
        [Required]
        [Comment("data_dictionary_id")]
        [Column("dictionary_id")]
        public Guid DictionaryId { get; set; }

        [Required]
        [StringLength(100)]
        [Comment("條件")]
        [Column("criteria")]
        public string Criteria { get; set; }
             
    }
}