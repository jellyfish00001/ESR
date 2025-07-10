using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.BDTreelevel;
using ERS.Entities;
using ERS.IDomainServices;
using ERS.IRepositories;
using Volo.Abp.ObjectMapping;

namespace ERS.DomainServices
{
    public class BDTreelevelDomainService : CommonDomainService, IBDTreelevelDomainService
    {
        private IBDTreelevelRepository _BDTreelevelRepository;
        private IObjectMapper _ObjectMapper;
        public BDTreelevelDomainService(
            IBDTreelevelRepository BDTreelevelRepository,
            IObjectMapper ObjectMapper
        )
        {
            _BDTreelevelRepository = BDTreelevelRepository;
            _ObjectMapper = ObjectMapper;
        }

        public async Task<Result<List<QueryBDTreelevelDto>>> GetBDTreelevelAll(){
            Result<List<QueryBDTreelevelDto>> result = new Result<List<QueryBDTreelevelDto>>()
            {
                data = new List<QueryBDTreelevelDto>()
            };
            
            List<BDTreelevel> treeLevel = new List<BDTreelevel>();
            treeLevel = (await _BDTreelevelRepository.WithDetailsAsync()).ToList();

            List<QueryBDTreelevelDto> resultdata = _ObjectMapper.Map<List<BDTreelevel>, List<QueryBDTreelevelDto>>(treeLevel);
            result.data = resultdata.ToList();
            result.total = resultdata.Count;
            return result;
        }

        public async Task<Result<QueryBDTreelevelDto>> GetBDTreelevelByLevelnum(decimal levelnum){
            Result<QueryBDTreelevelDto> result = new Result<QueryBDTreelevelDto>()
            {
                data = new QueryBDTreelevelDto()
            };
            BDTreelevel treeLevel = new BDTreelevel();
            treeLevel = await _BDTreelevelRepository.GetAsync(x => x.levelnum == levelnum);

            QueryBDTreelevelDto resultdata = _ObjectMapper.Map<BDTreelevel, QueryBDTreelevelDto>(treeLevel);
            result.data = resultdata;
            return result;
        }
    }
}