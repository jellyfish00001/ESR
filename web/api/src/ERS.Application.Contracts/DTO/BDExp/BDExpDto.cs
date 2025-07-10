using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
namespace ERS.DTO.BDExp
{
    public class BDExpDto
    {
        public string expcode { get; set; }
        public string expname { get; set; }
        public string description { get; set; }
        public string keyword { get; set; }
        public string filecategory { get; set; }
        public string isupload { get; set; }
        public string filepoints { get; set; }
        public string isinvoice { get; set; }
        public int expcategory { get; set; }
        public string acctcode { get; set; }
        public string acctname { get; set; }
        public string datelevel { get; set; }
        public string company { get; set; }
        public string category { get; set; }
        public string expnamezhcn { get; set; }
        public string expnamezhtw { get; set; }
        public string expnamevn { get; set; }
        public string expnamees { get; set; }
        public string expnamecz { get; set; }
    }
    public class ExpDtoComparer : IEqualityComparer<BDExpDto>
    {
        public bool Equals(BDExpDto x, BDExpDto y)
        {
            if (x == null || y == null) return false;
            return x.expcode == y.expcode;
        }
        public int GetHashCode([DisallowNull] BDExpDto obj)
        {
            if (obj == null) return 0;
            return obj.expcode.GetHashCode() ^ obj.expcode.GetHashCode();
        }
    }
    public class EXPAddSign
    {
        public string expcode { get; set; }
        public string addsign { get; set; }
        public string addsignstep { get; set; }
    }
}
