using System.Threading.Tasks;
using ERS.DTO.Finreview;
using Xunit;

namespace ERS.Services
{
    public class FinReviewServiceTest : ERSApplicationTestBase
    {
        private IFinReviewService _FinReviewService;
        public FinReviewServiceTest()
        {
            _FinReviewService = GetRequiredService<IFinReviewService>();
        }
        
        //[Fact(DisplayName = "判斷是否為會計")]
        //public async Task DetermineAccountant()
        //{
        //    var result = await _FinReviewService.IsAccountantOrNot("Z22070025");
        //    Assert.True(result == false);
        //}

        [Fact(DisplayName = "獲取全部簽核會計")]
        public async Task GetAllFinSuccess()
        {
            var result = await _FinReviewService.GetAllFin("Z19081961");
            Assert.True(result.data.Count > 0);
        }
    }
}