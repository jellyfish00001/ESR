using System;
namespace ERS.DTO
{
    public class FinReviewDto
    {
        public Guid Id { get; set; }
        public string company_code { get; set; }
        public string plant { get; set; }
        public string rv1 { get; set; }
        public string rv2 { get; set; }
        public string rv3 { get; set; }
    }
}
