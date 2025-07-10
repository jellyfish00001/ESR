
using System;

namespace ERS.DTO.CashCarryDetail
{
    public class CashCarryDetailDto
    {
        public string Id { get; set; }
        public string carryno { get; set; }
        public int carritem { get; set; }
        public int seq { get; set; }
        public DateTime docdate { get; set; }
        public DateTime postdate { get; set; }

        public string companysap { get; set; }
  
        public string basecurr { get; set; }

        public decimal rate { get; set; } = 1;

        public string @ref { get; set; }
   
        public string dochead { get; set; }

        public string doctyp { get; set; } = "KR";

        public string postkey { get; set; }

        public string acctcode { get; set; }

        public string specgl { get; set; }

        public decimal actamt1 { get; set; }

        public decimal actamt2 { get; set; }

        public string payterm { get; set; }

        public string paytyp { get; set; } = "T";

        public DateTime baslindate { get; set; }

        public string txtcode { get; set; }
  
        public decimal? taxamt1 { get; set; }

        public decimal? taxamt2 { get; set; }

        public string wtaxtyp { get; set; }

        public string wtaxcode { get; set; }

        public decimal? wtaxamt1 { get; set; }

        public decimal? wtaxamt2 { get; set; }

        public string costcenter { get; set; }

        public string order { get; set; }
 
        public string linetext { get; set; }

        public string asinmnt { get; set; }

        public string proficenter1 { get; set; }

        public string proficenter2 { get; set; }

        public string custercode { get; set; }

        public string plant { get; set; }

        public string busityp { get; set; }

        public string ecuster { get; set; }

        public string mtrldiv { get; set; }

        public string salsdiv { get; set; }
 
        public string ref1 { get; set; }

        public string ref2 { get; set; }

        public string ref3 { get; set; }

        public string formcode { get; set; }

        public string rno { get; set; }
        public int pstitem { get; set; }

        public string stat { get; set; } = "Y";

        public string rnostatus { get; set; } = "N";

        public string excelline { get; set; }

        public string tradingpartner { get; set; }

        public string acctant { get; set; }

        public string bank { get; set; }

        public string unifycode { get; set; }

        public string certificate { get; set; }
    }
}