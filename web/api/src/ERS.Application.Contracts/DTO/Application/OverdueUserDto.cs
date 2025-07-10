using System;
namespace ERS.Domain.Entities.Application
{
    public class OverdueUserDto
    {
        public string company { get; set; }
        public string username { get; set; }
        public string user { get; set; }
        public string payee { get; set; }
        public string payeeuser { get; set; }
        public string rno { get; set; }
        public string remark { get; set; }
        public decimal amount { get; set; }
        public decimal actamt { get; set; }
        public int opendays { get; set; }
        /// <summary>
        /// 逾期次数
        /// </summary>
        /// <value></value>
        public int delay { get; set; }
        /// <summary>
        /// 逾期天数
        /// </summary>
        /// <value></value>
        public int delaydays { get; set; }
        public DateTime? cdate { get; set; }
    }
}