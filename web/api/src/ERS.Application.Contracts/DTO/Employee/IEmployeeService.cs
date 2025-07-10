using ERS.Application.Contracts.DTO.Employee;
using ERS.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ERS.Application.Contracts.DTO.Communication
{
    public interface IEmployeeService
    {
        Task<string> GetCompanyCodeAsync(string user);
        //获取个人信息、是否存在银行卡账号
        Task<EmployeeDto> QueryEmployeeAsync(string user);
        //查询公司最高主管信息
        Task<List<EmployeeCompany>> QueryEmployeeCompany(string user);
        //获取最高主管所属类别
        Task<Result<Categoryname>> QuerySuperCategory(string userId, string category, string company);
        //依人数计算总预算
        BudgetAmountDto Countbudget(int txtcustnum, int txtwistnum, decimal budget);
        Task<EmployeeDto> GetEmployeeAsync(string user);
    }
}