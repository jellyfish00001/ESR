using ERS.Entities;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace ERS.IRepositories
{
    public interface IMailtemplateRepository : IRepository<Mailtemplate, Guid>
    {
        Task<Mailtemplate> GetNextApply(string company);
        Task<Mailtemplate> GetFinishApply(string company);
        Task<Mailtemplate> GetRejectApply(string company);
        Task<Mailtemplate> GetPaymentTemplate(string company);
    }
}
