using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.BDTicketRail;
namespace ERS.IDomainServices
{
    public interface IBDTicketRailDomainService
    {
        Task<Result<List<BDTicketRailDto>>> GetPageBDTicketRails(Request<QueryBDTicketRailDto> request);
        Task<Result<string>> AddBDTicketRail(AddBDTicketRailDto request, string userId);
        Task<Result<string>> EditBDTicketRail(EditBDTicketRailDto request, string userId);
        Task<Result<string>> DeleteBDTicketRails(List<Guid> Ids, string userId);
        Task<Result<List<TicketRailDto>>> GetBDTicketRailsByUserCompany(string company);
    }
}