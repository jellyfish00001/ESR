using System.Collections.Generic;
namespace ERS.Model
{
    public class DirectorSign
    {
        public string project { get; set; }
        public string project_num { get; set; }
        public decimal money { get; set; }
        public string emplid { get; set; }
        public string currency { get; set; }
        public string deptid { get; set; }
        public string key { get; set; }
    }
    public class signUser
    {
        public decimal seq { get; set; }
        public string step { get; set; }
        public string emplid { get; set; }
        public string deptid { get; set; }
        public int level { get; set; }
        public bool isCoHead { get; set; }
        public string levelDesc { get; set; }
        public string levelDescA { get; set; }
    }
    public class VerificationAuthorityResult
    {
        public IList<signUser> signUsers { get; set; }
    }
}
