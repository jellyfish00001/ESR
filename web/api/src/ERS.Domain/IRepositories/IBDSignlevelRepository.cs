using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Entities;
using Volo.Abp.Domain.Repositories;

namespace ERS.IRepositories
{
    public interface IBDSignlevelRepository : IRepository<BDSignlevel, Guid>
    {
        Task<BDSignlevel> GetBDSignlevelByCompany(string company);

        Task<BDSignlevel> GetBDSignlevel(string company, string item, string signlevel);

        Task<List<BDSignlevel>> GetBDSignlevelByIds(List<Guid?> Ids);

        Task<List<BDSignlevel>> GetBDSignlevelByCompanyandMoney(string company, decimal money);
    }
}