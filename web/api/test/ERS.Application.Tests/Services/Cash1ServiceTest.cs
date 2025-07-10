using ERS.DTO.Application;
using ERS.DTO.BDExp;
using ERS.IRepositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ERS.Services
{
    public class Cash1ServiceTest : ERSApplicationTestBase
    {
        private ICash1Service _Cash1Service;
        private IBDCarRepository _bdcarRepository;

        public Cash1ServiceTest()
        {
            _Cash1Service = GetRequiredService<ICash1Service>();
            _bdcarRepository = GetRequiredService<IBDCarRepository>();
        }


        [Theory(DisplayName = "获取费用情景")]
        [InlineData("WZS")]
        public async Task GetGeneralCostExp(string company)
        {
            IEnumerable<BDExpDto> data = (await _Cash1Service.GetGeneralCostExp(company)).data;
            Assert.True(data.Count() > 0);
        }

        [Theory(DisplayName = "根据公司别获取BDCar")]
        [InlineData("WZS")]
        public async Task GetBDCarByCompany(string company)
        {
            var Result = await _bdcarRepository.QueryBDCarByCompany(company);
            Assert.True(Result.Count > 0);
        }
    }
}
