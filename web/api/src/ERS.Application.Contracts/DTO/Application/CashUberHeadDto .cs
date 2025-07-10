using System;
using System.ComponentModel.DataAnnotations;
namespace ERS.DTO.Application
{
    public class CashUberHeadDto
    {
        public string Emplid { get; set; }
        public string Rno { get; set; }
        public string FormCode { get; set; }
        public string Name { get; set; }
        public string ProjectCode { get; set; }
        public string BusinessTripNo { get; set; }
        public string Program { get; set; }
        public string company { get; set; }
        public DateTime? cdate { get; set; }
        public string cuser { get; set; }
    }
}
