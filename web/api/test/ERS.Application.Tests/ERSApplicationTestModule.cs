using Volo.Abp.Modularity;

namespace ERS;

[DependsOn(
    typeof(ERSApplicationModule),
    typeof(ERSDomainTestModule)
    )]
public class ERSApplicationTestModule : AbpModule
{

}
