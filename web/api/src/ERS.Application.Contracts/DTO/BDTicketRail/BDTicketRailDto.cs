using System;
namespace ERS.DTO.BDTicketRail
{
    public class BDTicketRailDto
    {
        public Guid Id { get; set; }
        public string ticketrail { get; set; }
        public string voucheryear { get; set; }
        public string vouchermonth { get; set; }
        public string currentnumber { get; set; }
        public DateTime? cdate { get; set; }
        public string cuser { get; set; }
        public DateTime? mdate { get; set; }
        public string muser { get; set; }
        public string company { get; set; }
    }
}