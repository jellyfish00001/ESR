using System;
using System.ComponentModel.DataAnnotations;
namespace ERS.DTO.Application
{
    public class CashHeadDto
    {
        public string formcode { get; set; }
        public string rno { get; set; }
        [Required]
        public string cname { get; set; }
        [Required]
        public string deptid { get; set; }
        public string ext { get; set; }
        [Required]
        public string company { get; set; }
        public string projectcode { get; set; }
        public string costdeptid { get; set; }
        public string currency { get; set; }
        public decimal? amount { get; set; }
        [Required]
        public string payeeId { get; set; }
        [Required]
        public string payeename { get; set; }
        public string payeeaccount { get; set; }
        public string welfare { get; set; }
        public string bank { get; set; }
        public DateTime? payment { get; set; }
        public string dtype { get; set; }
        public decimal? l2l3seq { get; set; }
        public string companysap { get; set; }
        public string stat { get; set; }
        public string originalcurrency { get; set; }
        // Y/N
        public string whetherapprove { get; set; }//宴客前是否已事先取得權責主管核准?
        public string approvereason { get; set; }//请说明原因
        public string expensetype { get; set; }
        public string vendor { get; set; }
        public DateTime? paymentdate { get; set; }
        public string expensetypedesc { get; set; }
        public string vendordesc { get; set; }
        public string unifycode { get; set; }
        public string iscloudinvoice { get; set; }
        public string istemp { get; set; }
        // 预支延期申请单号
        public string overduerno { get; set; }
        // 逾期天数
        public int overdueday { get; set; }
        // 逾期次数
        public int overduesum { get; set; }
        // 逾期原因
        public string overduereason { get; set; }
        public DateTime? cdate { get; set; }
        public string cuser { get; set; }

        public decimal requestAmount { get; set; }

        public decimal deductionAmount { get; set; }

        public decimal actualamount { get; set; }

        public string apid { get; set; }

        public string status { get; set; }
        //付款方式
        public string paymentway { get; set; }
        //预支金状态
        public string advancestatus { get; set; }
    }
    public class Query
    {
        public string rno { get; set; }
    }
}
