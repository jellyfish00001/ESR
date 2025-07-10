using ERS.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace ERS;

[DependsOn(
    typeof(ERSEntityFrameworkCoreTestModule),
    typeof(ERSDomainModule),
    typeof(ERSEntityFrameworkCoreModule)
    )]
public class ERSDomainTestModule : AbpModule
{

}
