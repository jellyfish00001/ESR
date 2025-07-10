using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.BDTreelevel;
using ERS.IDomainServices;
using Volo.Abp.Application.Services;

namespace ERS.Services
{
    public class BDTreelevelService : ApplicationService, IBDTreelevelService
    {
        private IBDTreelevelDomainService _BDTreelevelDomainService;

        public BDTreelevelService(IBDTreelevelDomainService BDTreelevelDomainService)
        {
            _BDTreelevelDomainService = BDTreelevelDomainService;
        }

        public async Task<Result<List<QueryBDTreelevelDto>>> GetBDTreelevelAll()
        {
            return await _BDTreelevelDomainService.GetBDTreelevelAll();
        }

        public async Task<Result<QueryBDTreelevelDto>> GetBDTreelevelByLevelnum(decimal levelnum)
        {
            return await _BDTreelevelDomainService.GetBDTreelevelByLevelnum(levelnum);
        }
    }
}