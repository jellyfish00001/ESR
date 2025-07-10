namespace ERS.DTO.BDCompanyCategory
{
    using System.Collections.Generic;

    public class BDCompanyCategoryParamDto
    {
        public string Id { get; set; }
        public string CompanyCategory { get; set; }

        public string CompanyDesc { get; set; }

        public string CompanySap { get; set; }

        public string Stwit { get; set; }

        public string BaseCurrency { get; set; }

        public string IdentificationNo { get; set; }

        public decimal IncomeTaxRate { get; set; }

        public decimal Vatrate { get; set; }

        public string Status { get; set; }

        public string Area { get; set; }

        public int TimeZone { get; set; }

        public string Site { get; set; }

        public string Company { get; set; }

        public bool Primary { get; set; }

        public List<BDCompanySiteParamDto> CompanySite { get; set; }
        }
}