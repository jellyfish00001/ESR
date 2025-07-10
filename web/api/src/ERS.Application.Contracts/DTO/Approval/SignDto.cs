using System.Collections.Generic;
namespace ERS.DTO
{
    public class SignDto
    {
        public string company { get; set; }
        public string rno { get; set; }
        public string formCode { get; set; }
        // -1/1
        public int? inviteMethod { get; set; }
        public string inviteUser { get; set; }
        public string remark { get; set; }
        public string fromEmplid { get; set; }
        public string toEmplid { get; set; }
        public string toEmplid1 { get; set; }
        public string toEmplid2 { get; set; }
        public string toEmplid3 { get; set; }
        public bool paperSign { get; set; }
        public List<Detail> detail { get; set; }
        public Amount amount { get; set; }
    }
    public class SignResult
    {
        public string formNo { get; set; }
        public string staus { get; set; }
    }
    public class Detail
    {
        public int seq { get; set; }
        public string assignment { get; set; }
        public decimal? taxexpense { get; set; }
        public decimal? amount { get; set; }
    }
    public class Amount
    {
        public decimal actamt { get; set; }
    }
}
