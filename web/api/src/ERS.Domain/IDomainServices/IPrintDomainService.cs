using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.Print;
using Volo.Abp.Domain.Services;
namespace ERS.IDomainServices
{
    public interface IPrintDomainService : IDomainService
    {
        Task<Result<List<PrintDto>>> QueryPagePrint(Request<PrintQueryDto> request);
        Task<string> EntertainmentExpPrintAsync(string rno, string token);
        Task<string> CateringGuestsPrintAsync(string rno, string token);
        Task<string> AdvancePaymentPrintAsync(string rno, string token);
        Task<string> GENCommExpPrintAsync(string rno, string token);
        Task<string> BatchReimbursementPrintAsync(string rno, string token);
        Task<string> ReturnTaiwanMeetingPrint(string rno, string token);
        Task<string> PayrollRequestPrintAsync(string rno, string token);
    }
}