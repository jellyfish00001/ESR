using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERS.DTO.Application
{
    public class UberApplicationDto
    {
        public CashUberHeadDto head { get; set; }
        public List<CashUberDetailDto> detail { get; set; }
    }
}
