using ERS.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
namespace ERS.IRepositories
{
    public interface IAppConfigRepository : IRepository<AppConfig, Guid>
    {
        Task<AppConfig> GetValue(string classNo);
        Task<IList<string>> GetReplaceMailBySite(string company);
        Task<bool> GetIsPMCSCodeCHK(string apid, string status);
        Task<List<string>> GetClassNoList();
        Task<string> GetPostKeyBySite(string company);
    }
}
