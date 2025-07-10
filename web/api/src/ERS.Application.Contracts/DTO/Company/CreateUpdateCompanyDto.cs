namespace ERS.DTO.Company
{
    public class CreateUpdateCompanyDto
    {
        public string company { get; set; }
        public string companydesc { get; set; }
        public string companysap { get; set; }
        public string companycode { get; set; }
        public int seq { get; set; }
        public string stwit { get; set; }
        public string basecurr { get; set; }
        public string identificationcode { get; set; }
        public decimal taxcode { get; set; }
        public string stat { get; set; }
        public string flag { get; set; }
        public string unifycode { get; set; }
        public string skipfin { get; set; }
        public int timezone { get; set; }
    }
}
