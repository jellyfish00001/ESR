using ERS.DTO.AppConfig;
using ERS.Entities;
using ERS.Localization;
using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using System.Threading.Tasks;
namespace ERS.Services.Config;
public class AppConfigService : CrudAppService<AppConfig, AppConfigDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateAppConfigDto>, IAppConfigService
//public class AppConfigService : ApplicationService, IAppConfigService
{
    private IRepository<AppConfig, Guid> _repository;
    public AppConfigService(IRepository<AppConfig, Guid> repository) : base(repository)
    {
        _repository = repository;
        LocalizationResource = typeof(ERSResource);
    }
    //public AppConfigService(IRepository<AppConfig, Guid> repository)
    //{
    //    _repository = repository;
    //    LocalizationResource = typeof(ERSResource);
    //}
    public async Task<string> GetValueByKey(string key)
    {
        return (await _repository.FirstOrDefaultAsync(i=>i.key==key))?.value;
    }
}
