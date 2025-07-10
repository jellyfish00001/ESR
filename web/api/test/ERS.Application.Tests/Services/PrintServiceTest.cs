using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.Print;
using Xunit;

namespace ERS.Services
{
    public class PrintServiceTest : ERSApplicationTestBase
    {
        private IPrintService _printService;
        private Request<PrintQueryDto> request = new Request<PrintQueryDto>()
        {
            data = new PrintQueryDto()
            {
                startdate = null,
                enddate = null,
                rno = "",
                cuser = "",
                status = new List<string>(),
                formcode = new List<string>(),
                company = new List<string>()
            }
        };
        public PrintServiceTest()
        {
            _printService = GetRequiredService<IPrintService>();
        }

        [Fact]
        public async Task Get_PagePrints()
        {
            // Given
            var result = await _printService.GetQueryPagePrint(request);
            // When
            Assert.True(result.data.Count > 0);
            // Then
        }

        /// <summary>
        /// 獲取列印html測試
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetPrintHtml()
        {
            List<string> rnolist = new List<string>()
            {
                "C1108120010",
                "E22083000010",
                "E22082600005",
                "E22093000002"
            };
            var result = await _printService.GetPrintAsync(rnolist, "test token");
            Assert.NotNull(result);
        }
    }
}