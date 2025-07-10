namespace ERS.DTO.Account
{
    /// <summary>
    /// SA入账清单ExcelDTO
    /// </summary>
    public class SACarryDetailReportDto
    {
        public int seq { get; set; }
        public string companysap { get; set; }
        public string docdate { get; set; }
        public string postdate { get; set; }
        public string doctyp { get; set; } = "KR";
        public string basecurr { get; set; }
        public decimal rate { get; set; } = 1;//5
        public string @ref { get; set; }
        public string dochead { get; set; }
        public string postkey { get; set; }
        public string acctcode { get; set; }
        public decimal actamt1 { get; set; }//12
        public decimal actamt2 { get; set; }//13
        public string costcenter { get; set; }
        public string proficenter1 { get; set; }
        public string asinmnt { get; set; }
        public string txtcode { get; set; }
        public string lintext { get; set; }
        public string orer { get; set; }
        public string custercode { get; set; }
        public string plant { get; set; }
        public string busityp { get; set; }
        public string ref1 { get; set; }
        public string ref2 { get; set; }
        public string ref3 { get; set; }
        public string excelline { get; set; }
        public string tradingpartner { get; set; }
        public string proficenter2 { get; set; }
    }
}