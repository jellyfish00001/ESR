using System;
using ERS.DTO;
using ERS.DTO.Application;
using Xunit;

namespace ERS.Services
{
    public class CashServiceTest : ERSApplicationTestBase
    {
        private ICashService _CashService;
        public CashServiceTest()
        {
            _CashService = GetRequiredService<ICashService>();
        }

        [Fact(DisplayName = "查询历史签核数据（单号存在）")]
        public async void GetHistoricalSignRecords()
        {
            var result = await _CashService.GetHistoricalSignRecords("C1108120010");
            Assert.True(result.data.Count > 0);
        }

        [Fact(DisplayName = "查询历史签核数据（单号不存在）")]
        public async void GetHistoricalSignRecordsError()
        {
            var result = await _CashService.GetHistoricalSignRecords("123");
            Assert.True(result.data.Count == 0);
        }

        [Fact(DisplayName = "查询需签核数据（单号存在）")]
        public async void GetSignedData()
        {
            var result = await _CashService.GetSignedData("E22102800001");
            Assert.True(result.data.Count > 0);
        }

        [Fact(DisplayName = "查询需签核数据（单号不存在）")]
        public async void GetSignedDataError()
        {
            var result = await _CashService.GetSignedData("123");
            Assert.True(result.data.Count == 0);
        }

        [Fact(DisplayName = "根据当前登录人获取待签的单子api（分页），按申请时间升序排")]
        public async void GetUnSignedByCemplid()
        {
            string userId = "Z19081961";
            Request<string> parameters = new Request<string>();
            var result = await _CashService.QuerySignedFormByUserId(parameters,userId);
            Console.WriteLine(result);
            Assert.True(result.data.Count > 0);
        }
    }
}