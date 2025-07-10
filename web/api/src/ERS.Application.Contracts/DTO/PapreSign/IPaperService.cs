using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Application.Contracts.DTO.PapreSign;
using ERS.DTO.PapreSign;
namespace ERS.DTO
{
    public interface IPaperService
    {
        //根据单号查询纸本单
        Task<PaperDto> queryPaper(string rno);
        public Task<Result<List<PaperDto>>> GetUnsignPaper(Request<PaperQueryDto> request);
        //签核纸本单
        public Task<Result<string>> SignPaper(List<string> rno, string emplid, string token);
    }
}
