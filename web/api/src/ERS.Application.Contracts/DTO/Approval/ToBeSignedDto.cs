using System;
namespace ERS.DTO.Approval
{
    public class ToBeSignedDto
    {
        public string rno { get; set; }
        public string company { get; set; }
        public string formcode { get; set; }
        public string formname { get; set; }
        public string expname { get; set; }
        public string currency { get; set; }
        public decimal? actamt { get; set; }
        public string cuser { get; set; }
        public DateTime? cdate { get; set; }
        public string cname { get; set; }
        public decimal? step { get; set; }
        public string formcategory { get; set; }
        public string stepname { get; set; }
        public string apid { get; set; }
    }
}