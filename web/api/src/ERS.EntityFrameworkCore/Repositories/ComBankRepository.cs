using System;
using System.Linq;
using System.Threading.Tasks;
using ERS.Entities.Bank;
using ERS.EntityFrameworkCore;
using ERS.IRepositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace ERS.Repositories
{
    public class ComBankRepository : EfCoreRepository<ERSDbContext, ComBank, Guid>, IComBankRepository
    {
        public ComBankRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<string> GetVenderCode(string company, string bank)
        {
            return (await GetDbSetAsync()).Where(w => w.company == company && w.banid == bank).Select(w => w.vendercode).FirstOrDefault();
        }
    }
}