using System.ComponentModel.DataAnnotations;
namespace ERS.DTO.PapreSign
{
    public class EFormHeadDto
    {
        [StringLength(20)]
        public string formcode { get; set; }
        [StringLength(20)]
        public string rno { get; set; }
        public string cname { get; set; }
        [StringLength(20)]
        public string cemplid { get; set; }
        public decimal? step { get; set; }
        public string apid { get; set; }
    }
}
