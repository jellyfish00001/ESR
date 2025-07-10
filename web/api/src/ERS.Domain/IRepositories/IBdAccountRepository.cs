using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Entities;
using Volo.Abp.Domain.Repositories;
namespace ERS.IRepositories
{
    public interface IBdAccountRepository : IRepository<BdAccount,Guid>
    {
        Task<List<string>> GetAcctcodeByCompany(string company);
        Task<BdAccount> GetBdAccount(string acctcode, string company);
    }
}