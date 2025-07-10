using ERS.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace ERS.IRepositories
{
    public interface IEmpOrgRepository : IRepository<EmpOrg, Guid>
    {
        Task<EmpOrg> GetDeptid(string deptid, string company);
        Task<IList<EmpOrg>> GetDeptidList(int number);

        //模糊查询部门
        Task<IEnumerable<EmpOrg>> QueryVagues(string deptid);
        Task<string> GetCostDeptid(string deptid, string company);

        //查询部门


        Task<IList<EmpOrg>> GetDeptidList(string company);
        Task<string> GetPlantByDeptid(string deptid);
        Task<string> GetBGByDeptid(string deptid);
        Task<string> GetCompanyCodeByDeptid(string deptid);
    }
}
