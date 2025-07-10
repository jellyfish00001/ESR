using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.Auditor;
using ERS.IDomainServices;
using ERS.IRepositories;
using Volo.Abp.ObjectMapping;
using ERS.Entities;
namespace ERS.DomainServices
{
    /// <summary>
    /// 簽核主管Auditor維護（BD04）
    /// </summary>
    public class AuditorDomainService : CommonDomainService, IAuditorDomainService
    {
        private IEFormAuditRepository _EFormAuditRepository;
        private IBDFormRepository _bdformRepository;
        private IEmployeeRepository _EmployeeRepository;
        private IObjectMapper _ObjectMapper;
        public AuditorDomainService(
            IEFormAuditRepository EFormAuditRepository,
            IBDFormRepository BDformRepository,
            IEmployeeRepository EmployeeRepository,
            IObjectMapper ObjectMapper
        )
        {
            _EFormAuditRepository = EFormAuditRepository;
            _bdformRepository =BDformRepository;
            _EmployeeRepository = EmployeeRepository;
            _ObjectMapper = ObjectMapper;
        }
        public async Task<Result<List<AuditorDto>>> GetPageAuditors(Request<AuditorParamsDto> request)
        {
            Result<List<AuditorDto>> result = new Result<List<AuditorDto>>();
            var formQuery = (await _bdformRepository.WithDetailsAsync()).ToList();
            var auditQuery = (await _EFormAuditRepository.WithDetailsAsync())
                            .WhereIf(!string.IsNullOrEmpty(request.data.formcode), w => w.formcode == request.data.formcode)
                            .WhereIf(!string.IsNullOrEmpty(request.data.emplid), w => w.emplid == request.data.emplid)
                            .Where( w => request.data.companyList.Contains(w.company))
                            .Select(w => new
                            {
                                Id = w.Id,
                                formcode = w.formcode,
                                emplid = w.emplid,
                                auditid = w.auditid,
                                sdate = w.sdate,
                                edate = w.edate,
                                muser = w.muser,
                                mdate = w.mdate,
                                company = w.company
                            }).ToList();
            var emplNameQuery = (await _EmployeeRepository.WithDetailsAsync()).Where(w => auditQuery.Select(w => w.emplid).Contains(w.emplid)).Select(w => new { w.emplid, w.cname }).ToList();
            var auditNameQuery = (await _EmployeeRepository.WithDetailsAsync()).Where(w => auditQuery.Select(w => w.auditid).Contains(w.emplid)).Select(w => new { w.emplid, w.cname }).ToList();
            var resultQuery = (from a in auditQuery
                               join b in emplNameQuery
                               on a.emplid equals b.emplid
                               join c in auditNameQuery
                               on a.auditid equals c.emplid
                               select new AuditorDto
                               {
                                   Id = a.Id,
                                   formcode = a.formcode,
                                   emplid = a.emplid,
                                   cname = b.cname,
                                   auditid = a.auditid,
                                   auditname = c.cname,
                                   sdate = a.sdate,
                                   edate = a.edate,
                                   muser = a.muser,
                                   mdate = a.mdate,
                                   formname = a.formcode == "ALL" ? "ALL" : formQuery.Where(w => w.FormCode == a.formcode).Select(w => w.FormName).FirstOrDefault(),
                                   company = a.company
                               }).DistinctBy(W => W.Id).ToList();
            int pageIndex = request.pageIndex;
            int pageSize = request.pageSize;
            if (pageIndex < 1 || pageSize < 0)
            {
                pageIndex = 1;
                pageSize = 10;
            }
            int count = resultQuery.Count;
            result.data = resultQuery.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            result.total = count;
            return result;
        }
        public async Task<Result<string>> AddAuditor(AuditorParamsDto request, string userId)
        {
            Result<string> result = new Result<string>();
            Result<bool> check = await InputCheck(request);
            if (!check.data)
            {
                result.status = check.status;
                result.message = check.message;
                return result;
            }
            EFormAudit eFormAudit = _ObjectMapper.Map<AuditorParamsDto, EFormAudit>(request);
            //判断是否重复
            var repeatQuery = (await _EFormAuditRepository.WithDetailsAsync())
            .Where(w => w.company == eFormAudit.company && w.emplid == eFormAudit.emplid && w.auditid == eFormAudit.auditid && w.formcode == eFormAudit.formcode).ToList();
            if(repeatQuery.Count > 0)
            {
                result.status = 2;
                result.message = L["DataAlreadyexist"];
                return result;
            }
            eFormAudit.cuser = userId;
            eFormAudit.cdate = System.DateTime.Now;
            eFormAudit.muser = userId;
            eFormAudit.mdate = System.DateTime.Now;
            eFormAudit.company = request.company;
            await _EFormAuditRepository.InsertAsync(eFormAudit);
            result.message = L["SaveSuccessMsg"];
            return result;
        }
        public async Task<Result<string>> EditAuditor(AuditorParamsDto request, string userId)
        {
            Result<string> result = new Result<string>();
            Result<bool> check = await InputCheck(request);
            if (!check.data)
            {
                result.status = check.status;
                result.message = check.message;
                return result;
            }
            EFormAudit eFormAudit = await _EFormAuditRepository.GetAuditById(request.Id);
            if (eFormAudit == null)
            {
                result.status = 2;
                result.message = L["AuditNotFound"];
                return result;
            }
            //eFormAudit = _ObjectMapper.Map<AuditorParamsDto,EFormAudit>(request);
            eFormAudit.formcode = request.formcode;
            eFormAudit.emplid = request.emplid;
            eFormAudit.deptid = request.deptid;
            eFormAudit.auditid = request.auditid;
            eFormAudit.sdate = request.sdate;
            eFormAudit.edate = request.edate;
            eFormAudit.muser = userId;
            eFormAudit.mdate = System.DateTime.Now;
            await _EFormAuditRepository.UpdateAsync(eFormAudit);
            result.message = L["SaveSuccessMsg"];
            return result;
        }
        public async Task<Result<string>> DeleteAuditors(List<Guid?> Ids)
        {
            Result<string> result = new Result<string>();
            List<EFormAudit> eFormAudits = (await _EFormAuditRepository.GetAuditsByIds(Ids));
            if (eFormAudits.Count <= 0)
            {
                result.message = L["DeleteFail"] + "：" + L["AuditorNotFound"];
                result.status = 2;
                return result;
            }
            await _EFormAuditRepository.DeleteManyAsync(eFormAudits);
            result.message = L["DeleteSuccess"];
            return result;
        }
        public async Task<Result<bool>> InputCheck(AuditorParamsDto request)
        {
            Result<bool> result = new Result<bool>();
            result.data = true;
            if (string.IsNullOrEmpty(request.emplid))
            {
                result.status = 2;
                result.message = L["EmployeeIDNotEmpty"];
                result.data = false;
                return result;
            }
            if (string.IsNullOrEmpty(request.deptid))
            {
                result.status = 2;
                result.message = L["DepartmentNotEmpty"];
                result.data = false;
                return result;
            }
            if (string.IsNullOrEmpty(request.auditid))
            {
                result.status = 2;
                result.message = L["AuditNotEmpty"];
                result.data = false;
                return result;
            }
            if (string.IsNullOrEmpty(request.sdate.ToString()))
            {
                result.status = 2;
                result.message = L["StartDateNotEmpty"];
                result.data = false;
                return result;
            }
            if (string.IsNullOrEmpty(request.edate.ToString()))
            {
                result.status = 2;
                result.message = L["EndDateNotEmpty"];
                result.data = false;
                return result;
            }
            TimeSpan ts = request.edate - request.sdate;
            if (ts.TotalDays < 0)
            {
                result.status = 2;
                result.data = false;
                result.message = L["EndDateErrorMsg"];
                return result;
            }
            if (!await _EmployeeRepository.EmpIsExist(request.auditid))
            {
                result.status = 2;
                result.data = false;
                result.message = L["AuditidNotExist"] + request.auditid;
                return result;
            }
            if (!await _EmployeeRepository.EmpIsExist(request.emplid))
            {
                result.status = 2;
                result.data = false;
                result.message = L["EmplidNotExist"] + request.emplid;
                return result;
            }
            return result;
        }
        public async Task<Result<List<BDFormDto>>> GetEFormCodeName()
        {
            Result<List<BDFormDto>> result = new();
            var query = (await _bdformRepository.WithDetailsAsync()).ToList();
            var queryResult = _ObjectMapper.Map<List<BDForm>, List<BDFormDto>>(query);
            result.data = queryResult;
            return result;
        }
    }
}