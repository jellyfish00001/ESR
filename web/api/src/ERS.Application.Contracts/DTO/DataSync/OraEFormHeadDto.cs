using System;
namespace ERS.DTO.DataSync
{
    /// <summary>
    /// 接收Oracle查询到的数据
    /// </summary>
    public class OraEFormHeadDto
    {
        public string FORM_CODE { get; set; }
        public string RNO { get; set; }
        public string CUSER { get; set; }
        public string CNAME { get; set; }
        public DateTime CDATE { get; set; }
        public string CEMPLID { get; set; }
        public decimal? STEP { get; set; }
        public string APID { get; set; }
        public string STATUS { get; set; }
        public string NEMPLID { get; set; }
        public string ADDUSER { get; set; }
    }
}