using System;
using System.Collections.Generic;
namespace ERS.DTO.Approval
{
    public class SignlogDto
    {
        public string rno { get; set; }
        public decimal seq { get; set; }
        public string aemplid { get; set; }
        public string astepname { get; set; }
        public DateTime adate { get; set; }
        public string aresult { get; set; }
        public string aremark { get; set; }
        public string formcode { get; set; }
        public decimal step { get; set; }
        public string cemplid { get; set; }
        public string aname { get; set; }
        public string aename { get; set; }
        public string deptid { get; set; }
    }
    public class SignProcessAndLogDto
    {
        public List<SignlogDto> signLog { get; set; }
        public List<AlistDto> signProcess { get; set; }
    }
}