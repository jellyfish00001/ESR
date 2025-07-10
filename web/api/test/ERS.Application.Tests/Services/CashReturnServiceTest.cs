using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.BaseData;
using ERS.DTO.BDCash;
using Xunit;

namespace ERS.Services
{
    public class CashReturnServiceTest : ERSApplicationTestBase
    {
        private ICashReturnService _CashReturnService;
        public CashReturnServiceTest()
        {
            _CashReturnService = GetRequiredService<ICashReturnService>();
        }
        //[Fact(DisplayName = "查询api（公司必填）")]
        //public async Task QueryBDCash()
        //{
        //    Request<BaseDataDto> baseDataDto = new Request<BaseDataDto>()
        //    {
        //        data = new BaseDataDto()
        //        {
        //            company = new List<string>(),
        //            rno = new List<string>()
        //        },
        //        pageIndex = 1,
        //        pageSize = 10
        //    };
        //    baseDataDto.data.company.Add("WZS");
        //    Result<List<BDCashDto>> result = await _CashReturnService.QueryBDCash(baseDataDto);
        //    Assert.True(result.data.Count > 0);
        //}

        [Fact(DisplayName = "BDCash新增")]
        public async Task AddBDCash()
        {
            BDCashDto dCashDto = new BDCashDto()
            {
                company = "WZS",
                rno = "A20220922007",
                amount = 1,
                cuser = "Shaowei",
            };
            var result = await _CashReturnService.AddBDCash(dCashDto);
            Assert.True(result.status == 1);
        }

        //[Fact(DisplayName = "BDCash修改")] 
        //public async Task EditBDCash()
        //{
        //    Request<BaseDataDto> baseDataDto = new Request<BaseDataDto>()
        //    {
        //        data = new BaseDataDto()
        //        {
        //            company = new List<string>(),
        //            rno = new List<string>()
        //        },
        //        pageIndex = 1,
        //        pageSize = 10
        //    };
        //    baseDataDto.data.company.Add("WZS");
        //    baseDataDto.data.rno.Add("A22092200001");
        //    Result<List<BDCashDto>> _result = await _CashReturnService.QueryBDCash(baseDataDto);

        //    BDCashDto bDCashDto = new BDCashDto()
        //    {
        //        Id = _result.data.First().Id,
        //        amount = 23
        //    };
        //    var result = await _CashReturnService.UpdateBDCash(bDCashDto);
        //    Assert.True(result.status == 1);
        //}

        [Fact(DisplayName = "BDCash删除")] 
        public async Task DeleteBDCash()
        {
            Request<BaseDataDto> baseDataDto = new Request<BaseDataDto>()
            {
                data = new BaseDataDto()
                {
                    company = new List<string>(),
                    rno = new List<string>()
                },
                pageIndex = 1,
                pageSize = 10
            };
            baseDataDto.data.company.Add("WZS");
            baseDataDto.data.rno.Add("E22092300002");
            Result<List<BDCashDto>> _result = await _CashReturnService.QueryBDCash(baseDataDto);

            BDCashDto bDCashDto = new BDCashDto()
            {
                Id = _result.data.First().Id,
                rno = "E22092300002"
            };
            var result = await _CashReturnService.DeleteBDCash(bDCashDto);
            Assert.True(result.status == 1);
        }
    }
}