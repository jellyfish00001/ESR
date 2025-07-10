using System.Collections.Generic;
namespace ERS.DTO.Application
{
    public class CreateUpdateApplicationDto
    {
        public CashHeadDto head { get; set; }
        public IList<CashDetailDto> detailList { get; set; }
    }
}
