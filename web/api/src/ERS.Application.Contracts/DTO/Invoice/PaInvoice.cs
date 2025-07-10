using System;
#nullable disable
namespace ExpenseApplication.Model.Models
{
    public partial class PaInvoice
    {
        public string Invtype { get; set; }
        public string Batchno { get; set; }
        public string Invcode { get; set; }
        public string Invno { get; set; }
        public string Invdate { get; set; }
        public string Invstat { get; set; }
        public string Salestaxno { get; set; }
        public string Salesname { get; set; }
        public string Buyertaxno { get; set; }
        public string Buyername { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Taxamount { get; set; }
        public decimal? Tlprice { get; set; }
        public string Verifycode { get; set; }
        public string Salesaddress { get; set; }
        public string Salesbank { get; set; }
        public string Buyeraddress { get; set; }
        public string Buyerbank { get; set; }
        public string Pwarea { get; set; }
        public string Remark { get; set; }
        public string Drawer { get; set; }
        public string Payee { get; set; }
        public string Reviewer { get; set; }
        public string Machinecode { get; set; }
        public string Machineno { get; set; }
        public string Verifystat { get; set; }
        public string Paymentstat { get; set; }
        public string Expcode { get; set; }
        public string Modifyreason { get; set; }
        public string Cuser { get; set; }
        public DateTime? Cdate { get; set; }
        public string Muser { get; set; }
        public DateTime? Mdate { get; set; }
        public string Rno { get; set; }
        public string Invdesc { get; set; }
        public string VerifyStateDesc { get; set; }
        public string Expdesc { get; set; }
        public string paymentStatDesc { get; set; }
        public string expcodeDesc { get; set; }
        public decimal? taxrete { get; set; }
    }
}
