using System;
using System.Collections.Generic;
namespace ERS.DTO.UberFare
{
    public class QueryUberTransactionalDto
    {
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public string rno { get; set; }
        public string cuser { get; set; }
        public List<string> signStatus { get; set; }
        public List<string> company { get; set; }// ¤½¥q
    }
}