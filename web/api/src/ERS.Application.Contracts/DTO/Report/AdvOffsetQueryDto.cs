using System.Collections.Generic;
namespace ERS.DTO.Report
{
    public class AdvOffsetQueryDto
    {
        public string rno { get; set; }
        public string cuser { get; set; }
        public string status { get; set; }// Y:已冲账 N：未冲账
        public List<string> company { get; set; }
    }
}