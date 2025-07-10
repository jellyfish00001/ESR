using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.BDTreelevel;

namespace ERS.IDomainServices
{
    public interface IBDTreelevelDomainService
    {
        Task<Result<List<QueryBDTreelevelDto>>> GetBDTreelevelAll();
        Task<Result<QueryBDTreelevelDto>> GetBDTreelevelByLevelnum(decimal levelnum);
    }
}