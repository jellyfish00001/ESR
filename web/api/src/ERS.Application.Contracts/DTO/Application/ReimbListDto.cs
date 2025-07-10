using System;
namespace ERS.DTO.Application
{
    public class ReimbListDto
    {
        public DateTime cdate { get; set; }
        public string deptid { get; set; }
        public string currency { get; set; }
        public decimal amount { get; set; }
        public string summary { get; set; }
        public string payeeid { get; set; }
        public string payeename { get; set; }
        public string bank { get; set; }
        public string payeedept { get; set; }
        public decimal rate { get; set; }//汇率
        public decimal baseamt { get; set; }//本位币金额
    }
}