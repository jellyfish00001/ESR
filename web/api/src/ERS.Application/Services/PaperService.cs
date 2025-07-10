using ERS.Application.Contracts.DTO.PapreSign;
using ERS.DTO;
using ERS.DTO.PapreSign;
using ERS.Entities;
using ERS.IDomainServices;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
namespace ERS.Services
{
    public class PaperService : ApplicationService, IPaperService
    {
        private IApprovalPaperDomainService _EFormPaperDomainService;
        public PaperService(IApprovalPaperDomainService EFormPaperDomainService)
        {
            _EFormPaperDomainService = EFormPaperDomainService;
        }
        public async Task<Result<List<PaperDto>>> GetUnsignPaper(Request<PaperQueryDto> request)
        {
            return await _EFormPaperDomainService.QueryUnsignPaperByemplid(request);
        }
        public async Task<PaperDto> queryPaper(string rno)
        {
            ApprovalPaper eFormPaper = await _EFormPaperDomainService.queryPaper(rno);
            PaperDto paperDto = new PaperDto();
            paperDto.rno = eFormPaper.rno;
            paperDto.status = eFormPaper.status;
            paperDto.emplid = eFormPaper.emplid;
            return paperDto;
        }
        public async Task<Result<string>> SignPaper(List<string> rno, string emplid, string token)
        {
            return await _EFormPaperDomainService.SignPaper(rno, emplid, token);
        }
    }
}
