using Localization.Resources.AbpUi;
using ERS.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Autofac;
using Microsoft.Extensions.DependencyInjection;
using ERS.Services;

namespace ERS;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpAspNetCoreMvcModule),
    typeof(ERSApplicationModule),
    typeof(ERSApplicationContractsModule)
    )]
public class ERSHttpApiModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        ConfigureLocalization();
        context.Services.AddTransient<IDemoService, DemoService>();
        Configure<AbpAspNetCoreMvcOptions>(options =>
        {
            options
                .ConventionalControllers
                .Create(typeof(ERSHttpApiModule).Assembly, opts =>
                {
                    opts.RootPath = "api/1.0";
                });
        });
    }

    private void ConfigureLocalization()
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<ERSResource>()
                .AddBaseTypes(
                    typeof(AbpUiResource)
                );
        });
    }
}
