using System;
namespace ERS.DTO.BDTicketRail
{
    public class EditBDTicketRailDto
    {
        public Guid Id { get; set; }
        public string company { get; set; }
        public string ticketrail { get; set; }
        public string voucheryear { get; set; }
        public string vouchermonth { get; set; }
    }
}