using System;
namespace ERS.DTO.BDPaperSign
{
    public class EditPaperSignDto
    {
        public Guid Id { get; set; }
        public string company { get; set; }
        public string company_code { get; set; }
        public string plant { get; set; }
        public string emplid { get; set; }
    }
}