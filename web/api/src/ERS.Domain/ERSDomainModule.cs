
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace ERS;

[DependsOn(
    typeof(ERSDomainSharedModule),
    typeof(AbpAutoMapperModule)
)]
public class ERSDomainModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<ERSDomainModule>();
        });
    }
}
