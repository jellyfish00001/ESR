using ERS.Localization;
using Volo.Abp.Application.Services;

namespace ERS;

/// <summary>
/// Inherit your application services from this class.
/// ERSAppService 是应用层服务的基类，主要用于提供本地化支持和统一的服务结构，同时简化了具体服务的开发。
/// 它是 ABP 框架模块化设计的一部分，确保应用服务能够高效地与框架集成
/// </summary>
public abstract class ERSAppService : ApplicationService
{
    protected ERSAppService()
    {
        LocalizationResource = typeof(ERSResource);
    }
}
