using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Application.Contracts.DTO.PJcode;
using ERS.Application.Contracts.DTO.PmcsPjcode;
using Volo.Abp.Application.Services;
using ERS.Entities;
using ERS.IRepositories;
using System.Linq;
namespace ERS.Application.Services
{
    public class PmcsPjcodeService : ApplicationService, IPmcsPjcodeService
    {
        private IPmcsPjcodeRepository _PmcsPjcodeRepository;
        public PmcsPjcodeService(IPmcsPjcodeRepository PmcsPjcodeRepository)
        {
            _PmcsPjcodeRepository = PmcsPjcodeRepository;
        }
        public async Task<List<PjcodeDto>> QueryPjcode(string code,string company)
        {
            List<PjcodeDto> pjcodeDtos = new List<PjcodeDto> ();
            List<PmcsPjCode> pmcsPjCodes=  await _PmcsPjcodeRepository.QueryPjcode(code,company);
            pmcsPjCodes.ForEach(b => pjcodeDtos.Add(new PjcodeDto {code =b.code, description=b.description}));
            return pjcodeDtos;
        }
        public async Task<string> CopyPjcode(string company)
        {
            List<PmcsPjCode> query = (await _PmcsPjcodeRepository.WithDetailsAsync()).ToList();
            foreach(var item in query)
            {
                item.company = company;
            }
            await _PmcsPjcodeRepository.InsertManyAsync(query);
            return "success";
        }
    }
}