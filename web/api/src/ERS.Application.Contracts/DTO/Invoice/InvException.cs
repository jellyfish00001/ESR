#nullable disable
namespace ExpenseApplication.Model.Models
{
    public partial class InvException
    {
        public string invcode { get; set; }
        public string invno { get; set; }
        public string paymentstat { get; set; }//未请款、已请款、Lock
        public string expcode { get; set; }//发票异常原因
        public decimal? taxamount { get; set; }//损失税金
    }
}
