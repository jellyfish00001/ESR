using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Application.Contracts.DTO.Application;
using ERS.DTO;
using ERS.DTO.Application;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace ERS.Services
{
    public class Cash3AServiceTest : ERSApplicationTestBase
    {
        private ICash3AService _Cash3AService;
        public Cash3AServiceTest()
        {
            _Cash3AService = GetRequiredService<ICash3AService>();
        }

          [Theory(DisplayName = "暂存與提交預支金延期單據")]
        [InlineData("Z19081961")]
        public async Task KeepNewCash2(string user)
        {
            Dictionary<string, Microsoft.Extensions.Primitives.StringValues> dictionary = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>();
            //FormFileCollection filesForm = new FormFileCollection();
            dictionary.Add("head", "{\"cname\":\"xing\",\"deptid\":\"JML121\",\"ext\":null,\"company\":\"WZS\",\"projectcode\":null,\"currency\":\"RMB\",\"amount\":\"45,645\",\"payeeId\":\"Z19081961\",\"payeename\":\"xing\"}");
            dictionary.Add("detail", "[{\"expname\":\"活动-部门内部活动\",\"rdate\":\"2022-09-24T00:00:00\",\"summary\":\"7月5s团建\",\"revsdate\":\"2022-10-24T00:00:00\",\"payname\":\"汇款\",\"currency\":\"RMB\",\"amount1\":\"888999\",\"remarks\":\"测试备注\",\"delaydays\": 3,\"delayreason\":\"延期原因\"}]");
            IFormCollection data = new FormCollection(dictionary);
            Result<CashResult> result = await _Cash3AService.Keep(data, user,"");
            Result<CashResult> result1= await _Cash3AService.Submit(data, user,"");
            Assert.True(result1.status == 1 && !string.IsNullOrEmpty(result1.data.rno)); 
        }

        [Theory(DisplayName = "查詢預支金延期數據")]
        [InlineData("Z21032798")]
        public async Task DeferredQuery(string user)
        {
           List<DeferredDto>   dtos= (await  _Cash3AService.DeferredQuery(user)).data;
        Assert.True(dtos.Count == 0);
        }

    }
}