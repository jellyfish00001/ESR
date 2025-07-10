using System.Threading.Tasks;
using ERS.Application.Contracts.DTO.PmcsPjcode;
using Xunit;

namespace ERS.Application.Tests.Services
{
    public class PjcodeServiceTest: ERSApplicationTestBase
    {
      private IPmcsPjcodeService _PjcodeService;
        public PjcodeServiceTest(){

            _PjcodeService=GetRequiredService<IPmcsPjcodeService>();

        }
        [Theory(DisplayName = "Pjcode")]
        [InlineData("EP2018","WZS")]
        public async Task get_pjcode(string code,string company)
        {
            //Act
            var result = await _PjcodeService.QueryPjcode(code,company);
            //Assert
            Assert.NotNull(result);
        }
        
    }
}