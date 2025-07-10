using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace ERS.DTO.Application
{
    public class CashUberDetailDto
    {
        public string FormCode { get; set; }

        public string Rno { get; set; }

        public string Item { get; set; }

        public DateTime? StartDate { get; set; }

        public string Destination { get; set; }

        public string Origin { get; set; }

        public string Status { get; set; }

        public decimal? Amount { get; set; }

        public string Reason { get; set; }

        public string ExpCode { get; set; }

        public string Emplid { get; set; }

        public string Name { get; set; }

        public string DeptId {get; set;}
    }
}
