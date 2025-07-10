using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERS.Application.Contracts.DTO.Emporg;
using ERS.DTO;
using ERS.Entities;
using ERS.IDomainServices;
using ERS.IRepositories;
using NUglify.Helpers;
using Volo.Abp.Application.Services;
using Volo.Abp.ObjectMapping;
namespace ERS.Application.Services
{
    public class EmporgService : ApplicationService, IEmporgService
    {
        private ICompanyDomainService _companyDomainService;
        private IEmpOrgRepository _EmpOrgRepository;
        private IObjectMapper _ObjectMapper;
        public EmporgService(IEmpOrgRepository EmpOrgRepository, ICompanyDomainService companyDomainService, IObjectMapper ObjectMapper)
        {
            _companyDomainService = companyDomainService;
            _EmpOrgRepository = EmpOrgRepository;
            _ObjectMapper = ObjectMapper;
        }
        //递归获取TREE_LEVEL_NUM小于或等于7的部门
        public async Task<string> GetDeptid(string deptid, string company) => await _EmpOrgRepository.GetCostDeptid(deptid, company);
        /// <summary>
        /// 返回前100个部门
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        public async Task<IList<EmporgDto>> GetPartDeptidList()
        {
            IList<EmpOrg> data = await _EmpOrgRepository.GetDeptidList(100);
            IList<EmporgDto> list = _ObjectMapper.Map<IList<EmpOrg>, IList<EmporgDto>>(data);
            return list;
        }
        public async Task<List<EmporgDto>> QueryVagues(string deptid)
        {
            List<EmporgDto> emporgDtos = new List<EmporgDto>();
            //Company companySite = await _companyDomainService.GetCompanybySite(company);
            // List<Company> companySites = await _companyDomainService.GetConpanyCodeBySite(company);
            // foreach(var item in  companySites)
            // {
            IEnumerable<EmpOrg> empOrgs = await _EmpOrgRepository.QueryVagues(deptid);
            empOrgs.ForEach(b => emporgDtos.Add(new EmporgDto { deptid = b.deptid, descr = b.descr }));
            return emporgDtos.GroupBy(w => w.deptid).Select(g => g.FirstOrDefault()).ToList();
        }

        //递归获取TREE_LEVEL_NUM小于或等于7的部门for API
        public async Task<Result<string>> GetCodtDeptid(string deptid, string company)
        {
            Result<string> result = new Result<string>();
            result.data = await _EmpOrgRepository.GetCostDeptid(deptid, company);
            result.status = 1;
            return result;

        }
    }
    }