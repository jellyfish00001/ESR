using ERS.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
namespace ERS.IRepositories
{
    public interface IEFormHeadRepository : IRepository<EFormHead, Guid>
    {
        Task<EFormHead> GetByNo(string rno);
          Task<bool> IsSigned(string rno);
        Task<EFormHead> GetByCuser(string cuser);
        Task<IList<EFormHead>> GetEFormHeadList(List<string> rnolist);
    }
}
