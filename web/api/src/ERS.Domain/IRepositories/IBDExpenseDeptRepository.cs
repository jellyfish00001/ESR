using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Entities;
using Volo.Abp.Domain.Repositories;

namespace ERS.IRepositories
{
    public interface IBDExpenseDeptRepository : IRepository<BDExpenseDept, Guid>
    {
        Task<BDExpenseDept> GetBDExpenseDept(string company, string deptid, string isvirtualdept);
        Task<List<BDExpenseDept>> GetBDExpenseDeptByIds(List<Guid?> Ids);
    }
}