using ERS.Application.Contracts.DTO.Communication;
using ERS.Application.Contracts.DTO.Employee;
using ERS.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ERS.Controllers
{
 #if DEBUG
    [AllowAnonymous]
#endif
    public class EmployeeController : BaseController
    {
        private IEmployeeService _EmployeeService;
        public EmployeeController(IEmployeeService EmployeeService)
        {
            _EmployeeService = EmployeeService;
        }
         [HttpGet("Employee")]
        public async Task<EmployeeDto> QueryEmployeeAsync()
        {
            return await _EmployeeService.GetEmployeeAsync(this.userId);
        }
        [HttpGet("Employee/{user}")]
        public async Task<EmployeeDto> QueryEmployeeAsync(string user)
        {
            return await _EmployeeService.QueryEmployeeAsync(user);
        }
        //查询最高主管所属类别 category(一般交际费（宴客申请）餐饮时间段)
        [HttpGet("Employee/manager/Category")]
        public async Task<Result<Categoryname>> QuerySuperCategory(string userId, string category, string company)
        {
            return await _EmployeeService.QuerySuperCategory(userId,category, company);
        }
        //模糊查询最高主管信息
        [HttpGet("Employee/manager")]
        public async Task<List<EmployeeCompany>> QueryEmployeeCompany(string user)
        {
            return await _EmployeeService.QueryEmployeeCompany(user);
        }
        //依据人数判断总预算
        [HttpGet("Employee/budget")]
        public   BudgetAmountDto  Countbudget(int customernum, int employeenum, decimal budget)
        {
            return  _EmployeeService.Countbudget(customernum,employeenum,budget);
        }
    }
}
