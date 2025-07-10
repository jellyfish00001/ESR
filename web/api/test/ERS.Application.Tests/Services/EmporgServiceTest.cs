using System.Linq;
using System.Threading.Tasks;
using ERS.Application.Contracts.DTO.Emporg;
using Xunit;

namespace ERS.Application.Tests.Services
{
    public class EmporgServiceTest: ERSApplicationTestBase
    {
      private IEmporgService _emporgService;
        public EmporgServiceTest(){

            _emporgService=GetRequiredService<IEmporgService>();

        }
        [Theory(DisplayName = "部门模糊查询")]
        [InlineData("JML1")]
        public async Task get_pjcode(string code)
        {
            //Act
            var result = await _emporgService.QueryVagues(code);
            //Assert
            Assert.NotNull(result);
        }

        [Fact(DisplayName = "获取前100个部门")]
        public async Task GetPartDeptidList()
        {
            //Act
            var result = await _emporgService.GetPartDeptidList();
            //Assert
            Assert.True(result.Count() > 0);
        }
    }
}