using System;
namespace ERS.DTO.Payment
{
    public class GenerateListDto
    {
        public string company { get; set; }
        public DateTime? paymentdate { get; set; }
        public string identification { get; set; }
    }
}