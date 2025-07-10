using System.Collections.Generic;
using System.Threading.Tasks;
namespace ERS.DTO.Finreview
{
    public interface IFinReviewService
    {
        Task<Result<IList<FinReviewDto>>> GetAllFin(string user);
        Task<Result<bool>> IsAccountantOrNot(string user);
    }
}
