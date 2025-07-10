using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [Table("e_form_head")]
    public class EFormHead : BaseEntity
    {
        [Required]
        [StringLength(20)]
        public string formcode { get; set; }
        [Required]
        [StringLength(20)]
        public string rno { get; set; }
        [StringLength(200)]
        public string cname { get; set; }
        [StringLength(20)]
        public string cemplid { get; set; }
        [Column(TypeName = "decimal(8, 2)")]
        public decimal? step { get; set; }
        [StringLength(20)]
        [Comment("approval link application")]
        public string apid { get; set; }
        [StringLength(1)]
        [Comment("status;T:temporary storage,P:process,A:approval,R:reject")]
        public string status { get; set; }
        [StringLength(20)]
        [Comment("Next approval emplid")]
        public string nemplid { get; set; }
        [StringLength(1)]
        [Comment("add user approval,P:之前,N:之後")]
        public string adduser { get; set; }

        [StringLength(1)]
        [Comment("archivestatus归档状态 wishare,N:待归档,Y:已归档")]
        public string archivestatus { get; set; }
        public void SetRno(string rno) => this.rno = rno;
        public void SetFormCode(string formcode) => this.formcode = formcode;
        public void ChangeStatus(string status) => this.status = status;
        public void SetApid(string apid) => this.apid = apid;
        public void InsertData(string cemplid, string cname)
        {
            this.cemplid = cemplid;
            this.cname = cname;
        }
        public void ChangeApidToRequest() => this.apid = this.apid == "RQ204A" ? "RQ201A" : this.apid.TrimEnd('4') + "1";
        public void SetStep(decimal data) => this.step = data;
        public void SetStatus(string data) => this.status = data;
    }
}
