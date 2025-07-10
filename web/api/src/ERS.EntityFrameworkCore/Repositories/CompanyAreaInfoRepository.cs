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
    public class CompanyAreaInfoRepository : EfCoreRepository<ERSDbContext, CompanyAreaInfo, Guid>, ICompanyAreaInfoRepository
    {
        public CompanyAreaInfoRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }

        public Task<List<CompanyAreaInfo>> GetComInfoBycompanyCode(string companyCode)
        {
            throw new NotImplementedException();
        }

        public Task<List<string>> GetCompanyByCompanyCodeAndSite(string companyCode, string site)
        {
            throw new NotImplementedException();
        }
    }
}
