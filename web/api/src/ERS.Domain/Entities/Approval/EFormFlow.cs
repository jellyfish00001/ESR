using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [Table("e_form_flow")]
    public class EFormFlow : BaseEntity
    {
        [Required]
        [StringLength(20)]
        public string formcode { get; set; }
        [Required]
        [StringLength(1)]
        public string formstart { get; set; }
        [Column(TypeName = "decimal(8, 2)")]
        public decimal step { get; set; }
        public string stepname { get; set; }
        [Required]
        [StringLength(200)]
        [Comment("supervisor role,L1:課級,L2:部級,L3:廠處級,A:?單人,B:接收人,C:移出人,SQL,ALL部?的全部人")]
        public string role { get; set; }
        [Required]
        [StringLength(20)]
        [Comment("supervisor role type,DEPT:部門,EMPLID:工號,BD:BaseData")]
        public string roletype { get; set; }
        [StringLength(1)]
        [Comment("joint signature,N:任一簽核人通過就到下一關,Y:所有人都要簽")]
        public string joint { get; set; } = "N";
        [StringLength(1)]
        [Comment("add supervisor")]
        public string addsuper { get; set; } = "N";
        [StringLength(200)]
        [Comment("next step")]
        public string nstep { get; set; }
    }
}
