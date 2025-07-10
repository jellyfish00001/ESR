using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.IDomainServices;
using ERS.IRepositories;
using ERS.DTO.Proxy;
using ERS.Entities;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.ObjectMapping;
namespace ERS.DomainServices
{
    public class ProxyCashDomainService : CommonDomainService, IProxyCashDomainService
    {
        private IProxyCashRepository _ProxyCashRepository;
        private IEmployeeRepository _EmployeeRepository;
        private ICompanyRepository _CompanyRepository;
        private IObjectMapper _ObjectMapper;
        public ProxyCashDomainService(
            IProxyCashRepository ProxyCashRepository,
            IEmployeeRepository EmployeeRepository,
            ICompanyRepository CompanyRepository,
            IObjectMapper ObjectMapper
        )
        {
            _ProxyCashRepository = ProxyCashRepository;
            _EmployeeRepository = EmployeeRepository;
            _CompanyRepository = CompanyRepository;
            _ObjectMapper = ObjectMapper;
        }
        async Task<Result<string>> CheckEmpIsExist(List<string> emplids)
        {
            Result<string> result = new Result<string>();
            foreach (var emp in emplids)
            {
                if (!(await _EmployeeRepository.EmpIsExist(emp)))
                {
                    result.status = 2;
                    result.message = String.Format(L["ProxyCash-EmpNotExist"], emp);
                    return result;
                }
            }
            return result;
        }
        //添加代報銷員工代理
        public async Task<Result<string>> AddProxyCash(AddProxyCashDto request, string userId)
        {
            Result<string> result = new Result<string>();
            if (String.IsNullOrEmpty(request.aemplid) || String.IsNullOrEmpty(request.remplid))
            {
                result.status = 2;
                result.message = L["ProxyCash-EmptyEmplid"];
                return result;
            }
            if (request.aemplid == request.remplid)
            {
                result.status = 2;
                result.message = L["ProxyCash-SameEmplid"];
                return result;
            }
            List<string> emplids = new List<string>()
            {
                request.aemplid,
                request.remplid
            };
            var isExistResult = await CheckEmpIsExist(emplids);
            if (isExistResult.status == 2)
            {
                result.status = 2;
                result.message = isExistResult.message;
                return result;
            }
            if (await _ProxyCashRepository.ProxyIsExist(request.aemplid, request.remplid))
            {
                result.status = 2;
                result.message = L["ProxyCash-ProxyIsExist"];
                return result;
            }
            emplids.Add(userId);
            var empComCode = (await _EmployeeRepository.WithDetailsAsync()).Where(w => w.emplid == userId).Select(s => s.company).AsNoTracking().FirstOrDefault();
            var empCompany = (await _CompanyRepository.WithDetailsAsync()).Where(w => w.CompanyCategory == empComCode).Select(s => s.company).AsNoTracking().FirstOrDefault();
            ProxyCash proxyCash = new ProxyCash()
            {
                aemplid = request.aemplid,
                remplid = request.remplid,
                cdate = System.DateTime.Now,
                cuser = userId,
                company = empCompany
            };
            await _ProxyCashRepository.InsertAsync(proxyCash);
            result.message = L["AddSuccess"];
            return result;
        }
        //查詢代報銷員工代理
        public async Task<Result<List<ProxyCashDto>>> QueryProxyCash(Request<QueryProxyCashDto> request, string userId)
        {
            Result<List<ProxyCashDto>> result = new Result<List<ProxyCashDto>>();
            if (request.pageIndex < 1 || request.pageSize < 0)
            {
                request.pageIndex = 1;
                request.pageSize = 10;
            }
            var empComCode = (await _EmployeeRepository.WithDetailsAsync()).Where(w => w.emplid == userId).Select(s => s.company).AsNoTracking().FirstOrDefault();
            var empArea = (await _CompanyRepository.WithDetailsAsync()).Where(w => w.CompanyCategory == empComCode).Select(s => s.Area).AsNoTracking().FirstOrDefault();
            var empCompany = (await _CompanyRepository.WithDetailsAsync()).Where(w => w.Area == empArea).Select(s => s.company).AsNoTracking().ToList();
            var query = (await _ProxyCashRepository.WithDetailsAsync())
            .Where(w => empCompany.Contains(w.company))
            .WhereIf(!String.IsNullOrEmpty(request.data.aemplid), w => w.aemplid == request.data.aemplid)
            .WhereIf(!String.IsNullOrEmpty(request.data.remplid), w => w.remplid == request.data.remplid)
            .ToList();
            List<ProxyCashDto> data = _ObjectMapper.Map<List<ProxyCash>, List<ProxyCashDto>>(query);
            result.total = data.Count();
            data = data.Skip((request.pageIndex - 1) * request.pageSize).Take(request.pageSize).ToList();
            result.data = data;
            return result;
        }
        //刪除代報銷員工代理
        public async Task<Result<string>> DeleteProxyCash(List<Guid?> ids)
        {
            Result<string> result = new Result<string>();
            result.status = 2;
            var query = await _ProxyCashRepository.GetProxyCashListByIds(ids);
            if (query.Count > 0)
            {
                await _ProxyCashRepository.DeleteManyAsync(query);
                result.status = 1;
                result.message = L["DeleteSuccess"];
                return result;
            }
            result.message = L["DeleteFail"] + ": " + L["ProxyCash-DataNotExist"];
            result.total = query.Count;
            return result;
        }
        //編輯代報銷員工代理
        public async Task<Result<string>> EditProxyCash(EditProxyCashDto request, string userId)
        {
            Result<string> result = new Result<string>();
            List<string> emplids = new List<string>()
            {
                request.aemplid,
                request.remplid
            };
            ProxyCash proxyCash = await _ProxyCashRepository.GetProxyCashById(request.Id);
            //數據不存在
            if (proxyCash == null)
            {
                result.message = L["SaveFailMsg"] + "：" + L["ProxyCash-DataNotExist"];
                result.status = 2;
                return result;
            }
            //工號為空
            if (String.IsNullOrEmpty(request.aemplid) || String.IsNullOrEmpty(request.remplid))
            {
                result.status = 2;
                result.message = L["SaveFailMsg"] + "：" + L["ProxyCash-EmptyEmplid"];
                return result;
            }
            var isExistResult = await CheckEmpIsExist(emplids);
            if (isExistResult.status == 2)
            {
                result.status = 2;
                result.message = isExistResult.message;
                return result;
            }
            //相同工號
            if (request.aemplid == request.remplid)
            {
                result.status = 2;
                result.message = L["SaveFailMsg"] + "：" + L["ProxyCash-SameEmplid"];
                return result;
            }
            emplids.Add(userId);
            var empComCode = (await _EmployeeRepository.WithDetailsAsync()).Where(w => w.emplid == userId).Select(s => s.company).AsNoTracking().FirstOrDefault();
            var empCompany = (await _CompanyRepository.WithDetailsAsync()).Where(w => w.CompanyCategory == empComCode).Select(s => s.company).AsNoTracking().FirstOrDefault();
            proxyCash.aemplid = request.aemplid;
            proxyCash.remplid = request.remplid;
            proxyCash.company = empCompany;
            proxyCash.mdate = System.DateTime.Now;
            proxyCash.muser = userId;
            await _ProxyCashRepository.UpdateAsync(proxyCash);
            result.message = L["SaveSuccessMsg"];
            return result;
        }
    }
}