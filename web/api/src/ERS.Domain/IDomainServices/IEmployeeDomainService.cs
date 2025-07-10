using ERS.DTO;
using ERS.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
namespace ERS.IDomainServices
{
    public interface IEmployeeDomainService : IDomainService
    {
        Task<string> GetCompanyCodeAsync(string user);
        //查询返回员工信息
        Task<Employee> QueryEmployeeAsync(string user);
        Task<string> GetDefaultCompanyAsync(string user);
        //查询银行卡账号
         Task<CashAccount> QueryAccount(string userId);
         //模糊查询公司最高主管
        Task<List<Employee>> QueryCompany(string user);
        //查询组织层次信息
        Task<EmpOrgLv> QueryEmporgLv(string level);
        //组织分类
        Task<SuperCategory>QuerySuperCategory(int categoryid, string category, string company);
        //查詢傳入部門所有員工
        Task<IList<Employee>> deptIdEmployees(List<string> depts);
        Task<CheckProxyCashDto> GetCashProxy(string cuser);
    }
}
