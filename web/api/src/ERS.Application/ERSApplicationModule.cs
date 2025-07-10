
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace ERS;

/// <summary>
/// ERSApplicationModule是基于ABP 框架的一个模块类，主要用于配置和初始化应用层的服务和功能：
/// 1.通过模块化设计，自动注册和管理应用层的服务
/// 2.集成 AutoMapper
/// </summary>
[DependsOn(
    typeof(ERSDomainModule),
    typeof(ERSApplicationContractsModule),
    typeof(AbpAutoMapperModule)
    )]
public class ERSApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        //在ConfigureServices 方法中，配置了 AutoMapper 的映射规则
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<ERSApplicationModule>();
        });
    }
}
