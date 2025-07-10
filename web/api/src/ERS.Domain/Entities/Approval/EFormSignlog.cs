using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities;
[Table("e_form_signlog")]
public class EFormSignlog : BaseEntity
{
    [Required]
    [StringLength(20)]
    public string rno { get; set; }
    [Column(TypeName = "decimal(8, 2)")]
    public decimal seq { get; set; }
    [Required]
    [StringLength(20)]
    [Comment("approval emplid")]
    public string aemplid { get; set; }
    [StringLength(200)]
    public string astepname { get; set; }
    [Comment("approval date")]
    public DateTime adate { get; set; }
    [Required]
    [StringLength(20)]
    public string aresult { get; set; }
    [StringLength(500)]
    public string aremark { get; set; }
    [Required]
    [StringLength(20)]
    public string formcode { get; set; }
    [Column(TypeName = "decimal(8, 2)")]
    public decimal step { get; set; }
    [StringLength(20)]
    public string cemplid { get; set; }
    [StringLength(100)]
    public string aname { get; set; }
    [StringLength(100)]
    public string aename { get; set; }
    [StringLength(1)]
    [Comment("Y:add user approval log")]
    public string isadduser { get; set; }
    [StringLength(20)]
    public string deptid { get; set; }
}