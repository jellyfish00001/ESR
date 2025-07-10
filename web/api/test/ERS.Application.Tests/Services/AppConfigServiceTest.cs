using ERS.DTO.AppConfig;
using System.Threading.Tasks;
using Xunit;

namespace ERS.Services;
public class AppConfigServiceTest : ERSApplicationTestBase
{
    private readonly IAppConfigService _AppConfigService;

    public AppConfigServiceTest()
    {
        _AppConfigService = GetRequiredService<IAppConfigService>();
    }

    [Fact(DisplayName = "根据key值来获取配置")]
    public async Task get_value_by_key()
    {
        //Act
        var result = await _AppConfigService.GetValueByKey("123");

        //Assert
        Assert.NotNull(result);
    }
}
