using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
namespace ERS.DTO.AppConfig;
public interface IAppConfigService : ICrudAppService<AppConfigDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateAppConfigDto>
{
    Task<string> GetValueByKey(string key);
}
