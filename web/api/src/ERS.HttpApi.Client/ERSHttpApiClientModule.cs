using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace ERS;

[DependsOn(
    typeof(ERSApplicationContractsModule)
)]
public class ERSHttpApiClientModule : AbpModule
{
    public const string RemoteServiceName = "Default";

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        //context.Services.AddHttpClientProxies(
        //    typeof(ERSApplicationContractsModule).Assembly,
        //    RemoteServiceName
        //);

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<ERSHttpApiClientModule>();
        });
    }
}
