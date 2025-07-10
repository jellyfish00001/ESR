using ERS.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace ERS.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(ERSDomainModule),
    typeof(ERSEntityFrameworkCoreModule),
    typeof(ERSApplicationContractsModule)
    )]
public class ERSDbMigratorModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        
    }
}
