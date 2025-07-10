using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERS.Entities
{
    [Table("e_autono")]
    public class EAutono : BaseEntity
    {
        [StringLength(20)]
        public string date { get; set; }
        [StringLength(20)]
        public string no { get; set; }
        [StringLength(20)]
        public string formcode { get; set; }
        public EAutono()
        {
            this.company = "ALL";
        }
        public EAutono(string form_code)
        {
            this.formcode = form_code;
            this.company = "ALL";
            this.date = DateTime.Now.ToString("yyyyMMdd");
        }
        public EAutono SetNo(EAutono eAutono)
        {
            if (eAutono == null)
            {
                this.no = "00001";
                this.date = DateTime.Now.ToString("yyyyMMdd");
            }
            else
            {
                int i = Convert.ToInt32(eAutono.no) + 1;
                this.no = i.ToString("00000");
                //this.Id = eAutono.Id;
                this.date = eAutono.date;
            }
            return this;
        }
        public string GetNo()
        {
            return GetPrefix() + this.date.Substring(2) + this.no;
        }
        string GetPrefix()
        {
            string pre = "";
            switch(this.formcode)
            {
                case "CASH_1":
                    pre = "C";
                    break;
                case "CASH_2":
                    pre = "E";
                    break;
                case "CASH_3":
                    pre = "A";
                    break;
                case "CASH_3A":
                    pre = "T";
                    break;
                case "CASH_4":
                    pre = "B";
                    break;
                case "CASH_5":
                    pre = "F";
                    break;
                case "CASH_6":
                    pre = "H";
                    break;
                case "CASH_X":
                    pre = "XZ";
                    break;
                case "Payment":
                    pre = "P";
                    break;
                case "Account":
                    pre = "";
                    break;
                default:
                    break;
            }
            return pre;
        }
    }
}
