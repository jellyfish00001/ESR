using System;
namespace ERS.DTO.Company
{
    public class ComInfoDto
    {
        public Guid? Id { get; set; }//Id
        public string company { get; set; }//公司别
        public string companycode { get; set; }//公司
        public string companysap { get; set; }//SAP公司别
        public string companydesc { get; set; }//名称
        public string basecurr { get; set; }//本位币
        public string area { get; set; }
        public string identificationcode { get; set; }//纳税人识别号
        public decimal? taxcode { get; set; } = 0;//所得稅稅率
        public int timezone { get; set; }//时区
        public string cuser { get; set; }//创建人
        public string muser { get; set; }//更新人
        public DateTime? cdate { get; set; }//创建日期
        public DateTime? mdate { get; set; }//更新日期
        public string stwit { get; set; }//缩写
    }
}