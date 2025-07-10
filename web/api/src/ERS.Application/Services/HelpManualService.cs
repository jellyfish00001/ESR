using ERS.DTO;
using ERS.Entities;
using ERS.IDomainServices;
using ERS.Localization;
using ERS.Minio;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;
namespace ERS.Services
{
    public class HelpManualService : ApplicationService, IHelpManualService
    {
        private IRepository<HelpManual, Guid> _repository;
        private IObjectMapper _ObjectMapper;
        private IMinioRepository _MinioRepository;
        private IMinioDomainService _minioDomainService;
        public HelpManualService(IRepository<HelpManual, Guid> repository, IObjectMapper ObjectMapper, IMinioRepository MinioRepository, IMinioDomainService minioDomainService)
        {
            _repository = repository;
            LocalizationResource = typeof(ERSResource);
            _ObjectMapper = ObjectMapper;
            _MinioRepository = MinioRepository;
            _minioDomainService = minioDomainService;
        }
        public async Task<Result<List<HelpManualDto>>> Get(string company, string userId)
        {
            Result<List<HelpManualDto>> result = new Result<List<HelpManualDto>>();
            string area = await _minioDomainService.GetMinioArea(userId);

            List<HelpManual> helpManuals = (await _repository.WithDetailsAsync()).Where(i => i.company.Contains(company)).AsNoTracking().ToList();
            foreach (HelpManual helpManual in helpManuals)
            {
                helpManual.url = !string.IsNullOrEmpty(helpManual.path) ? await _MinioRepository.PresignedGetObjectAsync(helpManual.path,area) : helpManual.url;
            }
            List<HelpManualDto> data = _ObjectMapper.Map<List<HelpManual>, List<HelpManualDto>>(helpManuals);
            result.data = data;
            return result;
        }
    }
}
