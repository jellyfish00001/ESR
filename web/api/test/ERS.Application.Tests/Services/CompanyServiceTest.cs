using System.Linq;
using ERS.DTO;
using ERS.DTO.Company;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using ERS.DTO.BDCompanyCategory;

namespace ERS.Services
{
    public class CompanyServiceTest : ERSApplicationTestBase
    {
        private ICompanyService _CompanyService;

        public CompanyServiceTest()
        {
            _CompanyService = GetRequiredService<ICompanyService>();
        }

        [Theory(DisplayName = "获取公司别")]
        [InlineData("Z19081961")]
        public async Task get_companys(string user)
        {
            //Act
            var result = await _CompanyService.GetAllComapnyAsync(user);

            //Assert
            Assert.NotNull(result);
        }

        [Fact(DisplayName = "公司別維護（查詢）")]
        public async Task GetCompaniesInfo()
        {
            Request<List<string>> request = new Request<List<string>>(){
                data = new List<string>()
            };
            request.data.Add("WZS");
            var result = await _CompanyService.GetCompaniesInfo(request);
            Assert.True(result.data.Count > 0);
        }

        [Fact(DisplayName = "公司別維護（添加成功）")]
        public async Task AddCompanyInfoSuccess()
        {
            ComInfoDto request = new ComInfoDto()
            {
                company = "WZSss",
                companycode = "130",
                companysap = "test",
                companydesc = "測試公司",
                basecurr = "RMB",
                timezone = 8,
                taxcode = 0,
                stwit = "ZS"
            };
            var result = await _CompanyService.AddCompanyInfo(request, "Z22070025");
            Assert.True(result.status == 1);
        }

        [Fact(DisplayName = "公司別維護（添加失敗）")]
        public async Task AddCompanyInfoFail()
        {
            ComInfoDto request = new ComInfoDto()
            {
                company = "WZS",
                companycode = "130",
                companysap = "test",
                companydesc = "測試公司",
                basecurr = "RMB",
                timezone = 8,
                taxcode = 0,
                stwit = "ZS"
            };
            var result = await _CompanyService.AddCompanyInfo(request, "Z22070025");
            Assert.True(result.status == 2);
        }

        [Fact(DisplayName = "公司別維護（編輯）")]
        public async Task EditCompanyInfoSuccess()
        {
            Request<List<string>> request = new Request<List<string>>(){
                data = new List<string>()
            };
            request.data.Add("WZS");
            var query = await _CompanyService.GetCompaniesInfo(request);
            BDCompanyCategoryParamDto comInfoDto = new BDCompanyCategoryParamDto()
            {
                Id = query.data.FirstOrDefault().Id,
                Company = "testEDITCompany",
                CompanyCategory = query.data.FirstOrDefault().CompanyCategory,
                BaseCurrency = query.data.FirstOrDefault().BaseCurrency,
                CompanyDesc = query.data.FirstOrDefault().CompanyDesc,
                CompanySap = query.data.FirstOrDefault().CompanySap,
                Stwit = "testEDITCompany"
            };
            var result = await _CompanyService.EditCompanyInfo(comInfoDto,"Z22070025");
            Assert.True(result.status == 1);
        }

        [Fact(DisplayName = "公司別維護（刪除）")]
        public async Task DeleteCompanyInfo()
        {
            List<BDCompanyCategoryParamDto> comInfoDtos = new List<BDCompanyCategoryParamDto>();
            Request<List<string>> request = new Request<List<string>>(){
                data = new List<string>()
            };
            request.data.Add("WZS");
            var query = await _CompanyService.GetCompaniesInfo(request);
            BDCompanyCategoryParamDto comInfoDto = new BDCompanyCategoryParamDto()
            {
                Id = query.data.FirstOrDefault().Id
            };
            comInfoDtos.Add(comInfoDto);
            var result = await _CompanyService.DeleteCompanyInfo(comInfoDtos);
            Assert.True(result.status == 1);
        }

        [Fact(DisplayName = "根據公司別獲取companycode")]
        public async Task GetCompanycodeByCompany()
        {
            var result = await _CompanyService.GetCompanyCodeByCompany("WZS");
            Assert.True(result.data.Count > 0);
        }
    }
}
