using System;
using System.Threading.Tasks;
using ERS.Entities;
using Volo.Abp.Domain.Repositories;

namespace ERS.IRepositories
{
    public interface IBDTreelevelRepository : IRepository<BDTreelevel, Guid>
    {
        Task<BDTreelevel> GetBDTreelevelByLevelnum(decimal levelnum);

        Task<BDTreelevel> GetBDTreelevelAll();
    }
}