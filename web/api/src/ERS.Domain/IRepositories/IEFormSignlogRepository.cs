using ERS.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
namespace ERS.IRepositories
{
    public interface IEFormSignlogRepository : IRepository<EFormSignlog, Guid>
    {
        Task<List<EFormSignlog>> GetListByRno(string rno);
    }
}
