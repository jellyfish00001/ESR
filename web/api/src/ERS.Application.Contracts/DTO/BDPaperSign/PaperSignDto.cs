using System;
namespace ERS.DTO.BDPaperSign
{
    /// <summary>
    /// 纸本单签核人返回DTO
    /// </summary>
    public class PaperSignDto
    {
        public Guid Id { get; set; }
        public string company { get; set; }
        public string company_code { get; set; }
        public string plant { get; set; }
        public string emplid { get; set; }
        public string emplname { get; set; }
        public string cuser { get; set; }
        public DateTime? cdate { get; set; }
        public string muser { get; set; }
        public DateTime? mdate { get; set; }
    }
}