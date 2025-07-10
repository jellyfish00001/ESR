using System;
namespace ERS.DTO.Print
{
    public class PrintDto
    {
        public string rno { get; set; }
        public string formcode { get; set; }
        public string cuser { get; set; }
        public string cname { get; set; }
        public DateTime? cdate { get; set; }
        public decimal? step { get; set; }
        public string stepname { get; set; }
        public string status { get; set; }
        public string apid { get; set; }
        public string formname { get; set; }
        public string company { get; set; }
        public string payment { get; set; }
        public DateTime? paymentdate { get; set; }
    }
}