using System;
namespace ERS.DTO.BDMealArea
{
    public class AddBDMealAreaDto
    {
        public string area { get; set; }
        public string city { get; set; }
        public int gotype1 { get; set; }
        public int gotype2 { get; set; }
        public DateTime? gotime1 { get; set; }
        public DateTime? gotime2 { get; set; }
        public int backtype1 { get; set; }
        public int backtype2 { get; set; }
        public DateTime? backtime1 { get; set; }
        public DateTime? backtime2 { get; set; }
        public decimal amount { get; set; }
        public string currency { get; set; }
        public string company { get; set; }
    }
}