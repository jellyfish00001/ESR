using System;
using System.Collections.Generic;
namespace ERS.DTO.Print
{
    public class PrintQueryDto
    {
        public DateTime? startdate { get; set; }
        public DateTime? enddate { get; set; }
        public string rno { get; set; }
        public string cuser { get; set; }
        public List<string> status { get; set; }
        public List<string> company { get; set; }
        public List<string> formcode { get; set; }
        /// <summary>
        /// 签核人
        /// </summary>
        /// <value></value>
        public string aemplid { get; set; }
    }
}