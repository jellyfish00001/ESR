using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ERS.DTO.BDPaperSign
{
    public interface IBDPaperSignService
    {
        Task<Result<string>> AddBDPaperSign(AddPaperSignDto request, string userId);
        Task<Result<string>> RemoveBDPaperSign(List<Guid?> Ids);
        Task<Result<string>> EditBDPaperSign(EditPaperSignDto request, string userId);
        Task<Result<List<PaperSignDto>>> QueryBDPaperSign(Request<QueryPaperSignDto> request);
    }
}