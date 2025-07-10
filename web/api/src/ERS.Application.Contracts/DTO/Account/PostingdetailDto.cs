using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERS.DTO.Account
{
    public class PostingdetailDto
    {
        public DateTime? ddate { get; set; }
        public string companysap { get; set; }
        public string basecurr { get; set; }
        public string postkey { get; set; }
        public string acctcode { get; set; }
        public decimal  baseamt { get; set; }
        public string txtcode { get; set; }
        public string costcenter { get; set; }
        public string lintext { get; set; }
        public string asinmnt { get; set; }
        public string payeeid { get; set; }
        public string rno { get; set; }
        public string formcode { get; set; }
        public int pstitem { get; set; }
        public string expcode { get; set; }
        public string company { get; set; }
        public decimal? lctaxbase { get; set; }
        public decimal? taxbase { get; set; }
        public int detailitem { get; set; }
    }
}