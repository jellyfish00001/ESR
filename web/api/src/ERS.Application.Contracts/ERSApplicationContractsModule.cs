using Volo.Abp.Modularity;
namespace ERS;
[DependsOn(
    typeof(ERSDomainSharedModule)
)]
public class ERSApplicationContractsModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        ERSDtoExtensions.Configure();
    }
}
