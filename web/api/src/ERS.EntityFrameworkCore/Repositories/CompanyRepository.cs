using ERS.Entities;
using ERS.EntityFrameworkCore;
using ERS.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace ERS.Repositories
{
    public class CompanyRepository : EfCoreRepository<ERSDbContext, BDCompanyCategory, Guid>, ICompanyRepository
    {
        public CompanyRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }

        public async Task<string> demo()
        {
            return await (await GetDbSetAsync()).Select(i => i.company).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<string> GetStarwith(string company)
        {
            return await (await GetDbSetAsync()).Where(w => w.company == company).Select(w => w.Stwit).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<string> GetCurrency(string company)
        {
            return (await GetDbSetAsync()).Where(w => w.company == company).Select(w => w.BaseCurrency).FirstOrDefault();
        }

        public async Task<List<BDCompanyCategory>> GetComInfoByCompany(string company)
        {
            return (await GetDbSetAsync()).Where(w => w.company == company).ToList();
        }

        public async Task<BDCompanyCategory> GetCompanyByCode(string company, string companycode)
        {
            return (await GetDbSetAsync()).Where(w => w.company == company && w.CompanyCategory == companycode).FirstOrDefault();
        }

        public async Task<BDCompanyCategory> GetCompanyById(Guid? Id)
        {
            return (await GetDbSetAsync()).Where(w => w.Id == Id).FirstOrDefault();
        }

        //方法有误，现已废弃
        //public async Task<string> GetCompanyByCode(string companycode)
        //{
        //    return (await GetDbSetAsync()).Where(w => w.CompanyCategory == companycode).FirstOrDefault().company;
        //}

        public async Task<string> GetAreaByCode(string companycode)
        {
            return await (await GetDbSetAsync()).Where(w => w.CompanyCategory == companycode).Select(s => s.Area).FirstOrDefaultAsync();
        }
    }
}
