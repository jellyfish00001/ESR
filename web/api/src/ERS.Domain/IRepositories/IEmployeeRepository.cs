using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using ERS.Entities;


namespace ERS.IRepositories
{
    public interface IEmployeeRepository : IRepository<Employee, Guid>
    {
        Task<string> GetCnameByEmplid(string emplid);
        Task<bool> EmpIsExist(string emplid);
        Task<string> GetCompanyCodeByUser(string emplid);
        Task<bool> CheckIsLeft(string emplid);
    }
}
