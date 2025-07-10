using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using ERS.Entities;


namespace ERS.IRepositories
{
    public interface IEmployeeInfoRepository : IRepository<EmployeeInfo, Guid>
    {
        //模糊查询員工；根據工號或者英文名
        Task<List<EmployeeInfo>> QueryByEmplidOrEngName(string keyword);
        Task<bool> EmpIsExist(string emplid);

        Task<EmployeeInfo> QueryByEmail(string email);

        Task<EmployeeInfo> QueryByEmplid(string emplid);
    }
}
