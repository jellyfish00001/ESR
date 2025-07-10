using System;
using System.Collections.Generic;
namespace ERS.DTO.Report
{
    public class GetOverSpendExcelDto
    {
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public List<string> companyList { get; set; }
    }
}