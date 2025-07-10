using System.Collections.Generic;
namespace ERS.DTO.BDPaperSign
{
    public class QueryPaperSignDto
    {
        public List<string> companyList { get; set; }
        public string plant { get; set; }
        public string emplid { get; set; }
    }
}