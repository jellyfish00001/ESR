using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.BDPaperSign;
using Xunit;

namespace ERS.Services
{
    public class BDPaperSignServiceTest : ERSApplicationTestBase
    {
        private IBDPaperSignService _BPaperSignService;
        public BDPaperSignServiceTest()
        {
            _BPaperSignService = GetRequiredService<IBDPaperSignService>();
        }

        [Fact(DisplayName = "纸本单签核人添加（成功）")]
        public async Task AddBDPaperSignSuccess ()
        {
            AddPaperSignDto addPaperSignDto = new()
            {
                company = "WZS",
                company_code = "130",
                plant = "TEST",
                emplid = "Z22070025"
            };
            var result = await _BPaperSignService.AddBDPaperSign(addPaperSignDto, "Z22070025");
            Assert.True(result.status == 1);
        }

        [Fact(DisplayName = "纸本单签核人添加（失败）")]
        public async Task AddBDPaperSignFail ()
        {
            AddPaperSignDto addPaperSignDto = new()
            {
                company = "WZS",
                company_code = "130",
                plant = "UO",
                emplid = "Z21032798"
            };
            var result = await _BPaperSignService.AddBDPaperSign(addPaperSignDto, "Z22070025");
            Assert.True(result.status == 2);
        }

        [Fact(DisplayName = "纸本单签核人删除（成功）")]
        public async Task DeleteBDPaperSignSuccess()
        {
            Request<QueryPaperSignDto> request = new()
            {
                pageIndex = 1,
                pageSize = 10,
                data = new QueryPaperSignDto()
                {
                    companyList = new List<string>()
                    {
                        "WZS"
                    },
                    plant = "UO",
                    emplid = "Z21032798"
                }
            };
            var query = await _BPaperSignService.QueryBDPaperSign(request);
            List<Guid?> Ids = new();
            Ids.Add(query.data.FirstOrDefault().Id);
            var result = await _BPaperSignService.RemoveBDPaperSign(Ids);
            Assert.True(result.status == 1);
        }

        [Fact(DisplayName = "纸本单签核人删除（失败）")]
        public async Task DeleteBDPaperSignFail()
        {
            List<Guid?> Ids = new();
            Guid Id = new Guid("1a05eb90-352a-8fc3-b0f6-de0749b01a91");
            Ids.Add(Id);
            var result = await _BPaperSignService.RemoveBDPaperSign(Ids);
            Assert.True(result.status == 2);
        }

        [Fact(DisplayName = "纸本单签核人编辑（成功）")]
        public async Task EditBDPaperSignSuccess()
        {
            Request<QueryPaperSignDto> request = new()
            {
                pageIndex = 1,
                pageSize = 10,
                data = new QueryPaperSignDto()
                {
                    companyList = new List<string>()
                    {
                        "WZS"
                    },
                    plant = "UO",
                    emplid = "Z21032798"
                }
            };
            var query = await _BPaperSignService.QueryBDPaperSign(request);
            EditPaperSignDto editPaperSignDto = new()
            {
                Id = query.data.FirstOrDefault().Id,
                company = "string",
                company_code = "string",
                plant = "string",
                emplid = "string"
            };
            var result = await _BPaperSignService.EditBDPaperSign(editPaperSignDto,"Z22070025");
            Assert.True(result.status == 1);
        }

        [Fact(DisplayName = "纸本单签核人编辑（失败）")]
        public async Task EditBDPaperSignFail()
        {
            EditPaperSignDto editPaperSignDto = new()
            {
                Id = new Guid("3a05eb50-352a-8fc3-b0f6-de0749b01a91"),
                company = "string",
                company_code = "string",
                plant = "string",
                emplid = "string"
            };
            var result = await _BPaperSignService.EditBDPaperSign(editPaperSignDto,"Z22070025");
            Assert.True(result.status == 2);
        }

        [Fact(DisplayName = "纸本单签核人查询（有数据）")]
        public async Task QueryBDPaperSignSuccess()
        {
            Request<QueryPaperSignDto> request = new()
            {
                pageIndex = 1,
                pageSize = 10,
                data = new QueryPaperSignDto()
                {
                    companyList = new List<string>()
                    {
                        "WZS"
                    },
                    plant = "UO",
                    emplid = "Z21032798"
                }
            };
            var result = await _BPaperSignService.QueryBDPaperSign(request);
            Assert.True(result.data.Count > 0); 
        }

        [Fact(DisplayName = "纸本单签核人查询（无数据）")]
        public async Task QueryBDPaperSignFail()
        {
            Request<QueryPaperSignDto> request = new()
            {
                pageIndex = 1,
                pageSize = 10,
                data = new QueryPaperSignDto()
                {
                    companyList = new List<string>()
                    {
                        "ZS"
                    },
                    plant = "UO",
                    emplid = "Z21032798"
                }
            };
            var result = await _BPaperSignService.QueryBDPaperSign(request);
            Assert.True(result.data.Count == 0); 
        }
    }
}