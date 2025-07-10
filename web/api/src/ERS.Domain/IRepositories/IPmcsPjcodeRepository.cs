using ERS.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace ERS.IRepositories
{
    public interface IPmcsPjcodeRepository : IRepository<PmcsPjCode, Guid>
    {
        Task<List<PmcsPjCode>> QueryPjcode(string code, string company);
    }
}
