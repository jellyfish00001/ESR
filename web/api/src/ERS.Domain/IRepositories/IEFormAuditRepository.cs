using ERS.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
namespace ERS.IRepositories
{
    public interface IEFormAuditRepository : IRepository<EFormAudit, Guid>
    {
        Task<string> GetAudit(string emplid, string company, string formcode);
        Task<EFormAudit> GetAuditById(Guid? Id);
        Task<List<EFormAudit>> GetAuditsByIds(List<Guid?> Ids);
    }
}
