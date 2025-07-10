using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERS.DTO.BDTreelevel
{
    public interface IBDTreelevelService
    {
        Task<Result<List<QueryBDTreelevelDto>>> GetBDTreelevelAll();
        Task<Result<QueryBDTreelevelDto>> GetBDTreelevelByLevelnum(decimal levelnum);
    }
}