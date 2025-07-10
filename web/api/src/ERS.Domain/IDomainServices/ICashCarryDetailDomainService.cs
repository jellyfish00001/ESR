

using ERS.DTO;
using ERS.DTO.CashCarryDetail;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace ERS.IDomainServices
{
    public interface ICashCarryDetailDomainService : IDomainService
    {
        Task<Result<string>> UpdateCashCarrydetail(IFormCollection formCollection);

        Task<Result<string>> SaveCashCarryDetail(string rno, string userId);
        Task<Result<List<CashCarryDetailDto>>> GetCarryDetailByRno(string rno);
    }
}