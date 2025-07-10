using System.Threading.Tasks;
using ERS.DTO.Application;
using Xunit;

namespace ERS.Services
{
    public class ApplicationServiceTest : ERSApplicationTestBase
    {
        private ICash2Service _cash2Service;

        public ApplicationServiceTest()
        {
            _cash2Service = GetRequiredService<ICash2Service>();
        }

        [Theory(DisplayName = "单据明细查询")]
        [InlineData("E22080900001")]
        public async Task QueryApllication(string rno)
        {
            var result = await _cash2Service.GetQueryApplications(rno);
            Assert.NotNull(result);
        }

        [Theory(DisplayName = "单据明细查询 错误rno")]
        [InlineData("E2208090000122")]
        public async Task QueryApllication_ErrorRno(string rno)
        {
            var result = await _cash2Service.GetQueryApplications(rno);
            Assert.True(result.status == 1);
        }
    }
}