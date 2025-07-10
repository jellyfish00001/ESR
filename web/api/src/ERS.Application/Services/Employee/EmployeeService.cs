using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Application.Contracts.DTO.Communication;
using ERS.Entities;
using Volo.Abp.Application.Services;
using ERS.Application.Contracts.DTO.Employee;
using ERS.IDomainServices;
using ERS.Application.Contracts.DTO.Emporg;
using ERS.DTO;
namespace ERS.Application.Services
{
    public class EmployeeService : ApplicationService, IEmployeeService
    {
        private IEmployeeDomainService _EmployeeDomainService;
        private ICompanyDomainService _CompanyDomainService;
        private IEmporgService _emporgService;
        public EmployeeService(IEmployeeDomainService EmployeeDomainService,
        ICompanyDomainService CompanyDomainService, IEmporgService emporgService)
        {
            _EmployeeDomainService = EmployeeDomainService;
            _CompanyDomainService = CompanyDomainService;
            _emporgService = emporgService;
        }
        public Task<string> GetCompanyCodeAsync(string user)
        {
            return _EmployeeDomainService.GetCompanyCodeAsync(user);
        }
        public async Task<List<EmployeeCompany>> QueryEmployeeCompany(string user)
        {
            List<EmployeeCompany> list = new List<EmployeeCompany>();
            if(user == null)
            {
                return list;
            }
            List<Employee> employees = await _EmployeeDomainService.QueryCompany(user);
            employees.ForEach(b => list.Add(new EmployeeCompany { emplid = b.emplid, name = b.cname, ename = b.ename }));
            return list;
        }
        public async Task<EmployeeDto> GetEmployeeAsync(string user)
        {
            EmployeeDto employeeDto = await QueryEmployeeAsync(user);
            CheckProxyCashDto checkProxyCashDto = await _EmployeeDomainService.GetCashProxy(user);
            //employeeDto.proxylist.Add(user);
            //employeeDto.isproxy = checkProxyCashDto.isproxy;
            //if (checkProxyCashDto.isproxy) employeeDto.proxylist.AddRange(checkProxyCashDto.proxylist);
            return employeeDto;
        }
        public async Task<EmployeeDto> QueryEmployeeAsync(string user)
        {
            EmployeeDto employeeDto = new EmployeeDto();
            //查询返回员工信息
            Employee employee = await _EmployeeDomainService.QueryEmployeeAsync(user);
            if (employee == null) return employeeDto;
            //查询PJccode是否存在
            employeeDto.emplid = employee.emplid;
            employeeDto.deptid = employee.deptid;
            employeeDto.cname = employee.cname;
            employeeDto.ename = employee.ename;
            employeeDto.email = employee.mail;
            employeeDto.phone = employee.phone;
            employeeDto.isaccount = await _EmployeeDomainService.QueryAccount(user) != null; ;
            BDCompanyCategory company = await _CompanyDomainService.GetCompany(employee.company);
            if (company == null) return employeeDto;
            //employeeDto.timezone = company.timezone;
            //employeeDto.company = company.company;
            //employeeDto.curr = company.basecurr;
            //employeeDto.costdeptid = await _emporgService.GetDeptid(employee.deptid, company.companycode);
            return employeeDto;
        }
        //查询返回主管所属类别
        public async Task<Result<Categoryname>> QuerySuperCategory(string userId, string category, string company)
        {
            Result<Categoryname> result = new Result<Categoryname>();
            Employee employee = await _EmployeeDomainService.QueryEmployeeAsync(userId);
            EmpOrgLv empOrgLv = await _EmployeeDomainService.QueryEmporgLv(employee.officer_level.ToString());
            if(empOrgLv == null)
            {
                result.data = null;
                result.status = 2;
                result.message = "The category of company top supervisor ID --" + userId +"-- is null,need use Company code --" + employee.company + "-- check SUPERCATEGORY table.";
                return result;
            }
            Categoryname categoryname = new Categoryname();
            SuperCategory superCategory = await _EmployeeDomainService.QuerySuperCategory(empOrgLv.seq,category, company);
            categoryname.budget = superCategory == null ? 0 : superCategory.budget;
            categoryname.categoryid = superCategory == null ? 0 : superCategory.categoryid;
            categoryname.categoryname = superCategory?.categoryname;
            result.data = categoryname;
            return result;
        }
        //依据人数判断总预算
        //txtcustnum客户参加 & txtwistnum公司参加
        public BudgetAmountDto Countbudget(int txtcustnum, int txtwistnum, decimal budget)
        {
            BudgetAmountDto budgetAmount = new BudgetAmountDto();
            decimal Amount = 0;
            if (txtcustnum == 1)
            {
                if (txtwistnum <= 2)
                {
                    Amount = (txtcustnum + txtwistnum) * budget;
                }
                else
                {
                    Amount = (2 + txtcustnum) * budget;
                }
            }
            else
            {
                if (txtwistnum <= txtcustnum)
                {
                    Amount = (txtcustnum + txtwistnum) * budget;
                }
                else
                {
                    Amount = (2 * txtcustnum) * budget;
                }
            }
            budgetAmount.Budget=Amount;
            if(txtcustnum==1)
            {
                if(txtwistnum<=2)
                {
                    budgetAmount.Stat=true;
                }else
                {
                     budgetAmount.Stat=false;
                }
            }else if(txtcustnum>1)
            {
                if(txtwistnum<=txtcustnum)
                {
                     budgetAmount.Stat=true;
                }else
                {
                     budgetAmount.Stat=false;
                }
            }
            return budgetAmount;
        }
    }
}