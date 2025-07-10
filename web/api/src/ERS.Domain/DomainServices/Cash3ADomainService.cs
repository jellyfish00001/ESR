using ERS.Application.Contracts.DTO.Application;
using ERS.DTO;
using ERS.DTO.Application;
using ERS.Entities;
using ERS.IDomainServices;
using ERS.IRepositories;
using ERS.Minio;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.ObjectMapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
namespace ERS.DomainServices
{
    public class Cash3ADomainService : CommonDomainService, ICash3ADomainService
    {
        private IBDCashReturnRepository _bdcashreturnRepository;
        private IObjectMapper _objectMapper;
        private ICashDetailRepository _CashDetailRepository;
        private ICashHeadRepository _CashHeadRepository;
        private ICashAmountRepository _CashAmountRepository;
        private IBDashReturnRepository _BDashReturnRepository;
        private ICashAccountRepository _CashAccountRepository;
        private IEFormHeadRepository _EFormHeadRepository;
        private IApprovalDomainService _ApprovalDomainService;
        private IEFormAlistRepository _EFormAlistRepository;
        private IEFormAuserRepository _EFormAuserRepository;
        private ICashFileRepository _CashFileRepository;
        private IMinioRepository _MinioRepository;
        private IAutoNoRepository _AutoNoRepository;
        private IEmployeeRepository _EmployeeRepository;
        private IEmpOrgRepository _EmpOrgRepository;
        private IConfiguration _configuration;
        private IProxyCashRepository _ProxyCashRepository;
        private IMinioDomainService _minioDomainService;
        public Cash3ADomainService(
            IBDCashReturnRepository bDCashReturnRepository,
            IObjectMapper ObjectMapper,
            ICashDetailRepository CashDetailRepository,
            ICashHeadRepository CashHeadRepository,
            ICashAmountRepository CashAmountRepository,
            IBDashReturnRepository BDashReturnRepository,
            ICashAccountRepository CashAccountRepository,
             IAutoNoRepository AutoNoRepository,
             IEFormHeadRepository EFormHeadRepository,
             IApprovalDomainService ApprovalDomainService, IEFormAlistRepository EFormAlistRepository, IEFormAuserRepository EFormAuserRepository,
            ICashFileRepository CashFileRepository,
            IEmployeeRepository EmployeeRepository,
            IMinioRepository MinioRepository,
            IProxyCashRepository proxyCashRepository,
            IMinioDomainService minioDomainService,
            IEmpOrgRepository EmpOrgRepository, IConfiguration configuration
            )
        {
            _bdcashreturnRepository = bDCashReturnRepository;
            _objectMapper = ObjectMapper;
            _CashDetailRepository = CashDetailRepository;
            _CashHeadRepository = CashHeadRepository;
            _CashAmountRepository = CashAmountRepository;
            _BDashReturnRepository = BDashReturnRepository;
            _CashAccountRepository = CashAccountRepository;
            _AutoNoRepository = AutoNoRepository;
            _EFormHeadRepository = EFormHeadRepository;
            _ApprovalDomainService = ApprovalDomainService;
            _EFormAlistRepository = EFormAlistRepository;
            _EFormAuserRepository = EFormAuserRepository;
            _CashFileRepository = CashFileRepository;
            _MinioRepository = MinioRepository;
            _EmployeeRepository = EmployeeRepository;
            _EmpOrgRepository = EmpOrgRepository;
            _configuration = configuration;
            _ProxyCashRepository = proxyCashRepository;
            _minioDomainService = minioDomainService;
        }
        //向上递归找出部级部门代码
        public async Task<EmpOrg> GetMinisterialDeptid(string deptid)
        {
            EmpOrg empOrg = (await _EmpOrgRepository.WithDetailsAsync()).Where(b => b.deptid == deptid).AsNoTracking().FirstOrDefault();
            EmpOrg emp = new EmpOrg();
            emp = empOrg;
            if (empOrg.tree_level_num > 7)
            {
                emp = await GetMinisterialDeptid(empOrg.uporg_code_a);
            }
            return emp;
        }
        // //向下递归找出下级部门
        // public async Task<EmpOrg> GetLowerDeptid(string deptid, string company, IList<EmpOrg> empOrgs)
        // {
        //     EmpOrg empOrg = empOrgs.Where(b => b.uporg_code_a == deptid && b.company == company).FirstOrDefault();
        //     EmpOrg emp = new EmpOrg();
        //     emp = empOrg;
        //     if (empOrg != null)
        //     {
        //         emp = await GetLowerDeptid(empOrg.deptid, company, empOrgs);
        //     }
        //     return emp;
        // }
        //获取所有下级部门
        public async Task<List<string>> GetAllLowerDepts(string deptid, List<string> list)
        {
            List<string> empOrg = (await _EmpOrgRepository.WithDetailsAsync()).Where(b => b.uporg_code_a == deptid).Select(i => i.deptid).AsNoTracking().Distinct().ToList();
            list.Add(deptid);
            foreach (string item in empOrg)
            {
                list = await GetAllLowerDepts(item, list);
            }
            return list;
        }
        public async Task<Result<List<DeferredDto>>> DeferredQuery(string user)
        {
            Result<List<DeferredDto>> result = new();
            List<DeferredDto> deferredlist = new List<DeferredDto>();//所有预支金明细
            //申请人信息
            Employee employee = (await _EmployeeRepository.WithDetailsAsync()).Where(w => w.emplid == user).AsNoTracking().FirstOrDefault();
            // EmpOrg deptInfo = (await _EmpOrgRepository.WithDetailsAsync()).Where(w => w.deptid == employee.deptid && w.company == employee.company).FirstOrDefault();
            EmpOrg managerDeptInfo = (await _EmpOrgRepository.WithDetailsAsync()).Where(w => w.manager_id == user).AsNoTracking().FirstOrDefault();
            string dept = "";
            if (managerDeptInfo != null && managerDeptInfo.tree_level_num >= 7)
            {
                dept = managerDeptInfo.deptid;
            }
            else if (managerDeptInfo == null)
            {
                dept = employee.deptid;
            }
            EmpOrg deptInfo = (await _EmpOrgRepository.WithDetailsAsync()).Where(w => w.deptid == dept).AsNoTracking().FirstOrDefault();
            List<string> userList = new List<string>();
            //申请人所在部门为部级以上只查询自己的预支金明细
            if ((managerDeptInfo != null && managerDeptInfo.tree_level_num < 7) || deptInfo.tree_level_num < 7)
            {
                //添加自己
                userList.Add(user);
                //IList<CashDetail> cashDetails = await _CashDetailRepository.UserDetail(user, "CASH_3");
                //IList<CashHead> cashHeads = await _CashHeadRepository.GetUserHead(user);
                //List<BDCashReturn> cashReturns = (await _bdcashreturnRepository.WithDetailsAsync()).Where(w => cashHeads.Select(s => s.rno).ToList().Contains(w.rno)).AsNoTracking().ToList();
                //IList<CashAmount> cashAmounts = await _CashAmountRepository.GetuserAll(user);
                //List<CashFile> cashFiles = (await _CashFileRepository.ReadOnlyByCuser(user)).Where(i => i.ishead == "N").ToList();
                //foreach (CashDetail item in cashDetails)
                //{
                //    CashHead head = cashHeads.Where(b => b.rno == item.rno).FirstOrDefault();
                //    List<BDCashReturn> bDCash = cashReturns.Where(b => b.rno == item.rno).ToList();
                //    CashAmount amount = cashAmounts.Where(b => b.rno == item.rno).FirstOrDefault();
                //    if (head != null && amount != null)
                //    {
                //        if (bDCash.Count > 0)
                //        {
                //            amount.actamt -= bDCash.Sum(i => i.amount);
                //        }
                //        if (amount.actamt != 0)
                //        {
                //            //openday： 今天减去申请日期 dt1 - dt2
                //            //delay：今天减去冲销日期 dt1 - dt3
                //            DateTime dt1 = System.DateTime.Now;
                //            DateTime dt2 = Convert.ToDateTime(item.cdate);
                //            DateTime dt3 = Convert.ToDateTime(item.revsdate);
                //            DateTime Convert_dt1 = Convert.ToDateTime(string.Format("{0}-{1}-{2}", dt1.Year, dt1.Month, dt1.Day));
                //            DateTime Convert_dt2 = Convert.ToDateTime(string.Format("{0}-{1}-{2}", dt2.Year, dt2.Month, dt2.Day));
                //            DateTime Convert_dt3 = Convert.ToDateTime(string.Format("{0}-{1}-{2}", dt3.Year, dt3.Month, dt3.Day));
                //            DeferredDto dto = new DeferredDto();
                //            dto.expcode = item.expcode;
                //            dto.expname = item.expname;
                //            dto.company = head.company;
                //            dto.cuser = head.cuser;
                //            dto.cname = head.cname;
                //            dto.payeeid = head.payeeId;
                //            dto.payeename = head.payeename;
                //            dto.rno = item.rno;
                //            dto.remark = item.summary;//摘要
                //            dto.amount = Convert.ToDecimal(item.baseamt);
                //            dto.actamt = amount.actamt;
                //            dto.opendays = (Convert.ToDateTime(System.DateTime.Now) - Convert.ToDateTime(item.cdate)).Days;
                //            dto.delaydays = (Convert_dt1 - Convert_dt3).Days < 0 ? 0 : (Convert_dt1 - Convert_dt3).Days;
                //            dto.delay = head.overduesum;
                //            dto.revsdate = Convert.ToDateTime(item.revsdate);
                //            dto.cdate = Convert.ToDateTime(item.cdate);
                //            dto.file = _objectMapper.Map<List<CashFile>, List<CashFileDto>>(cashFiles.Where(i => i.rno == item.rno).ToList());
                //            foreach (var file in dto.file)
                //                file.url = !String.IsNullOrEmpty(file.path) ? await _MinioRepository.PresignedGetObjectAsync(file.path) : String.Empty;
                //            deferredlist.Add(dto);
                //        }
                //    }
                //}
            }
            //所在部门为部级以下，找出包括部级在内的所有人的预支金明细
            else
            {
                List<string> allDeptList = new List<string>();
                List<string> tempList = new List<string>();
                EmpOrg ministerialEmpOrg = await GetMinisterialDeptid(dept);//部级部门
                allDeptList = (await GetAllLowerDepts(ministerialEmpOrg.deptid, tempList)).Distinct().ToList();
                userList = (await _EmployeeRepository.WithDetailsAsync()).Where(w => allDeptList.Contains(w.deptid)).Select(s => s.emplid).ToList();
                userList.Add(user);
            }
            //(20240809修改为同时带出代理人)
            List<string> proxyCash = await _ProxyCashRepository.ReadProxyByAmplid(user);
            if (proxyCash.Count > 0)
            {
                userList.AddRange(proxyCash);
            }
            userList = userList.Distinct().ToList();
            List<CashDetail> allCashDetails = (await _CashDetailRepository.WithDetailsAsync()).Where(w => userList.Contains(w.cuser) && w.formcode == "CASH_3").AsNoTracking().ToList();
            List<CashHead> allCashHeads = (await _CashHeadRepository.WithDetailsAsync()).Where(w => allCashDetails.Select(s => s.rno).ToList().Contains(w.rno)).AsNoTracking().ToList();
            List<BDCashReturn> allCashReturns = (await _bdcashreturnRepository.WithDetailsAsync()).Where(w => allCashDetails.Select(s => s.rno).ToList().Contains(w.rno)).AsNoTracking().ToList();
            List<CashFile> allCashFiles = (await _CashFileRepository.WithDetailsAsync()).Where(w => allCashDetails.Select(s => s.rno).ToList().Contains(w.rno) && w.ishead == "N").AsNoTracking().ToList();
            List<CashAmount> allCashAmounts = (await _CashAmountRepository.WithDetailsAsync()).Where(w => allCashDetails.Select(s => s.rno).ToList().Contains(w.rno)).AsNoTracking().ToList();
            foreach (string userItem in userList.Distinct().ToList())
            {
                foreach (CashDetail detail in allCashDetails.Where(w => w.cuser == userItem))
                {
                    CashHead head = allCashHeads.Where(w => w.rno == detail.rno).FirstOrDefault();
                    var bDCash = allCashReturns.Where(w => w.rno == detail.rno).ToList();
                    CashAmount amount = allCashAmounts.Where(b => b.rno == detail.rno).FirstOrDefault();
                    List<CashFile> cashFiles = allCashFiles.Where(w => w.rno == detail.rno).ToList();
                    string area = await _minioDomainService.GetMinioArea(head.cuser);
                    if (head != null && amount != null)
                    {
                        if (bDCash.Count > 0)
                        {
                            amount.actamt -= bDCash.Sum(i => i.amount);
                        }
                        if (amount.actamt != 0)
                        {
                            //openday： 今天减去申请日期 dt1 - dt2
                            //delay：今天减去冲销日期 dt1 - dt3
                            DateTime dt1 = System.DateTime.Now;
                            DateTime dt2 = Convert.ToDateTime(detail.cdate);
                            DateTime dt3 = Convert.ToDateTime(detail.revsdate);
                            DateTime Convert_dt1 = Convert.ToDateTime(string.Format("{0}-{1}-{2}", dt1.Year, dt1.Month, dt1.Day));
                            DateTime Convert_dt2 = Convert.ToDateTime(string.Format("{0}-{1}-{2}", dt2.Year, dt2.Month, dt2.Day));
                            DateTime Convert_dt3 = Convert.ToDateTime(string.Format("{0}-{1}-{2}", dt3.Year, dt3.Month, dt3.Day));
                            DeferredDto dto = new DeferredDto();
                            dto.expcode = detail.expcode;
                            dto.expname = detail.expname;
                            dto.company = head.company;
                            dto.cuser = head.cuser;
                            dto.cname = head.cname;
                            dto.payeeid = head.payeeId;
                            dto.payeename = head.payeename;
                            dto.rno = detail.rno;
                            dto.remark = detail.summary;//摘要
                            dto.amount = Convert.ToDecimal(detail.baseamt);
                            dto.actamt = amount.actamt;
                            dto.opendays = (Convert_dt1 - Convert_dt2).Days;
                            dto.delaydays = (Convert_dt1 - Convert_dt3).Days < 0 ? 0 : (Convert_dt1 - Convert_dt3).Days;
                            dto.delay = head.overduesum;
                            dto.revsdate = Convert.ToDateTime(detail.revsdate);
                            dto.cdate = Convert.ToDateTime(detail.cdate);
                            dto.file = _objectMapper.Map<List<CashFile>, List<CashFileDto>>(cashFiles.Where(i => i.rno == detail.rno).ToList());
                            foreach (var file in dto.file)
                                file.url = !String.IsNullOrEmpty(file.path) ? await _MinioRepository.PresignedGetObjectAsync(file.path,area) : String.Empty;
                            deferredlist.Add(dto);
                        }
                    }
                }
            }
            deferredlist = deferredlist.Where(w => w.expcode != "EXP02").GroupBy(s => s.rno).Select(g => g.FirstOrDefault()).ToList();//过滤掉交际费相关单据
            //筛选出已经签完的
            var eFormQuery = (await _EFormHeadRepository.WithDetailsAsync()).Where(w => deferredlist.Select(s => s.rno).Contains(w.rno) && w.status == "A").ToList();
            deferredlist = deferredlist.Where(w => eFormQuery.Select(s => s.rno).Contains(w.rno)).ToList();
            result.data = deferredlist;
            result.total = deferredlist.Count;
            return result;
        }
        public async Task<Result<CashResult>> Submit(IFormCollection formCollection, string user, string token, string status = "P")
        {
            Result<CashResult> result = new();
            CashResult cash = new();
            result.status = 2;
            CashHead list = TransformAndSetData(formCollection, user);
            if (status == "T")
                list.SetKeepStatus("RQ401A");
            if (!list.CheckIsExistRno())
            {
                string rno = await _AutoNoRepository.CreateCash3ANo();
                list.SetRno(rno);
                await _EFormHeadRepository.InsertAsync(list.EFormHead);
            }
            else
            {
                await DeleteData(list);
                list.SetRno(list.rno);
                EFormHead temp = await _EFormHeadRepository.GetByNo(list.rno);
                temp.ChangeStatus(list.EFormHead.status);
                temp.SetApid(list.EFormHead.apid);
                list.UpdateEFormHead(temp);
            }
            // 改变延期次数
            result = await ChangeDelayNum(list, status);
            if (result.status == 2) return result;
            // file
            bool saveFlag = await InsertData(list);
            if (saveFlag)
            {
                if (status.Equals("P"))
                {
                    await _ApprovalDomainService.CreateSignSummary(list, false, token);
                }
            }
            cash.rno = list.rno;
            cash.Stat = false;
            result.data = cash;
            result.message = (cash.Stat ? L["AddPaperTipMessage"] : L["UnAddPaperTipMessage"]);
            result.status = 1;
            return result;
        }
        async Task<Result<CashResult>> ChangeDelayNum(CashHead list, string status)
        {
            Result<CashResult> result = new();
            result.status = 2;
            List<string> rnolist = list.CashDetailList.Select(i => i.advancerno).ToList();
            var headlist = await _CashHeadRepository.GetCashHead(rnolist);
            foreach (var item in headlist)
            {
                if (item.overduesum > 0)
                {
                    result.message = String.Format(L["rnoIsDelayed"], item.rno);
                    return result;
                }
            }
            if (status == "P")
            {
                foreach (var item in headlist)
                    item.overduesum = 1;
                await _CashHeadRepository.UpdateManyAsync(headlist);
            }
            result.status = 1;
            return result;
        }
        CashHead TransformAndSetData(IFormCollection formCollection, string user)
        {
            string head = formCollection["head"];
            string detail = formCollection["detail"];
            CashHeadDto hData = JsonConvert.DeserializeObject<CashHeadDto>(head);
            IList<CashDetailDto> dData = JsonConvert.DeserializeObject<IList<CashDetailDto>>(detail);
            CashHead list = _objectMapper.Map<CashHeadDto, CashHead>(hData);
            IList<CashDetail> detailData = _objectMapper.Map<IList<CashDetailDto>, IList<CashDetail>>(dData);
            list.SetCash3A(list, user);
            list.AddCash3ADetail(detailData);
            return list;
        }
        async Task DeleteData(CashHead list)
        {
            List<EFormAlist> eFormAlists = await _EFormAlistRepository.GetListAsync(i => i.rno == list.rno);
            if (eFormAlists.Count > 0) await _EFormAlistRepository.DeleteManyAsync(eFormAlists);
            List<EFormAuser> eFormAuser = await _EFormAuserRepository.GetListAsync(i => i.rno == list.rno);
            if (eFormAuser.Count > 0) await _EFormAuserRepository.DeleteManyAsync(eFormAuser);
            await _CashHeadRepository.DeleteAsync(await _CashHeadRepository.GetByNo(list.rno));
            await _CashDetailRepository.DeleteManyAsync(await _CashDetailRepository.GetByNo(list.rno));
        }
        async Task<bool> InsertData(CashHead list)
        {
            try {
                await _CashHeadRepository.InsertAsync(list);
                await _CashDetailRepository.InsertManyAsync(list.CashDetailList);
                return true;
            }
            catch (Exception ex) {
                return false;
            }
        }
    }
}
