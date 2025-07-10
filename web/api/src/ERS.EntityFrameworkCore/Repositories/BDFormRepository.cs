using System;
using System.Linq;
using System.Threading.Tasks;
using ERS.Entities;
using ERS.EntityFrameworkCore;
using ERS.IRepositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace ERS.Repositories
{
    public class BDFormRepository : EfCoreRepository<ERSDbContext, BDForm, Guid>, IBDFormRepository
    {
        public BDFormRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }
        public async Task<string> GetNameByFormcode(string formcode) => await (await GetDbSetAsync()).Where(i => i.FormCode == formcode).Select(w => w.FormName).FirstOrDefaultAsync();
    }
}
