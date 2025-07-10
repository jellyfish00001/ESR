using System.Threading.Tasks;
using ERS.Application.Contracts.DTO.Nickname;
using Xunit;

namespace ERS.Application.Tests.Services
{
    public class NicknameServiceTest: ERSApplicationTestBase
    {
      private INicknameService _nicknameService;
        public NicknameServiceTest(){

            _nicknameService=GetRequiredService<INicknameService>();

        }
        [Theory(DisplayName = "根据昵称或名称搜索")]
        [InlineData("员工","WZS")]
        public async Task get_nickname(string name,string company)
        {
            //Act
            var result = await _nicknameService.GetNickname(name,company);
            //Assert
            Assert.NotNull(result);
        }
        
    }
}