using ERS.DTO;
using ERS.DTO.Finreview;
using ERS.IDomainServices;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
namespace ERS.Application.Services
{
    public class FinReviewService : ApplicationService, IFinReviewService
    {
        private IApprovalDomainService _ApprovalDomainService;
        public FinReviewService(IApprovalDomainService ApprovalDomainService)
        {
            _ApprovalDomainService = ApprovalDomainService;
        }
        public async Task<Result<IList<FinReviewDto>>> GetAllFin(string user) => await _ApprovalDomainService.GetFinance(user);
        public async Task<Result<bool>> IsAccountantOrNot(string user) => await _ApprovalDomainService.IsAccountantOrNot(user);
    }
}
