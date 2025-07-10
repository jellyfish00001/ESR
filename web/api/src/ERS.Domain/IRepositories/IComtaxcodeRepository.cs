using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Entities;
using Volo.Abp.Domain.Repositories;
namespace ERS.IRepositories
{
    public interface IComtaxcodeRepository : IRepository<Comtaxcode,Guid>
    {
        Task<List<Comtaxcode>> GetComtaxcode(string company);
    }
}