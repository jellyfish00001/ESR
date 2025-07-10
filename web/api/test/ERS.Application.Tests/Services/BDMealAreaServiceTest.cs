using System.IO;
using System.Threading.Tasks;
using ERS.DTO.BDMealArea;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace ERS.Services
{
    public class BDMealAreaServiceTest : ERSApplicationTestBase
    {
        private IBDMealAreaService _BDMealAreaService;
        public BDMealAreaServiceTest()
        {
            _BDMealAreaService = GetRequiredService<IBDMealAreaService>();
        }

        [Fact(DisplayName = "批量上傳公出城市")]
        public async Task BatchUploadSucceed()
        {
            // 誤餐費地區標準 = Regional Standard for Meal Allowance
            var excelFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Files", "Cash4", "Regional Standard for Meal Allowance.xlsx");
            FileStream excelFS = new FileStream(excelFilePath, FileMode.Open);
            FormFile eFile = new FormFile(excelFS, 0, excelFS.Length, "1", "Regional Standard for Meal Allowance.xlsx");
            eFile.Headers = new HeaderDictionary();
            eFile.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var result = await _BDMealAreaService.BatchUploadBDMealArea(eFile,"Z22070025");
            Assert.True(result.status == 1);
        }
    }
}