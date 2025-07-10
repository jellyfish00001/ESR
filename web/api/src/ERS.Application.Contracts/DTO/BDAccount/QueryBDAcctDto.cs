using System;
namespace ERS.DTO.BDAccount
{
    public class QueryBDAcctDto
    {
        public string acctcode { get; set; }
        public string acctname { get; set; }
        public string stat { get; set; }
        public string cuser { get; set; }
        public DateTime? cdate { get; set; }
        public string muser { get; set; }
        public DateTime? mdate { get; set; }
        public string company { get; set; }
    }
}