using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [Table("cash_amount")]
    [Index(nameof(rno), Name = "rno_idx")]
    public class CashAmount : BaseEntity
    {
        [StringLength(20)]
        public string formcode { get; set; }
        [StringLength(20)]
        public string rno { get; set; }
        [StringLength(20)]
        public string currency { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal amount { get; set; }
        [StringLength(1)]
        public string actpay { get; set; } = "N";
        [Column(TypeName = "decimal(18, 2)")]
        public decimal actamt { get; set; }
        public void SetAmount(CashAmount data)
        {
            this.currency = data.currency;
            this.amount = data.amount;
            this.actamt = data.actamt;
        }
        public void SetRno(string rno)
        {
            this.rno = rno;
            this.actpay = "N";
        }
        public void SetPayState(string status) => this.actpay = status;
        public void SetFormCode(string formcode) => this.formcode = formcode;
    }
}
