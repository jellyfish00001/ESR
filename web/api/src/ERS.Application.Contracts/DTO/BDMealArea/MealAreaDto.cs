using System;
namespace ERS.DTO
{
    public class MealAreaDto
    {
    }
    public class TravelDto
    {
        public string city { get; set; }
        public DateTime gotime { get; set; }
        public DateTime backtime { get; set; }
        public decimal amount { get; set; }
        public string currency { get; set; }
        public string company { get; set; }
    }
}
