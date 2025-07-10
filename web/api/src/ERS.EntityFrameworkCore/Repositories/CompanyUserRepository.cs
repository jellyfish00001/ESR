using ERS.Entities;
using ERS.EntityFrameworkCore;
using ERS.IRepositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace ERS.Repositories
{
    public class CompanyUserRepository : EfCoreRepository<ERSDbContext, CompanyUser, Guid>, ICompanyUserRepository
    {
        public CompanyUserRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }

        public Task<List<string>> GetCompanyByCompany(string company)
        {
            throw new NotImplementedException();
        }
    }
}
