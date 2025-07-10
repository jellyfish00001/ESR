using System;
using Volo.Abp.Application.Dtos;
namespace ERS.DTO.AppConfig;
public class AppConfigDto : AuditedEntityDto<Guid>
{
    public string key { get; set; }
    public string value { get; set; }
}
