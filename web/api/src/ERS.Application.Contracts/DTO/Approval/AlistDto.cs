using System;
namespace ERS.DTO.Approval
{
    public class AlistDto
    {
        public string formcode { get; set; }
        public string rno { get; set; }
        public decimal step { get; set; }
        public string cemplid { get; set; }
        public DateTime? adate { get; set; }
        public string approval { get; set; }//签核人 工号/中文名
        public string status { get; set; }
        public string deptid { get; set; }
        public string deptn { get; set; }
        public string stepname { get; set; }
        public decimal seq { get; set; }
        public string company { get; set; }
    }
}