using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ERS.DTO.BDTicketRail
{
    public interface IBDTicketRailService
    {
        Task<Result<List<BDTicketRailDto>>> GetPageBDTicketRails(Request<QueryBDTicketRailDto> request);
        Task<Result<string>> AddBDTicketRail(AddBDTicketRailDto request, string userId);
        Task<Result<string>> EditIBDTicketRail(EditBDTicketRailDto request, string userId);
        Task<Result<string>> DeleteBDTicketRails(List<Guid> Ids, string userId);
        Task<Result<List<TicketRailDto>>> GetBDTicketRailsByUserCompany(string company);
    }
}