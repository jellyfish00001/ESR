using ERS.Application.Contracts.DTO.Application;
using ERS.Common;
using ERS.DTO.Application;
using Microsoft.AspNetCore.Mvc;
using NPOI.XSSF.UserModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERS.DTO.UberFare
{
    public interface IUberTransactionalService
    {
        Task<Result<List<UberTransactionalDayDto>>> GetUberTransactional(Request<QueryUberTransactionalDto> request);

        Task<XSSFWorkbook> DownloadUberTransactional(Request<QueryUberTransactionalDto> request);

        Task TransactionToSign();

        Task<Result<UberApplicationDto>> GetQueryApplications(string rno);

        Task<Result<string>> Submit(CashUberHeadDto cashUberHeadDto);
    }
}
