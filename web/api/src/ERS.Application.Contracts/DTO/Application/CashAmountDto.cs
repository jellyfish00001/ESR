using System.ComponentModel.DataAnnotations;
namespace ERS.DTO.Application
{
    public class CashAmountDto
    {
        [StringLength(20)]
        public string formcode { get; set; }
        [StringLength(20)]
        public string rno { get; set; }
        [StringLength(20)]
        public string currency { get; set; }
        public decimal amount { get; set; }
        [StringLength(1)]
        public string actpay { get; set; }
        public decimal actamt { get; set; }
    }
}
