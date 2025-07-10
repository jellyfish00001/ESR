using System.Collections.Generic;
using ERS.Application.Contracts.DTO.PapreSign;
using ERS.IDomainServices;
using ERS.IRepositories;
using ERS.Entities;
using System.Linq;
using Volo.Abp.ObjectMapping;
using ERS.DTO.Application;
using Volo.Abp.Domain.Repositories;
using System;
using System.Threading.Tasks;
using ERS.Minio;
using ERS.DTO.PapreSign;
using ERS.DTO;
using Microsoft.EntityFrameworkCore;
namespace ERS.DomainServices
{
    public class ApprovalPaperDomainService : CommonDomainService, IApprovalPaperDomainService
    {
        private IApprovalPaperRepository _EFormPaperRepository;
        private IEFormHeadRepository _eFormHeadRepository;
        private IBDFormRepository _BDformRepository;
        private ICashFileRepository _cashfileRepository;
        private IMinioRepository _MinioRepository;
        private IObjectMapper _objectMapper;
        private IEmployeeDomainService _EmployeeDomainService;
        private IRepository<BDPaperSign, Guid> _BDPaperSignRepository;
        private IEFormAlistRepository _EFormAlistRepository;
        private IEFormAuserRepository _EFormAuserRepository;
        private ICashHeadRepository _CashHeadRepository;
        private IAccountDomainService _AccountDomainService;
        private IBPMDomainService _BPMDomainService;
        private IMinioDomainService _minioDomainService;
        public ApprovalPaperDomainService(
            IApprovalPaperRepository EFormPaperRepository,
            IEFormHeadRepository eFormHeadRepository,
            ICashFileRepository cashfileRepository,
            IBDFormRepository BDformRepository,
            IMinioRepository MinioRepository,
            IEmployeeDomainService EmployeeDomainService,
            IRepository<BDPaperSign, Guid> BDPaperSignRepository,
            IEFormAlistRepository EFormAlistRepository,
            IEFormAuserRepository EFormAuserRepository,
            ICashHeadRepository CashHeadRepository,
            IAccountDomainService AccountDomainService,
            IBPMDomainService BPMDomainService,
            IObjectMapper ObjectMapper,
            IMinioDomainService minioDomainService)
        {
            _EFormPaperRepository = EFormPaperRepository;
            _eFormHeadRepository = eFormHeadRepository;
            _BDformRepository = BDformRepository;
            _cashfileRepository = cashfileRepository;
            _MinioRepository = MinioRepository;
            _EmployeeDomainService = EmployeeDomainService;
            _BDPaperSignRepository = BDPaperSignRepository;
            _EFormAlistRepository = EFormAlistRepository;
            _EFormAuserRepository = EFormAuserRepository;
            _CashHeadRepository = CashHeadRepository;
            _AccountDomainService = AccountDomainService;
            _BPMDomainService = BPMDomainService;
            _objectMapper = ObjectMapper;
            _minioDomainService = minioDomainService;
        }
        // 按签核人工号分页查询待签核的纸本单（含发票）
        public async Task<Result<List<PaperDto>>> QueryUnsignPaperByemplid(Request<PaperQueryDto> request)
        {
            //  取出待签核paper
            List<ApprovalPaper> paperquery = (await _EFormPaperRepository.GetUnsignByEmplid(request.data.emplid));
            List<EFormHead> headQuery = (await _eFormHeadRepository.WithDetailsAsync()).Where(w => paperquery.Select(s => s.rno).ToList().Contains(w.rno)).ToList();
            var eformalistQuery = (await _EFormAlistRepository.WithDetailsAsync()).Where(w => headQuery.Select(w => w.rno).ToList().Contains(w.rno)).AsNoTracking().ToList();
            var eformauserQuery = (await _EFormAuserRepository.WithDetailsAsync()).Where(w => headQuery.Select(w => w.rno).ToList().Contains(w.rno)).AsNoTracking().ToList();
            var cashheadQuery = (await _CashHeadRepository.WithDetailsAsync()).Where(w => headQuery.Select(w => w.rno).ToList().Contains(w.rno)).AsNoTracking().ToList();
            //  取出申请单号、表单类型、申请人、申请人中文名、申请时间、步骤
            var resultQuery1 = (from pq in paperquery
                                join efh in headQuery
                                on pq.rno equals efh.rno
                                join ch in cashheadQuery
                                on pq.rno equals ch.rno
                                select new
                                {
                                    rno = pq.rno,
                                    formcode = efh.formcode,
                                    cuser = efh.cuser,
                                    cname = efh.cname,
                                    cdate = efh.cdate,
                                    step = efh.step,
                                    apid = efh.apid,
                                    stepname = (efh.status == "B" ? "Return" : (efh.status == "R" ? "Rejected" : (efh.status == "E" ? "EDIT" : (efh.status == "A" ? "Approved" : (efh.status == "T" ? "Temporary" : (efh.status == "C" ? "Cancel" : (efh.status == "P" ? L["PaperReport-Stepname"] : ""))))))),
                                    payment = ch.payment
                                }).ToList();
            // 取出表单类型名
            var resultQuery2 = (from rq1 in resultQuery1
                                join f in await _BDformRepository.WithDetailsAsync()
                                on rq1.formcode equals f.FormCode
                                select new
                                {
                                    rno = rq1.rno,
                                    formcode = rq1.formcode,
                                    cuser = rq1.cuser,
                                    cname = rq1.cname,
                                    cdate = rq1.cdate,
                                    step = rq1.step,
                                    apid = rq1.apid,
                                    payment = rq1.payment,
                                    stepname = rq1.stepname,
                                    formname = f.FormName
                                }).ToList();
            List<CashFile> filequery = new List<CashFile>();
            List<CashFileDto> filedata = new List<CashFileDto>();
            List<CashFileDto> invlist = new List<CashFileDto>();
            foreach (var item in resultQuery2)
            {
                // 取发票清单
                filequery = (await _cashfileRepository.GetByNo(item.rno)).ToList();
                filedata = _objectMapper.Map<List<CashFile>, List<CashFileDto>>(filequery);
                string area = await _minioDomainService.GetMinioArea(item.cuser);
                foreach (var file in filedata)
                {
                    file.url = !String.IsNullOrEmpty(file.path) ? await _MinioRepository.PresignedGetObjectAsync(file.path,area) : String.Empty;
                }
                invlist.AddRange(filedata);
            }
            var result = (from rq2 in resultQuery2
                          select new PaperDto
                          {
                              rno = rq2.rno,
                              formcode = rq2.formcode,
                              formname = rq2.formname,
                              cemplid = rq2.cuser,
                              cname = rq2.cname,
                              cdate = rq2.cdate,
                              step = rq2.step,
                              stepname = rq2.stepname,
                              apid = rq2.apid,
                              payment = rq2.payment,
                              invlist = invlist.Where(x => x.rno == rq2.rno).OrderBy(i => i.item).ToList()
                          }).ToList();
            int pageIndex = request.pageIndex;
            int pageSize = request.pageSize;
            if (pageIndex < 1 || pageSize < 0)
            {
                pageIndex = 1;
                pageSize = 10;
            }
            int count = result.Count();
            result = result.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            Result<List<PaperDto>> finalresult = new Result<List<PaperDto>>()
            {
                data = result,
                total = count
            };
            return finalresult;
        }
        // 签核api（纸本单据中签核）
        public async Task<Result<string>> SignPaper(List<string> rno, string emplid, string token, bool isFinance = false)
        {
            Result<string> result = new Result<string>();
            result.status = 2;
            foreach (var item in rno)
            {
                ApprovalPaper eFormPaper = await queryPaper(item);
                if (eFormPaper != null)
                {
                    if (eFormPaper.status == "P")
                    {
                        if (eFormPaper.emplid == emplid || isFinance)
                        {
                            eFormPaper.status = "A";
                            eFormPaper.aemplid = emplid;
                            await _EFormPaperRepository.UpdateAsync(eFormPaper);
                            Result<bool> isSigned = await _BPMDomainService.BPMQueryIsSigned(eFormPaper.rno, eFormPaper.formcode, token);
                            if (isSigned.status != 1)
                            {
                                result.message = isSigned.message;
                                return result;
                            }
                            if (isSigned.data)
                                await _AccountDomainService.SaveAccountInfo(item, emplid);
                        }
                        else
                        {
                            result.message = item + "" + L["notTheCurrentSigner"];
                            return result;
                        }
                    }
                    else
                    {
                        result.message = item + "" + L["nonSignoff"];
                        return result;
                    }
                }
                else
                {
                    result.message = item + "" + L["docNotFound"];
                    return result;
                }
            }
            result.status = 1;
            result.message = L["signSucess"];
            return result;
        }
        // 签核api(报销单中签核)
        public async Task<Result<string>> SignPaper(List<string> rno, string emplid, bool isFinance = false)
        {
            Result<string> result = new Result<string>();
            result.status = 2;
            foreach (var item in rno)
            {
                ApprovalPaper eFormPaper = await queryPaper(item);
                if (eFormPaper != null)
                {
                    if (eFormPaper.status == "P")
                    {
                        if (eFormPaper.emplid == emplid || isFinance)
                        {
                            eFormPaper.status = "A";
                            eFormPaper.aemplid = emplid;
                            await _EFormPaperRepository.UpdateAsync(eFormPaper);
                        }
                        else
                        {
                            result.message = item + "" + L["notTheCurrentSigner"];
                            return result;
                        }
                    }
                    else
                    {
                        result.message = item + "" + L["nonSignoff"];
                        return result;
                    }
                }
                else
                {
                    result.message = item + "" + L["docNotFound"];
                    return result;
                }
            }
            result.status = 1;
            result.message = L["signSucess"];
            return result;
        }
        public async Task<Result<string>> addPaper(string rno, string formcode, string user, string company)
        {
            Result<string> result = new Result<string>();
            ApprovalPaper eFormPaper = await queryPaper(rno);
            if (eFormPaper != null)
            {
                eFormPaper.status = "P";
                eFormPaper.muser = user;
                eFormPaper.mdate = System.DateTime.Now;
                await _EFormPaperRepository.UpdateAsync(eFormPaper);
                return result;
            }
            Employee employee = await _EmployeeDomainService.QueryEmployeeAsync(user);
            BDPaperSign bDPaperSign = (await _BDPaperSignRepository.WithDetailsAsync()).Where(b => b.company == company && b.plant == employee.plant).FirstOrDefault();
            if (bDPaperSign == null)
                bDPaperSign = (await _BDPaperSignRepository.WithDetailsAsync()).Where(b => b.company == company && b.plant == "OTHERS").FirstOrDefault();
            if (bDPaperSign != null)
            {
                ApprovalPaper paper = new ApprovalPaper();
                paper.rno = rno;
                paper.status = "P";
                paper.aemplid = "";
                paper.formcode = formcode;
                paper.emplid = bDPaperSign.emplid;
                paper.company = company;
                paper.cuser = user;
                paper.cdate = System.DateTime.Now;
                await _EFormPaperRepository.InsertAsync(paper);
            }
            return result;
        }
        public async Task<Result<string>> RejectPaper(string rno, string user)
        {
            Result<string> result = new Result<string>();
            ApprovalPaper eFormPaper = await queryPaper(rno);
            if (eFormPaper != null)
            {
                eFormPaper.status = "R";
                await _EFormPaperRepository.UpdateAsync(eFormPaper);
            }
            return result;
        }
        public async Task<ApprovalPaper> queryPaper(string rno)
        {
            return (await _EFormPaperRepository.WithDetailsAsync()).Where(b => b.rno == rno).FirstOrDefault();
        }
        public async Task<Result<string>> UpdatePaper(string rno, string formcode, string user, string status)
        {
            ApprovalPaper eFormPaper = await queryPaper(rno);
            await _EFormPaperRepository.DeleteAsync(eFormPaper);
            return await addPaper(rno, formcode, user, status);
        }
    }
}
