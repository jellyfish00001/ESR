using ERS.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
namespace ERS.IRepositories
{
    public interface IAuthUserCompanyRepository : IRepository<AuthUserCompany, Guid>
    {
        Task<List<string>> GetUserCompanys(string userId,string module);
    }
}
