using ERS.DTO;
using ERS.DTO.BDExpenseSenario;
using ERS.DTO.BDExp;
using ERS.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
namespace ERS.IRepositories
{
    public interface IBDExpenseSenarioRepository : IRepository<BDExpenseSenario, Guid>
    {
        Task<List<BDExpenseSenario>> GetBdExps(string company, IList<string> expcodes, IList<string> acctcodes);
        //Task<IList<EXPAddSign>> GetAddSignBefore(IList<string> expcodes, string company, string category);
        //Task<IList<EXPAddSign>> GetAddSignAfter(IList<string> expcodes, string company, string category);
        Task<List<string>> GetAcctcodeByCompany(string company);
        Task<BDExpenseSenario> GetBDExpByCode(string expcode, string company);
        Task<BDExpenseSenario> GetBDExpById(Guid? Id);

        Task<BDSenarioDto> GetBDSenarioById(Guid Id);
        Task<List<BDExpenseSenario>> SearchBDSenariosByKeywordAsync(string companyCategory, string keyword);
        Task<PagedResultDto<BDSenarioDto>> GetBDSenarios(Request<BDSenarioFilterDto> request);
    }
}
