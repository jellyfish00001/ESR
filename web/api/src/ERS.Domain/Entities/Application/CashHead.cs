using ERS.DTO;
using ERS.DTO.Employee;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
namespace ERS.Entities
{
    [Table("cash_head")]
    [Index(nameof(rno), Name = "rno_idx")]
    public class CashHead : BaseEntity
    {
        [StringLength(10)]
        [Comment("base currency")]
        public string formcode { get; set; }
        [StringLength(20)]
        public string rno { get; set; }
        [StringLength(200)]
        [Comment("create user chinese name")]
        public string cname { get; set; }
        [StringLength(10)]
        public string deptid { get; set; }
        [StringLength(20)]
        public string ext { get; set; }
        [StringLength(50)]
        public string projectcode { get; set; }
        [StringLength(10)]
        public string currency { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? amount { get; set; }
        [StringLength(20)]
        public string payeeId { get; set; }
        [StringLength(200)]
        public string payeename { get; set; }
        [StringLength(200)]
        public string payeeaccount { get; set; }
        [StringLength(1)]
        [Comment("welfare fund;Y/N")]
        public string welfare { get; set; }
        public string bank { get; set; }
        [Comment("payment date")]
        public DateTime? payment { get; set; }
        [StringLength(20)]
        [Comment("DOA type")]
        public string dtype { get; set; }
        [Comment("current sign sequence")]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? l2l3seq { get; set; } = 1;
        [StringLength(20)]
        [Comment("SAP company code")]
        public string companysap { get; set; }
        [StringLength(1)]
        [Comment("Status; Y: available, N: Not available")]
        public string stat { get; set; }
        [StringLength(20)]
        [Comment("Original  CURRENCY")]
        public string originalcurrency { get; set; }
        [Comment("whether approve ")]
        [StringLength(20)]
        public string whetherapprove { get; set; }
        [StringLength(200)]
        [Comment("Approve Reason")]
        public string approvereason { get; set; }
        [StringLength(10)]
        [Comment("報銷類型 F01:週轉金報銷 or F02:廠商報銷")]
        public string expensetype { get; set; }
        [StringLength(20)]
        [Comment("廠商")]
        public string vendor { get; set; }
        [Comment("付款日")]
        public DateTime? paymentdate { get; set; }
        [StringLength(100)]
        [Comment("週轉金報銷 or 廠商報銷")]
        public string expensetypedesc { get; set; }
        [StringLength(100)]
        [Comment("廠商名稱")]
        public string vendordesc { get; set; }
        [StringLength(100)]
        [Comment("統一編碼")]
        public string unifycode { get; set; }
        [StringLength(1)]
        [Comment("是否是雲端電子發票")]
        public string iscloudinvoice { get; set; } = "N";
        [StringLength(1)]
        [Comment("是否暂存(Y/N)")]
        public string istemp { get; set; } = "N";
        [Comment("预支延期申请单号")]
        [StringLength(50)]
        public string overduerno { get; set; }
        [Comment("预支延期申请单号状态 status;T:temporary storage,P:process,A:approval,R:reject")]
        [StringLength(1)]
        public string overduestatus { get; set; }
        [Comment("逾期天数")]
        public int overdueday { get; set; }
        [Comment("逾期次数")]
        public int overduesum { get; set; }
        [Comment("逾期原因")]
        [StringLength(500)]
        public string overduereason { get; set; }
        [NotMapped]
        public IList<CashDetail> CashDetailList { get; set; }
        [NotMapped]
        public IList<CashFile> CashFileList { get; set; }
        [NotMapped]
        public IList<Invoice> InvoiceList { get; set; }
        [NotMapped]
        public CashAmount CashAmount { get; set; }
        [NotMapped]
        public EFormHead EFormHead { get; set; }
        [NotMapped]
        public int timeZone { get; set; } = 8;
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? actualamount { get; set; }
        [Comment("申請總計金額")]
        [Column("request_amount", TypeName = "decimal(18, 2)")]
        public decimal? requestamount { get; set; }
        [Comment("扣減金額(個人承擔稅金金額)")]
        [Column("deduction_amount", TypeName = "decimal(18, 2)")]
        public decimal? deductionamount { get; set; }
        [Comment("状态")]
        [StringLength(50)]
        public string? status { get; set; }

        [StringLength(50)]
        [Column("business_trip_no")]
        [Comment("出差单号")]
        public string BusinessTripNo { get; set; }

        [StringLength(20)]
        [Column("program")]
        [Comment("Uber出行方案")]
        public string Program { get; set; }
        [StringLength(20)]
        [Column("payment_way")]
        [Comment("付款方式")]
        public string PaymentWay { get; set; }

        [StringLength(10)]
        [Column("advance_status")]
        [Comment("预支金状态")]
        public string AdvanceStatus { get; set; }
        public void SetCash1(CashHead data, string cuser, int timeZone = 8)
        {
            this.formcode = "CASH_1";
            this.iscloudinvoice = "N";
            this.unifycode = null;
            this.vendordesc = null;
            this.expensetypedesc = null;
            this.paymentdate = null;
            this.vendor = null;
            this.expensetype = null;
            // this.approvereason = null;
            // this.whetherapprove = null;
            this.l2l3seq = 1;
            this.payment = null;
            this.istemp = "N";
            this.SetCUser(cuser);
            this.SetCDate(timeZone);
            this.timeZone = timeZone;
            this.EFormHead = new EFormHead();
            this.EFormHead.SetFormCode(this.formcode);
            this.EFormHead.SetCompany(this.company);
            this.EFormHead.ChangeStatus("P");
            this.EFormHead.SetApid("RQ104");
            this.EFormHead.InsertData("", this.cname);
            this.EFormHead.SetCUser(cuser);
            this.EFormHead.SetCDate(timeZone);
        }
        public void SetCash2(CashHead data, string cuser, int timeZone = 8)
        {
            this.formcode = "CASH_2";
            this.dtype = "A1";
            this.iscloudinvoice = "N";
            this.unifycode = null;
            this.vendordesc = null;
            this.expensetypedesc = null;
            this.paymentdate = null;
            this.vendor = null;
            this.expensetype = null;
            // this.approvereason = null;
            // this.whetherapprove = null;
            this.l2l3seq = 1;
            this.payment = null;
            this.istemp = "N";
            this.SetCUser(cuser);
            this.SetCDate(timeZone);
            this.timeZone = timeZone;
            this.EFormHead = new EFormHead();
            this.EFormHead.SetFormCode(this.formcode);
            this.EFormHead.SetCompany(this.company);
            this.EFormHead.ChangeStatus("P");
            this.EFormHead.SetApid("RQ204");
            this.EFormHead.InsertData("", this.cname);
            this.EFormHead.SetCUser(cuser);
            this.EFormHead.SetCDate(timeZone);
        }
        public void SetCash2A(CashHead data, string cuser, int timeZone = 8)
        {
            this.formcode = "CASH_2";
            this.dtype = "A1";
            this.iscloudinvoice = "N";
            this.unifycode = null;
            this.vendordesc = null;
            this.expensetypedesc = null;
            this.paymentdate = null;
            this.vendor = null;
            this.expensetype = null;
            //this.approvereason = null;
            //this.whetherapprove = null;
            this.l2l3seq = 1;
            this.payment = null;
            this.istemp = "N";
            this.SetCUser(cuser);
            this.SetCDate(timeZone);
            this.timeZone = timeZone;
            this.EFormHead = new EFormHead();
            this.EFormHead.SetFormCode(this.formcode);
            this.EFormHead.SetCompany(this.company);
            this.EFormHead.ChangeStatus("P");
            this.EFormHead.SetApid("RQ204A");
            this.EFormHead.InsertData("", this.cname);
            this.EFormHead.SetCUser(cuser);
            this.EFormHead.SetCDate(timeZone);
        }
        public void SetCash3(CashHead data, string cuser, int timeZone = 8)
        {
            this.formcode = "CASH_3";
            this.iscloudinvoice = "N";
            this.unifycode = null;
            this.vendordesc = null;
            this.expensetypedesc = null;
            this.paymentdate = null;
            this.vendor = null;
            this.expensetype = null;
            //this.approvereason = null;
            //this.whetherapprove = null;
            this.l2l3seq = 1;
            this.payment = null;
            this.istemp = "N";
            this.SetCUser(cuser);
            this.SetCDate(timeZone);
            this.timeZone = timeZone;
            this.EFormHead = new EFormHead();
            this.EFormHead.SetFormCode(this.formcode);
            this.EFormHead.SetCompany(this.company);
            this.EFormHead.ChangeStatus("P");
            this.EFormHead.SetApid("RQ404");
            this.EFormHead.InsertData("", this.cname);
            this.EFormHead.SetCUser(cuser);
            this.EFormHead.SetCDate(timeZone);
        }
        public void SetCash3A(CashHead data, string cuser, int timeZone = 8)
        {
            this.formcode = "CASH_3A";
            this.unifycode = null;
            this.vendordesc = null;
            this.expensetypedesc = null;
            this.paymentdate = null;
            this.vendor = null;
            this.expensetype = null;
            //this.approvereason = null;
            //this.whetherapprove = null;
            this.payment = null;
            this.istemp = "N";
            this.SetCUser(cuser);
            this.SetCDate(timeZone);
            this.timeZone = timeZone;
            this.EFormHead = new EFormHead();
            this.EFormHead.SetFormCode(this.formcode);
            this.EFormHead.SetCompany(this.company);
            this.EFormHead.ChangeStatus("P");
            this.EFormHead.SetApid("RQ404A");
            this.EFormHead.InsertData("", this.cname);
            this.EFormHead.SetCUser(cuser);
            this.EFormHead.SetCDate(timeZone);
        }
        public void SetCash4(CashHead data, string cuser, int timeZone = 8)
        {
            this.formcode = "CASH_4";
            this.iscloudinvoice = "N";
            this.unifycode = null;
            this.vendordesc = null;
            this.expensetypedesc = null;
            this.paymentdate = null;
            this.vendor = null;
            this.expensetype = null;
            // this.approvereason = null;
            // this.whetherapprove = null;
            this.l2l3seq = 1;
            this.payment = null;
            this.istemp = "N";
            this.SetCUser(cuser);
            this.SetCDate(timeZone);
            this.timeZone = timeZone;
            this.EFormHead = new EFormHead();
            this.EFormHead.SetFormCode(this.formcode);
            this.EFormHead.SetCompany(this.company);
            this.EFormHead.ChangeStatus("P");
            this.EFormHead.SetApid("RQ504");
            this.EFormHead.InsertData("", this.cname);
            this.EFormHead.SetCUser(cuser);
            this.EFormHead.SetCDate(timeZone);
        }
        public void SetCash5(CashHead data, string cuser, int timeZone = 8)
        {
            this.formcode = "CASH_5";
            this.iscloudinvoice = "N";
            this.unifycode = null;
            this.vendordesc = null;
            this.expensetypedesc = null;
            this.paymentdate = null;
            this.vendor = null;
            this.expensetype = null;
            this.l2l3seq = 1;
            this.payment = null;
            this.istemp = "N";
            this.SetCUser(cuser);
            this.SetCDate(timeZone);
            this.timeZone = timeZone;
            this.EFormHead = new EFormHead();
            this.EFormHead.SetFormCode(this.formcode);
            this.EFormHead.SetCompany(this.company);
            this.EFormHead.ChangeStatus("P");
            this.EFormHead.SetApid("RQ804");
            this.EFormHead.InsertData("", this.cname);
            this.EFormHead.SetCUser(cuser);
            this.EFormHead.SetCDate(timeZone);
        }
        public void SetCash6(CashHead data, string cuser, int timeZone = 8)
        {
            this.formcode = "CASH_6";
            this.iscloudinvoice = "N";
            this.unifycode = null;
            this.vendordesc = null;
            this.expensetypedesc = null;
            this.paymentdate = null;
            this.vendor = null;
            this.expensetype = null;
            this.l2l3seq = 1;
            this.payment = null;
            this.istemp = "N";
            this.SetCUser(cuser);
            this.SetCDate(timeZone);
            this.timeZone = timeZone;
            this.EFormHead = new EFormHead();
            this.EFormHead.SetFormCode(this.formcode);
            this.EFormHead.SetCompany(this.company);
            this.EFormHead.ChangeStatus("P");
            this.EFormHead.SetApid("RQ604");
            this.EFormHead.InsertData("", this.cname);
            this.EFormHead.SetCUser(cuser);
            this.EFormHead.SetCDate(timeZone);
        }
        public void SetCashX(CashHead data, string cuser, int timeZone = 8)
        {
            this.formcode = "CASH_X";
            this.iscloudinvoice = "N";
            this.unifycode = null;
            this.vendordesc = null;
            this.expensetypedesc = null;
            this.paymentdate = null;
            this.vendor = null;
            this.expensetype = null;
            this.l2l3seq = 1;
            this.payment = null;
            this.istemp = "N";
            this.dtype = "B1";
            this.SetCUser(cuser);
            this.SetCDate(timeZone);
            this.timeZone = timeZone;
            this.EFormHead = new EFormHead();
            this.EFormHead.SetFormCode(this.formcode);
            this.EFormHead.SetCompany(this.company);
            this.EFormHead.ChangeStatus("P");
            this.EFormHead.SetApid("RQ704");
            this.EFormHead.InsertData("", this.cname);
            this.EFormHead.SetCUser(cuser);
            this.EFormHead.SetCDate(timeZone);
        }
        public void AddCashFile(CashFile cashFile) => this.CashFileList.Add(cashFile);
        public void Setdtype(string dtype)
        {
            this.dtype = dtype;
        }
        public void SetOriginalFileNameAndSavePath(string key, string path, string fileName, string fileType)
        {
            var temp = this.CashFileList.Where(i => i.key == key).FirstOrDefault();
            if (temp != null) temp.SetSavePath(path);
            if (temp != null) temp.SetOriginalFileName(fileName);
            if (temp != null) temp.SetFileType(fileType);
        }
        public void SetOriginalFileNameAndSavePathByInvoiceFolder(Guid key, string path, string fileName)
        {
            var temp = this.CashFileList.Where(i => i.invoiceid == key).FirstOrDefault();
            if (temp != null) temp.SetSavePath(path);
            if (temp != null) temp.SetOriginalFileName(fileName);
        }
        public void SetRno(string rno)
        {
            this.rno = rno;
            foreach (CashDetail item in this.CashDetailList)
                item.SetRno(rno);
            if (this.InvoiceList != null)
            {
                foreach (Invoice item in this.InvoiceList)
                    item.SetRno(rno);
            }
            if (this.CashFileList != null)
            {
                foreach (CashFile item in this.CashFileList)
                    item.SetRno(rno);
            }
            if (this.CashAmount != null)
                this.CashAmount.SetRno(rno);
            if (this.EFormHead != null)
                this.EFormHead.SetRno(rno);
        }
        public void SetDeptId(string deptid)
        {
            this.deptid = deptid;
            foreach (CashDetail item in this.CashDetailList)
                item.SetDeptId(deptid);
        }
        public void SetKeepStatus(string apid)
        {
            this.istemp = "Y";
            foreach (CashDetail item in this.CashDetailList)
                item.SetKeepStatus();
            if (this.EFormHead != null)
            {
                this.EFormHead.ChangeStatus("T");
                this.EFormHead.SetApid(apid);
            }
        }
        public bool CheckIsExistRno() => !string.IsNullOrEmpty(this.rno);
        public void UpdateEFormHead(EFormHead data) => this.EFormHead = data;
        public void AddCash1Detail(IList<CashDetail> data)
        {
            this.CashDetailList = data;
            foreach (CashDetail item in this.CashDetailList)
            {
                item.SetCash1(item);
                item.SetCompany(this.company);
                item.SetFormCode(this.formcode);
                item.SetCUser(this.cuser);
                item.SetCDate(this.timeZone);
            }
        }
        public void AddCash2Detail(IList<CashDetail> data)
        {
            this.CashDetailList = data;
            foreach (CashDetail item in this.CashDetailList)
            {
                item.SetCash2(item);
                item.SetCompany(this.company);
                item.SetFormCode(this.formcode);
                item.SetCUser(this.cuser);
                item.SetCDate(this.timeZone);
            }
        }
        public void AddCash2ADetail(IList<CashDetail> data)
        {
            this.CashDetailList = data;
            foreach (CashDetail item in this.CashDetailList)
            {
                item.SetCash2A(item);
                item.SetCompany(this.company);
                item.SetFormCode(this.formcode);
                item.SetCUser(this.cuser);
                item.SetCDate(this.timeZone);
            }
        }
        public void AddCash3Detail(IList<CashDetail> data)
        {
            this.CashDetailList = data;
            foreach (CashDetail item in this.CashDetailList)
            {
                item.SetCash3(item);
                item.SetCompany(this.company);
                item.SetFormCode(this.formcode);
                item.SetCUser(this.cuser);
                item.SetCDate(this.timeZone);
            }
        }
        public void AddCash3ADetail(IList<CashDetail> data)
        {
            this.CashDetailList = data;
            foreach (CashDetail item in this.CashDetailList)
            {
                item.SetCash3A(item);
                item.SetCompany(this.company);
                item.SetFormCode(this.formcode);
                item.SetCUser(this.cuser);
                // 不设置时间，此为预支单产生日期
                //item.SetCDate(this.timeZone);
            }
        }
        public void AddCash4Detail(IList<CashDetail> data)
        {
            this.CashDetailList = data;
            foreach(CashDetail item in this.CashDetailList)
            {
                item.SetCash4(item);
                item.SetCompany(this.company);
                item.SetFormCode(this.formcode);
                item.SetCUser(this.cuser);
                item.SetCDate(this.timeZone);
            }
        }
        public void AddCash5Detail(IList<CashDetail> data)
        {
            this.CashDetailList = data;
            foreach (CashDetail item in this.CashDetailList)
            {
                item.SetCash5(item);
                item.SetCompany(this.company);
                item.SetFormCode(this.formcode);
                item.SetCUser(this.cuser);
                item.SetCDate(this.timeZone);
            }
        }
        public void AddCash6Detail(IList<CashDetail> data)
        {
            this.CashDetailList = data;
            foreach (CashDetail item in this.CashDetailList)
            {
                item.SetCash6(item);
                item.SetCompany(this.company);
                item.SetFormCode(this.formcode);
                item.SetCUser(this.cuser);
                item.SetCDate(this.timeZone);
            }
        }
        public void AddCashXDetail(IList<CashDetail> data)
        {
            this.CashDetailList = data;
            foreach (CashDetail item in this.CashDetailList)
            {
                item.SetCashX(item);
                item.SetCompany(this.company);
                item.SetCompanyCode(item.companycode);
                item.SetFormCode(this.formcode);
                item.SetCUser(this.cuser);
                item.SetCDate(this.timeZone);
            }
        }
        public bool CheckIsExistInvoice()
        {
            return this.formcode == "CASH_4" ? this.CashFileList.Where(i => i.ishead == "Y" && i.status == "I").Count() > 0 : this.CashFileList.Where(i => i.ishead == "N" && i.status != "F").Count() > 0;
        }
        public void AddFile(IList<CashFile> data)
        {
            this.CashFileList = data;
            foreach (CashFile item in this.CashFileList)
            {
                item.SetCompany(this.company);
                item.SetFormCode(this.formcode);
                item.SetCUser(this.cuser);
                item.SetCDate(this.timeZone);
            }
        }
        public void AddInvoice(IList<Invoice> data)
        {
            this.InvoiceList = data;
            foreach (Invoice item in this.InvoiceList)
            {
                item.SetCompany(this.company);
                item.SetCUser(this.cuser);
                item.SetCDate(this.timeZone);
            }
        }
        public void AddCashAmount(CashAmount data)
        {
            this.CashAmount = data;
            this.CashAmount.SetCompany(company);
            this.CashAmount.SetFormCode(this.formcode);
            this.CashAmount.SetPayState("N");
            this.CashAmount.SetCUser(this.cuser);
            this.CashAmount.SetCDate(this.timeZone);
        }
        public void SetPayeeAccount(CashAccountDto data)
        {
            this.payeeaccount = data.account;
            this.bank = data.bank;
        }
        public void SetDetailExp(ExpAcctDto data)
        {
            foreach (CashDetail item in this.CashDetailList) item.SetExp(data);
        }
        public void SetPayment(DateTime data) => this.payment = data;

        public void ChangeStatus(string status) => this.status = status;

    }
}
