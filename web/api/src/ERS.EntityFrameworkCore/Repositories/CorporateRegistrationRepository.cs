using System;
using ERS.Entities;
using ERS.EntityFrameworkCore;
using ERS.IRepositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace ERS.Repositories
{
    public class CorporateRegistrationRepository : EfCoreRepository<ERSDbContext, CorporateRegistration, Guid>, ICorporateRegistrationRepository
    {
        public CorporateRegistrationRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        
    }
}
