using ERS.DTO;
using ERS.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
namespace ERS.IDomainServices
{
    public interface IBPMDomainService : IDomainService
    {
        Task<Result<string>> BPMTransform(SignDto data, string cuser, string token);
        Task<Result<string>> BPMCancel(SignDto data, string cuser, string token);
        Task<Result<IList<SignForm>>> BPMQuerySign(string rno, string formcode, string token);
        Task<Result<string>> BPMCreateSign(CreateSign data, string token);
        Task<Result<bool>> BPMQueryIsSigned(string rno, string formcode, string token);
        Task<Result<bool>> BPMQueryHaveSign(string rno, string formcode, string token);
        Task<Result<IList<SignForm>>> GetBPMSelfPendingForm(string userid, string token);
        Result<bool> BPMQueryIsSigned(Result<IList<SignForm>> data);
    }
}
