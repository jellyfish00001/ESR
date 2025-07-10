using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.BDTicketRail;
using ERS.IDomainServices;
using Volo.Abp.Application.Services;
namespace ERS.Services
{
    public class BDTicketRailService : ApplicationService, IBDTicketRailService
    {
        private IBDTicketRailDomainService _BDTicketRailDomainService;
        public BDTicketRailService(
            IBDTicketRailDomainService BDTicketRailDomainService
        )
        {
            _BDTicketRailDomainService = BDTicketRailDomainService;
        }

        public async Task<Result<List<BDTicketRailDto>>> GetPageBDTicketRails(Request<QueryBDTicketRailDto> request)
        {
            return await _BDTicketRailDomainService.GetPageBDTicketRails(request);
        }

        public async Task<Result<string>> AddBDTicketRail(AddBDTicketRailDto request, string userId)
        {
            return await _BDTicketRailDomainService.AddBDTicketRail(request, userId);
        }

        public async Task<Result<string>> EditIBDTicketRail(EditBDTicketRailDto request, string userId)
        {
            return await _BDTicketRailDomainService.EditBDTicketRail(request, userId);
        }

        public async Task<Result<string>> DeleteBDTicketRails(List<Guid> Ids, string userId)
        {
            return await _BDTicketRailDomainService.DeleteBDTicketRails(Ids, userId);
        }

        public async Task<Result<List<TicketRailDto>>> GetBDTicketRailsByUserCompany(string company)
        {
            return await _BDTicketRailDomainService.GetBDTicketRailsByUserCompany(company);
        }
    }
}