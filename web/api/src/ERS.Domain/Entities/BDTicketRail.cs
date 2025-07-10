using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    //车票字轨
    [Table("bd_ticket_rail")]
    public class BDTicketRail : BaseEntity
    {
        [Required]
        [StringLength(4)]
        [Comment("车票字轨")]
        public string ticketrail { get; set; }
        [StringLength(2)]
        [Comment("凭证年份")]
        public string voucheryear { get; set; }
        [StringLength(2)]
        [Comment("凭证月份")]
        public string vouchermonth { get; set; }

        [StringLength(4)]
        [Comment("当前序号")]
        public string currentnumber { get; set; }

    }
}
