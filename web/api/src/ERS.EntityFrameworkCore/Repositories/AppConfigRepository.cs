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
    public class AppConfigRepository : EfCoreRepository<ERSDbContext, AppConfig, Guid>, IAppConfigRepository
    {
        public AppConfigRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }


        public async Task<AppConfig> GetValue(string classNo)
        {
            return (await GetDbSetAsync()).Where(b => b.key.Equals(classNo)).FirstOrDefault();
        }

        public async Task<IList<string>> GetReplaceMailBySite(string company)
        {
            string value = await (await GetDbSetAsync()).Where(i => i.key == "replace_mail" && i.company == company).AsNoTracking().Select(i => i.value).FirstOrDefaultAsync();
            return value?.Split(",")?.Where(i => !string.IsNullOrEmpty(i)).ToList();
        }


        public async Task<string> GetPostKeyBySite(string company)
        {
            string value = await (await GetDbSetAsync()).Where(i => i.key == "postKey" && i.company == company).AsNoTracking().Select(i => i.value).FirstOrDefaultAsync();
            return value;
        }

        public async Task<bool> GetIsPMCSCodeCHK(string apid, string status)
        {
            bool result = await (await GetDbSetAsync()).Where(w => w.key == apid && w.value == status).FirstOrDefaultAsync() == null ? false : true;
            return result;
        }
        public async Task<List<string>> GetClassNoList()
        {
            string query =  await (await GetDbSetAsync()).Where(w => w.key.Equals("Class_No")).Select(w => w.value).FirstOrDefaultAsync();
            List<string> result = new List<string>();
            if(!string.IsNullOrEmpty(query))
            {
                result = query.Split('/').Distinct().ToList();
            }
            return result;
        }
    }
}
