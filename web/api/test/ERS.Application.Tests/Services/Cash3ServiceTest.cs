using ERS.Application.Contracts.DTO.Application;
using ERS.Domain.Entities.Application;
using ERS.DTO;
using ERS.DTO.Application;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ERS.Services
{
    public class Cash3ServiceTest : ERSApplicationTestBase
    {
        private ICash3Service _Cash3Service;

        public Cash3ServiceTest()
        {
            _Cash3Service = GetRequiredService<ICash3Service>();
        }
        [Theory(DisplayName = "暂存一般交际费单子")]
        [InlineData("Z19081961")]
        public async Task KeepNewCash2(string user)
        {
            Dictionary<string, Microsoft.Extensions.Primitives.StringValues> dictionary = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>();
            //FormFileCollection filesForm = new FormFileCollection();
            dictionary.Add("head", "{\"cname\":\"xing\",\"deptid\":\"JML121\",\"ext\":null,\"company\":\"WZS\",\"projectcode\":null,\"currency\":\"RMB\",\"amount\":\"45,645\",\"payeeId\":\"Z19081961\",\"payeename\":\"xing\"}");
            dictionary.Add("detail", "[{\"expname\":\"活动-部门内部活动\",\"rdate\":\"2022-09-24T00:00:00\",\"summary\":\"7月5s团建\",\"revsdate\":\"2022-10-24T00:00:00\",\"payname\":\"汇款\",\"currency\":\"RMB\",\"amount1\":\"888999\",\"remarks\":\"测试备注\",\"expcode\":\"EXP10\"}]");
            dictionary.Add("file", "[{\"seq\":1,\"item\":1,\"filetype\":\"image/jpeg\",\"filename\":\"123.jpg\",\"ishead\":\"N\",\"key\":\"1\"}]");
            dictionary.Add("amount", "{\"currency\":\"RMB\",\"amount\":\"888999\",\"actamt\":\"888999\"}");
    

            // var excelFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Files", "Cash2", "123.jpg");
            //FileStream excelFS = new FileStream(excelFilePath, FileMode.Open);
            //filesForm.Add(new FormFile(excelFS, 0, excelFS.Length, "1", "123.jpg"));
            IFormCollection data = new FormCollection(dictionary);

            Result<CashResult> result = await _Cash3Service.Keep(data, user,"");
            Result<CashResult> result1= await _Cash3Service.Submit(data, user,"");
            Assert.True(result1.status == 1 && !string.IsNullOrEmpty(result1.data.rno)); 
        }

        [Theory(DisplayName = "查询预支场景（模糊查询）")]
        [InlineData("交","WZS")]
        public async Task QueryAdvanceScene(string input,string company)
        {
            var result = await _Cash3Service.GetAdvanceScene(input,company);
            Assert.NotNull(result);

        }

        [Theory(DisplayName = "查询预支场景（全部）")]
        [InlineData("WZS")]
        public async Task QueryAllAdvanceScenes(string company)
        {
            var result = await _Cash3Service.GetAllAdvanceScenes(company);
            Assert.NotNull(result);

        }

        [Theory(DisplayName = "根据expcode查询需要上传的附件类型")]
        [InlineData("EXP03", "WZS", 1)]
        public async Task GetFileCategory(string expcode, string company, int category)
        {
            var result = await _Cash3Service.GetFilecategoryByExpcode(expcode, company, category);
            Assert.True((result.data.Select(e => e.filecategory).Contains("交际费票据")));
        }

        // 获取请款方式
        [Fact(DisplayName = "获取请款方式")] 
        public async Task GetBDPayType()
        {
            var result = await _Cash3Service.GetBDPayType();
            Assert.NotEmpty(result.data);
        }


        [Theory(DisplayName = "逾期用戶返回")]
        [InlineData("Z21032798")]
        public async Task OverdueUser(string user)
        {
           List<OverdueUserDto>  overdues=(await _Cash3Service.OverdueUser(user)).data;
           Assert.True(overdues != null);   
        }
        
        [Theory(DisplayName = "预支单号模糊查询")]
        [InlineData("A","WZS")]
        public async Task FuzzyQueryAdvRno(string word,string company)
        {
            List<string> result = (await _Cash3Service.GetUnreversAdvRno(word,company)).data;
            Assert.True(result.Count > 0);
        }
    }
}
