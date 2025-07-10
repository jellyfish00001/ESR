using ERS.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
namespace ERS.IRepositories
{
    public interface IAuthRoleApiRepository : IRepository<AuthRoleApi, Guid>
    {
        Task<List<string>> GetApis(List<string> roleList);
    }
}
