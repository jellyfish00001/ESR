using System;
namespace ERS.DTO.BDCar
{
    public class SelfDriveCostDto
    {
        public string expcode { get; set; }//报销情景代码
        public string expname { get; set; }//报销情景
        public string deptid { get; set; }//费用归属部门
        public string departureplace { get; set; }//起始地
        public DateTime? cdate { get; set; }//费用发生日期
        public string vehicletype { get; set; }//车型
        public decimal vehiclevalue { get; set; } //车型value
        public decimal kilometers { get; set; }// 公里数
        public string summary { get; set; }//摘要
        public decimal? total { get; set; }
        public decimal? rate { get; set; }
        public string payeeid { get; set; }//收款人工号
        public string payeename { get; set; }//收款人中文名
        public string bank { get; set; }//銀行
        public string payeedept { get; set; }//收款人部門
    }
}