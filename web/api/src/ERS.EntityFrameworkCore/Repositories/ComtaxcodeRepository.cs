using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERS.Entities;
using ERS.EntityFrameworkCore;
using ERS.IRepositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace ERS.Repositories
{
    public class ComtaxcodeRepository : EfCoreRepository<ERSDbContext, Comtaxcode, Guid>, IComtaxcodeRepository
    {
        public ComtaxcodeRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<List<Comtaxcode>> GetComtaxcode(string company)
        {
            return (await GetDbSetAsync()).Where(w => w.company == company).ToList();
        }
    }
}