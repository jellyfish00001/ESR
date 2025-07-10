using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
namespace ERS.Entities
{
    [Table("bd_exp_identification")]
    public class BDExpID : BaseEntity
    {
        [Comment("費用代碼")]
        [StringLength(20)]
        public string expcode { get; set; }
        [Comment("會計科目代碼")]
        [StringLength(50)]
        public string acctcode { get; set; }
        [Comment("公司別代碼")]
        public string companycode { get; set; }
        [Comment("銀行別 （付款方式現金：Cash）")]
        public string bank { get; set; }
        [Comment("入賬 identification")]
        public string identification { get; set; }
    }
}