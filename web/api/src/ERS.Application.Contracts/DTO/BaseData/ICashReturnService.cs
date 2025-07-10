using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO.BDCash;
namespace ERS.DTO.BaseData
{
    public interface ICashReturnService
    {
        Task<Result<List<BDCashDto>>> QueryBDCash(Request<BaseDataDto> parameters);
        Task<Result<BDCashDto>> AddBDCash(BDCashDto parameters);
        Task<Result<BDCashDto>> UpdateBDCash(BDCashDto parameters);
        Task<Result<string>> DeleteBDCash(BDCashDto request);
    }
}