using ERS.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
namespace ERS.IRepositories
{
    public interface IEFormAlistRepository : IRepository<EFormAlist, Guid>
    {
        Task<List<EFormAlist>> GetListByRno(string rno);
        Task<bool> IsSign(string rno);
    }
}
