using ERS.DTO;
using ERS.Entities;
using ERS.IDomainServices;
using ERS.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
namespace ERS.DomainServices
{
    public class EmployeeDomainService : IEmployeeDomainService
    {
        private IRepository<Employee, Guid> _EmployeeRepository;
        private IRepository<EmpOrgLv, Guid> _EmpOrgLvRepository;
        private IRepository<SuperCategory, Guid> _SuperCategoryRepository;
        private IRepository<BDCompanyCategory, Guid> _CompanyRepository;
        private IRepository<CashAccount, Guid> _CashAccountRepository;
        private IProxyCashRepository _ProxyCashRepository;
        public EmployeeDomainService(IRepository<Employee, Guid> EmployeeRepository,
        IRepository<EmpOrgLv, Guid> EmpOrgLvRepository,
        IRepository<SuperCategory, Guid> SuperCategoryRepository,
        IRepository<BDCompanyCategory, Guid> CompanyRepository,
        IRepository<CashAccount, Guid> CashAccountRepository,
        IProxyCashRepository ProxyCashRepository)
        {
            _EmployeeRepository = EmployeeRepository;
            _EmpOrgLvRepository=EmpOrgLvRepository;
            _SuperCategoryRepository=SuperCategoryRepository;
            _CompanyRepository = CompanyRepository;
            _CashAccountRepository = CashAccountRepository;
            _ProxyCashRepository = ProxyCashRepository;
        }
        public async Task<string> GetCompanyCodeAsync(string user)
        {
            return (await _EmployeeRepository.WithDetailsAsync()).Where(i => i.emplid == user).Select(i => i.company).FirstOrDefault();
        }
        //查询用户id返回员工信息
        public async Task<Employee> QueryEmployeeAsync(string user)
        {
            return (await _EmployeeRepository.WithDetailsAsync()).Where(i => i.emplid == user).FirstOrDefault();
        }
        public async Task<IList<Employee>> deptIdEmployees(List<string> depts)
        {
            return (await _EmployeeRepository.WithDetailsAsync()).Where(b => depts.Any(c => c.Equals(b.deptid))).ToList();
        }
        public async Task<string> GetDefaultCompanyAsync(string user)
        {
            string code = await GetCompanyCodeAsync(user);
            return (await _CompanyRepository.WithDetailsAsync()).Where(i => i.CompanyCategory == code).Select(i => i.company).FirstOrDefault();
        }
        public async Task<CashAccount> QueryAccount(string userId)
        {
            return (await _CashAccountRepository.WithDetailsAsync()).Where(b => b.emplid == userId).FirstOrDefault();
        }
        public async Task<List<Employee>> QueryCompany(string user)
        {
            return (await _EmployeeRepository.WithDetailsAsync()).Where(i => String.IsNullOrEmpty(i.tdate) && (i.emplid.Contains(user) || i.cname.Contains(user) || i.ename.ToUpper().Contains(user.ToUpper())) ).GroupBy(w => w.emplid).Select(g => g.First()).Take(10).ToList();
        }
        public async Task<EmpOrgLv> QueryEmporgLv(string level)
        {
            return (await _EmpOrgLvRepository.WithDetailsAsync()).Where(b => b.officer_level==level).FirstOrDefault();
        }
        public async Task<SuperCategory> QuerySuperCategory(int categoryid,string category, string company)
        {
            string area = "out";
            if(category == "0")
                area = "in";
            return(await _SuperCategoryRepository.WithDetailsAsync()).Where(b => b.categoryid==categoryid && b.area == area && b.company == company).FirstOrDefault();
        }
        public async Task<CheckProxyCashDto> GetCashProxy(string cuser)
        {
            CheckProxyCashDto result = new();
            result.isproxy = false;
            List<string> proxyCash = await _ProxyCashRepository.ReadProxyByEmplid(cuser);
            if (proxyCash.Count() > 0)
            {
                result.isproxy = true;
                result.cuser = cuser;
                result.proxylist = proxyCash;
            }
            return result;
        }
    }
}
