using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
namespace ERS.Model
{
    public class Signs
    {
        public string formno { get; set; }
        public int seq { get; set; }
        public string emplid { get; set; }
        public string system_code { get; } = "ers";
        public string step { get; set; }
        public string form_code { get; set; }
        public string step_emplid { get; set; }
        public string sign_way { get; set; } = "and";
        public string status { get; set; } = "N";
        public string cuser { get; set; }
        public Signs()
        {
        }
        public void SetCUser(string cuser)
        {
            this.cuser = cuser;
            this.step_emplid = cuser;
        }
        public void SetCurrSign(string emplid, string signStep, int seq)
        {
            this.emplid = emplid;
            this.step = signStep;
            this.seq = seq;
        }
        public Signs(string emplid, string signStep, int seq)
        {
            this.emplid = emplid;
            this.step = signStep;
            this.seq = seq;
        }
    }
    public class CreateSign
    {
        public string systemCode { get; } = "ers";
        public bool sendMail { get; set; } = true;
        public IList<Signs> signs { get; set; } = new List<Signs>();
        public string formno { get; set; }
        public string formCode { get; set; }
        public string mailMsg { get; set; }
        public IList<string> mailTo { get; set; }
        public string subject { get; set; }
        public string remark { get; set; }
        public string reason { get; set; }
        public string fromEmplid { get; set; }
        public string toEmplid { get; set; }
        public string userid { get; set; }
        public string inviteOrder { get; set; }
        public string inviteEmplid { get; set; }
        public string step { get; set; }
        public void SetFormCode(string formCode)
        {
            this.formCode = formCode;
            foreach(var sign in signs)
                sign.form_code = formCode;
        }
        public void SetRno(string rno)
        {
            this.formno = rno;
            foreach (var sign in signs)
                sign.formno = rno;
        }
        public void SetRemark(string data) => this.remark = data;
        public void SetSignUser(string user) => this.userid = user;
        public void SetSignListCUser(string user)
        {
            foreach(var item in this.signs)
                item.SetCUser(user);
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="method">-1:之前；1：之后</param>
        /// <param name="user">工号</param>
        public void SetInvite(int? method, string user)
        {
            if (method == null && method == 0) return;
            if (method == -1)
            {
                this.inviteOrder = "BeforeInvite";
                this.step = "Invite(Before)";
            }
            if (method == 1)
            {
                this.inviteOrder = "AfterInvite";
                this.step = "Invite(After)";
            }
            this.inviteEmplid = user;
        }
        public void SetCurrApproval()
        {
            var value = this.signs.First();
            value.status = "C";
        }
        public void SetMailTo(IList<string> data) => this.mailTo = data;
        public void SetMailSubject(string data) => this.subject = data;
        public void SetMailMsg(string data) => this.mailMsg = data;
    }
    public class SignsComparer : IEqualityComparer<Signs>
    {
        public bool Equals(Signs x, Signs y)
        {
            if (x == null || y == null) return false;
            return x.step == y.step && x.emplid == y.emplid;
        }
        public int GetHashCode([DisallowNull] Signs obj)
        {
            if (obj == null) return 0;
            return obj.step.GetHashCode() ^ obj.emplid.GetHashCode();
        }
    }
}
