

using System.Threading.Tasks;
using ERS.Application.Contracts.DTO.Communication;
using Xunit;

namespace ERS.Application.Tests.Services
{
    public class EmployeeServiceTest: ERSApplicationTestBase
    {

        private IEmployeeService _EmployeeService;
        public EmployeeServiceTest(){

            _EmployeeService=GetRequiredService<IEmployeeService>();

        }

        [Theory(DisplayName = "获取员工信息")]
        [InlineData("Z21032798")]
        public async Task get_employee(string user)
        {
            //Act
            var result = await _EmployeeService.QueryEmployeeAsync(user);
            //Assert
            Assert.NotNull(result);
        }

          [Theory(DisplayName = "查询公司最高主管信息")]
        [InlineData("Z21032798")]
        public async Task get_employeeCompany(string user)
        {
            //Act
            var result = await _EmployeeService.QueryEmployeeCompany(user);
            //Assert
            Assert.NotNull(result);
        }

        [Fact(DisplayName = "按工號獲取companycode")]
        public async Task GetCompanyCodeByEmplid()
        {
            var result = await _EmployeeService.GetCompanyCodeAsync("Z21032798");
            Assert.NotNull(result);
        }

        [Fact(DisplayName = "依据人数判断总预算")]
        public void Countbudget()
        {
            var result = _EmployeeService.Countbudget(5, 5, 50);
            Assert.True(result.Budget == 500);
        }

        [Fact(DisplayName = "依据人数判断总预算2")]
        public void Countbudget2()
        {
            var result = _EmployeeService.Countbudget(1, 1, 50);
            Assert.True(result.Budget == 100);
        }

        [Fact(DisplayName = "依据人数判断总预算3")]
        public void Countbudget3()
        {
            var result = _EmployeeService.Countbudget(1, 4, 50);
            Assert.True(result.Budget == 150);
        }

        [Fact(DisplayName = "查詢返回主管所屬類別")]
        public async Task QueryCategory()
        {
            var result = await _EmployeeService.QuerySuperCategory("Z19081961","2","WZS");
            Assert.True(result.data.categoryid == 2);
        }

        [Fact(DisplayName = "查詢返回主管所屬類別2")]
        public async Task QueryCategoryHaveNoData()
        {
            var result = await _EmployeeService.QuerySuperCategory("Z19081966","77","");
            Assert.True(result.status == 2);
        }
    }
}