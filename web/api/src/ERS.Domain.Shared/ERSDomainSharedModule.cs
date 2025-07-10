using ERS.Localization;
using ERS.Middleware;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace ERS;

[DependsOn()]
public class ERSDomainSharedModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        ERSGlobalFeatureConfigurator.Configure();
        ERSModuleExtensionConfigurator.Configure();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<ERSDomainSharedModule>();
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<ERSResource>("zh-TW") //默认语言设置，这个culture名称要和本地JSON文件（如 zh.json）中的 "culture" 字段一致
                .AddBaseTypes(typeof(AbpValidationResource))
                .AddVirtualJson("/Localization/ERS");

            options.DefaultResourceType = typeof(ERSResource);
        });

        Configure<AbpExceptionLocalizationOptions>(options =>
        {
            options.MapCodeNamespace("ERS", typeof(ERSResource));
        });
        context.Services.AddTransient<RequestContextMiddleware>();
    }
}
