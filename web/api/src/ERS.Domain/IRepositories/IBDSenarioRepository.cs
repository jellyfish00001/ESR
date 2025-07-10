using ERS.DTO.BDExpenseSenario;
using ERS.DTO;
using ERS.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
namespace ERS.IRepositories
{
    public interface IBDSenarioRepository : IRepository<BDExpenseSenario, Guid>
    {
        Task<BDSenarioDto> GetBDSenarioById(Guid Id);
        Task<List<BDExpenseSenario>> SearchBDSenariosByKeywordAsync(string companyCategory, string keyword);
        Task<PagedResultDto<BDSenarioDto>> GetBDSenarios(Request<BDSenarioFilterDto> request);
    }
}
