using ERS.DTO.BDExp;
using ERS.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
namespace ERS.IRepositories
{
    public interface IBDExpRepository : IRepository<BdExp, Guid>
    {
        Task<List<BdExp>> GetBdExps(string company, IList<string> expcodes, IList<string> acctcodes);
        Task<IList<EXPAddSign>> GetAddSignBefore(IList<string> expcodes, string company, int category = 1);
        Task<IList<EXPAddSign>> GetAddSignAfter(IList<string> expcodes, string company, int category = 1);
        Task<List<string>> GetAcctcodeByCompany(string company);
        Task<BdExp> GetBDExpByCode(string expcode, string company);
        Task<BdExp> GetBDExpById(Guid? Id);
    }
}
