using ERS.Entities;
using ERS.EntityFrameworkCore;
using ERS.IRepositories;
using System;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace ERS.Repositories
{
    public class AuthRoleRepository : EfCoreRepository<ERSDbContext, AuthRole, Guid>, IAuthRoleRepository
    {
        public AuthRoleRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }
       
    }
}
