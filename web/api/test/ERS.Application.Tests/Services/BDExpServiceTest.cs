using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.BDExp;
using Xunit;

namespace ERS.Services
{
    public class BDExpServiceTest : ERSApplicationTestBase
    {
        private IBDExpService _BDExpService;
        public BDExpServiceTest()
        {
            _BDExpService = GetRequiredService<IBDExpService>();
        }

        [Fact(DisplayName = "費用類別查詢(有數據)")]
        public async Task QueryBDExpSuccess()
        {
            Request<BDExpParamDto> request = new()
            {
                pageIndex = 1,
                pageSize = 10,
                data = new BDExpParamDto()
                {
                    companyList = new List<string>()
                    {
                        "WZS"
                    }
                }
            };
            var result = await _BDExpService.GetPageExpenseCategory(request);
            Assert.True(result.data.Count > 0);
        }

        [Fact(DisplayName = "費用類別查詢(無數據)")]
        public async Task QueryBDExpFail()
        {
            Request<BDExpParamDto> request = new()
            {
                pageIndex = 1,
                pageSize = 10,
                data = new BDExpParamDto()
                {
                    companyList = new List<string>()
                    {
                        "WTS"
                    }
                }
            };
            var result = await _BDExpService.GetPageExpenseCategory(request);
            Assert.True(result.data.Count == 0);
        }

        [Fact(DisplayName = "新增費用類別（成功）")]
        public async Task AddBDExpSuccess()
        {
            AddBDExpDto bDExpFormDto = new()
            {
                companycategory = "WZS",
                expcode = "EXP002",
                expname = "測試費用類別名",
                auditlevelcode = "B1",
                acctcode = "15031000",
                senarioname = "測試描述",
                wording = "test"
            };
            var result = await _BDExpService.AddExpenseCategory(bDExpFormDto, "Z22070025");
            Assert.True(result.status == 1);
        }

        [Fact(DisplayName = "新增費用類別（失敗）")]
        public async Task AddBDExpFail()
        {
            AddBDExpDto bDExpFormDto = new()
            {
                companycategory = "",
                expcode = "EXP002",
                expname = "測試費用類別名",
                auditlevelcode = "B1",
                acctcode = "15031000",
                senarioname = "測試描述",
                wording = "test"
            };
            var result = await _BDExpService.AddExpenseCategory(bDExpFormDto, "Z22070025");
            Assert.True(result.status == 2);
        }

        [Fact(DisplayName = "删除費用類別（成功）")]
        public async Task DeleteBDExpSuccess()
        {
            Request<BDExpParamDto> request = new()
            {
                pageIndex = 1,
                pageSize = 10,
                data = new BDExpParamDto()
                {
                    companyList = new List<string>()
                    {
                        "WZS"
                    }
                }
            };
            var query = await _BDExpService.GetPageExpenseCategory(request);
            Guid? id = query.data.FirstOrDefault().Id;
            var result = await _BDExpService.DeleteExpenseCategory(query.data.FirstOrDefault());
            Assert.True(result.status == 1);
        }

        [Fact(DisplayName = "删除費用類別（失敗）")]
        public async Task DeleteBDExpFail()
        {
            BDExpFormDto bDExpParamDto = new()
            {
                Id = new Guid("431881d6-5454-b1a8-2640-009ed123e444")
            };
            var result = await _BDExpService.DeleteExpenseCategory(bDExpParamDto);
            Assert.True(result.status == 2);
        }

        [Fact(DisplayName = "編輯費用類別（成功）")]
        public async Task EditBDExpSuccess()
        {
            Request<BDExpParamDto> request = new()
            {
                pageIndex = 1,
                pageSize = 10,
                data = new BDExpParamDto()
                {
                    companyList = new List<string>()
                    {
                        "WZS"
                    }
                }
            };
            var query = (await _BDExpService.GetPageExpenseCategory(request)).data.FirstOrDefault();

            EditBDExpDto editBDExpDto = new EditBDExpDto()
            {
                Id = query.Id,
                acctcode = "test",
                expname = query.expname,
                auditlevelcode = query.auditlevelcode,
                //description = query.description
            };

            var result = (await _BDExpService.EditExpenseCategory(editBDExpDto, "Z22070025"));
            Assert.True(result.status == 1);
        }

        [Fact(DisplayName = "編輯費用類別（失敗）")]
        public async Task EditBDExpFail()
        {
            Request<BDExpParamDto> request = new()
            {
                pageIndex = 1,
                pageSize = 10,
                data = new BDExpParamDto()
                {
                    companyList = new List<string>()
                    {
                        "WZS"
                    }
                }
            };
            var query = (await _BDExpService.GetPageExpenseCategory(request)).data.FirstOrDefault();
            query.acctcode = "";

            EditBDExpDto editBDExpDto = new EditBDExpDto()
            {
                Id = new Guid("e588fb00-f640-476b-8ca7-8b83ac0acf49"),
                acctcode = "t"
            };

            var result = (await _BDExpService.EditExpenseCategory(editBDExpDto, "Z22070025"));
            Assert.True(result.status == 2);
        }
    }
}