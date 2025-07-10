using ERS.Entities;
using ERS.EntityFrameworkCore;
using ERS.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace ERS.Repositories
{
    public class ProxyCashRepository : EfCoreRepository<ERSDbContext, ProxyCash, Guid>, IProxyCashRepository
    {
        public ProxyCashRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }

        public async Task<List<string>> ReadProxyByEmplid(string emplid) => await (await GetDbSetAsync()).Where(i => i.remplid == emplid).Select(i => i.aemplid).AsNoTracking().Distinct().ToListAsync();
        public async Task<List<string>> ReadProxyByAmplid(string emplid) => await (await GetDbSetAsync()).Where(i => i.aemplid == emplid).Select(i => i.remplid).AsNoTracking().Distinct().ToListAsync();

        public async Task<bool> ProxyIsExist(string aemplid, string remplid)
        {
            bool result = false;
            var query = (await GetDbSetAsync()).Where(w => w.aemplid == aemplid && w.remplid == remplid).FirstOrDefault();
            if(query != null)
            {
                result = true;
            }
            return result;
        }
        public async Task<List<ProxyCash>> GetProxyCashListByIds(List<Guid?> ids)
        {
            return (await GetDbSetAsync()).Where(w => ids.Contains(w.Id)).ToList();
        }
        public async Task<ProxyCash> GetProxyCashById(Guid? id)
        {
            return (await GetDbSetAsync()).Where(w => w.Id == id).FirstOrDefault();
        }
    }
}
