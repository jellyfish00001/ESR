using ERS.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
namespace ERS.IRepositories
{
    public interface IAuthRoleMenuRepository : IRepository<AuthRoleMenu, Guid>
    {
        Task<List<string>> GetRoleMenus(List<string> roleList);
    }
}
