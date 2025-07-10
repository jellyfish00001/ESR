using ERS.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace ERS.IRepositories
{
    public interface IProxyCashRepository : IRepository<ProxyCash, Guid>
    {
        Task<List<string>> ReadProxyByEmplid(string emplid);
        Task<bool> ProxyIsExist(string aemplid, string remplid);
        Task<List<ProxyCash>> GetProxyCashListByIds(List<Guid?> ids);
        Task<ProxyCash> GetProxyCashById(Guid? id);
        Task<List<string>> ReadProxyByAmplid(string emplid);
    }
}
