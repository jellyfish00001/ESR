using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.PapreSign;
using Xunit;

namespace ERS.Application.Tests.Services
{
    public class EFormPaperServiceTest : ERSApplicationTestBase
    {
        private IPaperService _PaperService;
        private Request<PaperQueryDto> request = new Request<PaperQueryDto>()
        {
            data = new PaperQueryDto()
            {
                emplid = ""
            }
        };
        public EFormPaperServiceTest()
        {

            _PaperService = GetRequiredService<IPaperService>();
        }

        //[Theory(DisplayName = "新增纸本单")]
        //[InlineData("E22081000999", "Z21032798", "p")]
        //public async Task addPaperTest(string rno, string user, string status)
        //{
        //    //Act
        //    var result = await _PaperService.addPaper(rno, user, status);
        //    //Assert
        //    Assert.NotNull(result.status == 1);
        //}
        [Theory(DisplayName = "根据单号查询纸本单")]
        [InlineData("E22081000998")]
        public async Task queryPaperTest(string rno)
        {
            //Act
            var result = await _PaperService.queryPaper(rno);
            //Assert
            Assert.NotNull(result);
        }
        //[Theory(DisplayName = "修改纸本单状态")]
        //[InlineData("E22081000998", "Z21032798", "A")]
        //public async Task UpdatePaperTest(string rno, string user, string status)
        //{
        //    //Act
        //    var result = await _PaperService.UpdatePaper(rno, user, status);
        //    //Assert
        //    Assert.NotNull(result.status == 1);
        //}

        //[Fact(DisplayName = "按签核人工号查询待签核纸本单")]
        //public async Task Get_UnsignPaper()
        //{
        //    request.data.emplid = "Z22070025";
        //    var result = await _PaperService.GetUnsignPaper(request);
        //    Assert.NotNull(result);
        //}

        //[Fact(DisplayName = "纸本单签核")]
        //// [InlineData("E22082700996", "Z21032798")]
        //public async Task SignPaper()
        //{
        //    List<string> rno = new List<string>(); 
        //    string emplid;
        //    rno.Add("E22082700996");
        //    emplid = "Z21032798";
        //    Result<string> result = await _PaperService.SignPaper(rno, emplid);
        //    Assert.True(result.status == 1);
        //}
    }
}