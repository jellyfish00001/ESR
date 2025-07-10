using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERS.Entities
{
    [Table("bd_expense_department")]
    public class BDExpenseDept : BaseEntity
    {
        [Required]
        [StringLength(20)]
        [Comment("部門代碼")]
        public string deptid { get; set; }

        [Required]
        [StringLength(1)]
        [Comment("是否為虛擬部門")]
        public string isvirtualdept { get; set; }

    }
}