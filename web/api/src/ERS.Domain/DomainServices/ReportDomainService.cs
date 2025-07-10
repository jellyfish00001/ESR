using ERS.Application.Contracts.DTO.Report;
using ERS.Domain.IDomainServices;
using ERS.DomainServices;
using ERS.DTO;
using ERS.DTO.AppConfig;
using ERS.DTO.Application;
using ERS.DTO.Auditor;
using ERS.DTO.Report;
using ERS.Entities;
using ERS.IDomainServices;
using ERS.IRepositories;
using ERS.Minio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;
using System.Xml.Linq;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
namespace ERS.Domain.DomainServices
{
    public class ReportDomainService : CommonDomainService, IReportDomainService
    {
        private IRepository<EFormHead, Guid> _eformheadRepository;
        private IRepository<CashHead, Guid> _cashheadRepository;
        private IRepository<ApprovalFlow, Guid> _approvalflowRepository;
        private IRepository<CashDetail, Guid> _cashdetailRepository;
        private IRepository<CashAmount, Guid> _cashamountRepository;
        private IRepository<BDForm, Guid> _bdformRepository;
        private IRepository<ApprovalPaper, Guid> _eformpaperRepository;
        private IRepository<ChargeAgainst, Guid> _chargeagainstRepository;
        private ICashPaymentDetailRepository _CashPaylistRepository;
        private ICashFileRepository _cashfileRepository;
        private IEFormSignlogRepository _EFormSignlogRepository;
        private IObjectMapper _objectMapper;
        private IMinioRepository _MinioRepository;
        private IRepository<BDCashReturn, Guid> _bdcashreturnRepository;
        private IEFormAlistRepository _EFormAlistRepository;
        private IEFormAuserRepository _EFormAuserRepository;
        private IEmployeeRepository _EmployeeRepository;
        private IInvoiceRepository _invoiceRepository;
        private IApprovalFlowRepository _ApprovalFlowRepository;
        private IApprovalAssignedApproverRepository _ApprovalAssignedApproverRepository;


        public ReportDomainService
        (
        IRepository<EFormHead, Guid> eformheadRepository,
        IApprovalFlowRepository iApprovalFlowRepository,
        IRepository<CashHead, Guid> cashheadRepository,
        IRepository<ApprovalFlow, Guid> approvalflowRepository,
        IRepository<CashDetail, Guid> cashdetailRepository,
        IRepository<CashAmount, Guid> cashamountRepository,
        IRepository<BDForm, Guid> bdformRepository,
        IRepository<ApprovalPaper, Guid> eformpaperRepository,
        IRepository<BDCashReturn, Guid> bdcashreturnRepository,
        IRepository<ChargeAgainst, Guid> chargeagainstRepository,
        ICashFileRepository cashfileRepository,
        ICashPaymentDetailRepository CashPaylistRepository,
        IEFormSignlogRepository EFormSignlogRepository,
        IObjectMapper objectMapper,
        IMinioRepository MinioRepository,
        IEFormAlistRepository EFormAlistRepository,
        IEFormAuserRepository EFormAuserRepository,
        IEmployeeRepository EmployeeRepository,
        IInvoiceRepository InvoiceRepository
        )
        {
            _ApprovalFlowRepository = iApprovalFlowRepository;
            _eformheadRepository = eformheadRepository;
            _cashheadRepository = cashheadRepository;
            _approvalflowRepository = approvalflowRepository;
            _cashdetailRepository = cashdetailRepository;
            _cashamountRepository = cashamountRepository;
            _bdformRepository = bdformRepository;
            _eformpaperRepository = eformpaperRepository;
            _cashfileRepository = cashfileRepository;
            _objectMapper = objectMapper;
            _MinioRepository = MinioRepository;
            _bdcashreturnRepository = bdcashreturnRepository;
            _chargeagainstRepository = chargeagainstRepository;
            _CashPaylistRepository = CashPaylistRepository;
            _EFormSignlogRepository = EFormSignlogRepository;
            _EFormAlistRepository = EFormAlistRepository;
            _EFormAuserRepository = EFormAuserRepository;
            _EmployeeRepository = EmployeeRepository;
            _invoiceRepository = InvoiceRepository;
        }

        //查询已申请表单
        public async Task<Result<List<ReportDto>>> queryPageReport1(Request<ReportQueryDto> request)
        {
            Result<List<ReportDto>> result = new Result<List<ReportDto>>()
            {
                data = new List<ReportDto>()
            };
            int pageIndex = request.pageIndex;
            int pageSize = request.pageSize;
            if (pageIndex < 1 || pageSize < 0)
            {
                pageIndex = 1;
                pageSize = 10;
            }
            //A
            var query = (await _cashheadRepository.WithDetailsAsync());
            query = query
                 .WhereIf(request.data.startdate.HasValue, q => q.cdate.Value.Date >= request.data.startdate.Value.Date)
                 .WhereIf(request.data.enddate.HasValue, q => q.cdate.Value.Date <= request.data.enddate.Value.Date)
                 .WhereIf(!string.IsNullOrEmpty(request.data.rno), q => q.rno == request.data.rno)
                 .WhereIf(!string.IsNullOrEmpty(request.data.cemplid), q => q.cuser.ToUpper() == request.data.cemplid.ToUpper())
                 .WhereIf(!request.data.status.IsNullOrEmpty(), q => request.data.status.Contains(q.status))
                 .WhereIf(!request.data.category.Any() && request.data.category.Count > 1, q => request.data.category.Contains(request.data.rno.Substring(0, 1)))
                 .WhereIf(!request.data.formcode.IsNullOrEmpty(), q => request.data.formcode.Contains(q.formcode))
                 .WhereIf(!request.data.company.IsNullOrEmpty(), q => request.data.company.Contains(q.company));
            result.total = await query.CountAsync();

            if (result.total == 0)
            {
                return result;
            }
            var cashHeadPage = await query.OrderByDescending(w => w.cdate).Skip((pageIndex - 1) * pageSize).Take(pageSize).Select(w => new
            {
                w.rno,
                w.formcode,
                w.cuser,
                w.cdate,
                w.cname,
                w.deptid,
                w.status,
                w.company,
                w.payment,
                w.paymentdate,
                w.currency,
                w.actualamount,

            })
            .AsNoTracking()
            .ToListAsync();


            // 获取 cashdetail 中的 deptid 数据
            var rnoList = cashHeadPage.Select(q => q.rno).Distinct().ToList();

            var cashDetailDeptData = (await _cashdetailRepository.WithDetailsAsync())
                .AsQueryable()
                .Where(w => rnoList.Contains(w.rno))
                .Select(w => new { w.rno, w.deptid, w.expname, w.rdate })
                .ToList();

            // 建立 rno 到逗号分隔的 deptid 字典
            var expDeptDict = cashDetailDeptData
                .GroupBy(w => w.rno)
                .ToDictionary(
                    g => g.Key,
                    g => string.Join(",", g.Select(x => x.deptid).Distinct()) // 部门按逗号拼接
                );
            var expnameDict = cashDetailDeptData
                .GroupBy(w => w.rno)
                .ToDictionary(
                    g => g.Key,
                    g => string.Join(",", g.Select(x => x.expname).Distinct())
                );

            var approvalflowData = (await _approvalflowRepository.WithDetailsAsync())
                .AsQueryable()
                .Where(w => rnoList.Contains(w.rno))
                .Select(w => new { w.rno, w.step, w.stepname })
                .ToList();
            // 创建 rno -> step 和 stepname 的字典
            var approvalFlowDict = approvalflowData
                .GroupBy(w => w.rno)
                .ToDictionary(
                    g => g.Key,
                    g => g.FirstOrDefault() // 取分组的第一个值
                );

            var formcodes = cashHeadPage.Select(q => q.formcode).Distinct().ToList();
            var menukeData = (await _bdformRepository.WithDetailsAsync())
               .AsQueryable()
               .Where(w => formcodes.Contains(w.FormCode))
               .Select(w => new { w.FormCode, w.ApplicationMenuKey, w.SignMenuKey, w.FormName })
               .ToList();
            // 创建 rno -> step 和 stepname 的字典
            var menukeyDict = menukeData
                .GroupBy(w => w.FormCode)
             .ToDictionary(
                    g => g.Key,
                    g => new
                    {
                        ApplicationMenuKey = g.Select(x => x.ApplicationMenuKey).FirstOrDefault(),
                        SignMenuKey = g.Select(x => x.SignMenuKey).FirstOrDefault(),
                        formname = g.Select(x => x.FormName).FirstOrDefault()

                    }
                );


            // 将 cashhead 数据映射到 ReportDto
            result.data = cashHeadPage.Select(q =>
            {

                var expdeptid = expDeptDict.GetValueOrDefault(q.rno);
                var expname = expnameDict.GetValueOrDefault(q.rno);
                var approvalFlow = approvalFlowDict.GetValueOrDefault(q.rno);
                var menukeys = menukeyDict.GetValueOrDefault(q.formcode, new { ApplicationMenuKey = "", SignMenuKey = "", formname = "" });



                return new ReportDto
                {
                    // 栏位映射
                    rno = q.rno,
                    formcode = q.formcode,
                    formname = menukeys.formname,
                    deptid = q.deptid,
                    company = q.company,
                    status = q.status,
                    cdate = q.cdate,
                    cemplid = q.cuser,
                    cname = q.cname,
                    expdeptid = expdeptid,
                    step = approvalFlow?.step ?? 0,
                    stepname = approvalFlow?.stepname ?? null,
                    payment = q.payment,
                    expname = expname,
                    currency = q.currency,
                    actamt = q.actualamount,
                    apid = (q.status == "temporary" || q.status == "canceled")
                            ? menukeys.ApplicationMenuKey
                            : (q.status == "rejected"
                            ? menukeys.SignMenuKey
                            : menukeys.SignMenuKey),
                };
            }).ToList();
            // 返回结果
            return result;
        }

        public async Task<Result<List<ReportDto>>> queryPageReport(Request<ReportQueryDto> request)
        {
            Result<List<ReportDto>> result = new Result<List<ReportDto>>()
            {
                data = new List<ReportDto>()
            };

            // 校验分页参数
            int pageIndex = request.pageIndex;
            int pageSize = request.pageSize;
            if (pageIndex < 1 || pageSize < 0)
            {
                pageIndex = 1;
                pageSize = 10;
            }

            // 使用表连接查询数据
            var query = from cashHead in (await _cashheadRepository.WithDetailsAsync())
                        where !cashHead.isdeleted
                        join bdForm in (await _bdformRepository.WithDetailsAsync())
                            on cashHead.formcode equals bdForm.FormCode into bdFormGroup
                        from bdForm in bdFormGroup.DefaultIfEmpty() 
                        join cashAmount in (await _cashamountRepository.WithDetailsAsync())
                            on cashHead.rno equals cashAmount.rno into cashAmountGroup
                        from cashAmount in cashAmountGroup
                            .Where(ca => !ca.isdeleted).DefaultIfEmpty()

                        where (
                            (!request.data.startdate.HasValue || (cashHead.cdate != null && cashHead.cdate.Value.Date >= request.data.startdate.GetValueOrDefault().Date))
                            && (!request.data.enddate.HasValue || (cashHead.cdate != null && cashHead.cdate.Value.Date <= request.data.enddate.GetValueOrDefault().Date))
                            && (string.IsNullOrEmpty(request.data.rno) || cashHead.rno == request.data.rno)
                            && (string.IsNullOrEmpty(request.data.cemplid) || (!string.IsNullOrEmpty(cashHead.cuser) && cashHead.cuser.ToUpper() == request.data.cemplid.ToUpper()))
                            && (request.data.formcode == null || request.data.formcode.Count == 0 || request.data.formcode.Contains(cashHead.formcode))
                            && (request.data.company == null || request.data.company.Count == 0 || request.data.company.Contains(cashHead.company))
                            && (request.data.status == null || request.data.status.Count == 0 || request.data.status.Contains(cashHead.status))
                        )
                        select new
                        {
                            cashHead.rno,
                            cashHead.formcode,
                            cashHead.cuser,
                            cashHead.cdate,
                            cashHead.cname,
                            cashHead.deptid,
                            cashHead.status,
                            cashHead.company,
                            cashHead.payment,
                            cashHead.paymentdate,
                            hcurrency=cashHead.currency,
                            hactualamount=cashHead.actualamount,
                            acurrency = cashAmount != null ? cashAmount.currency : "",
                            aactamt = cashAmount != null ? cashAmount.actamt : 0,
                            bdForm.ApplicationMenuKey,
                            bdForm.SignMenuKey,
                            bdForm.FormName
                        };

            result.total = await query.CountAsync();

            if (result.total == 0)
            {
                return result;
            }

            // 分页查询
            var pagedQuery = await query
                .OrderByDescending(x => x.cdate)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // 获取 cashdetail 中的 deptid 数据
            var rnoList = pagedQuery.Select(q => q.rno).Distinct().ToList();

            var cashDetailDeptData = (await _cashdetailRepository.WithDetailsAsync())
                .AsQueryable()
                .Where(w => rnoList.Contains(w.rno))
                .Select(w => new { w.rno, w.deptid, w.expname, w.rdate })
                .ToList();

            // 建立 rno 到逗号分隔的 deptid 字典
            var expDeptDict = cashDetailDeptData
                .Select(w => new
                {
                    rno = w.rno,
                    deptid = (w.deptid != null && w.deptid.StartsWith("[") && w.deptid.EndsWith("]"))
                        ? string.Join(",", JsonConvert.DeserializeObject<List<departCost>>(w.deptid)
                            .Select(x => x.deptId)
                            .Distinct())
                        : w.deptid,
                })
                .GroupBy(s => s.rno)
                .Select(g => new
                {
                    rno = g.Key,
                    expdeptid = ProcessExpDept(
                        g.Select(i => i.deptid)
                        .Distinct()
                        .ToList()
                    )
                })
                .ToList();
            string ProcessExpDept(List<string> deptids)
            {
                if (deptids.Count > 3)
                {
                    return string.Join(",", deptids.Take(3)) + "...";
                }
                return string.Join(",", deptids);
            }
            var expnameDict = cashDetailDeptData
                .GroupBy(w => w.rno)
                .ToDictionary(
                    g => g.Key,
                    g => string.Join(",", g.Select(x => x.expname).Distinct())
                );

            var approvalflowData = (await _approvalflowRepository.WithDetailsAsync())
                .AsQueryable()
                .Where(w => rnoList.Contains(w.rno))
                .Select(w => new { w.rno, w.step, w.stepname })
                .ToList();
            // 创建 rno -> step 和 stepname 的字典
            var approvalFlowDict = approvalflowData
                .GroupBy(w => w.rno)
                .ToDictionary(
                    g => g.Key,
                    g => g.FirstOrDefault() // 取分组的第一个值
                );



            // 映射到 ReportDto
            result.data = pagedQuery.Select(q =>
            {
                // Replace the line using GetValueOrDefault with the following code
                var expdeptid = expDeptDict.FirstOrDefault(x => x.rno == q.rno)?.expdeptid;
                var expname = expnameDict.GetValueOrDefault(q.rno);
                var approvalFlow = approvalFlowDict.GetValueOrDefault(q.rno);
                return new ReportDto
                {
                    rno = q.rno,
                    formcode = q.formcode,
                    formname = q.FormName,
                    deptid = q.deptid,
                    company = q.company,
                    status = q.status,
                    cdate = q.cdate,
                    cemplid = q.cuser,
                    cname = q.cname,
                    expdeptid = expdeptid,
                    step = approvalFlow?.step ?? 0,
                    stepname = approvalFlow?.stepname ?? null,
                    payment = q.payment,
                    expname = expname,
                    currency = (q.formcode == "CASH_3A" || q.formcode == "CASH_UBER")
                                ? q.hcurrency
                                : q.acurrency,
                    actamt = (q.formcode == "CASH_3A" || q.formcode == "CASH_UBER")
                                ? q.hactualamount
                                : q.aactamt,
                    apid = (q.status == "temporary" || q.status == "canceled")
                              ? q.ApplicationMenuKey
                              : (q.status == "rejected"
                              ? q.SignMenuKey
                              : q.SignMenuKey),
                };
            }).ToList();

            return result;
        }

        //查询已申请表单明细多个invoice用，号连接
        public async Task<Result<List<ReportDto>>> queryPageReportDetail1(Request<ReportQueryDto> request)
        {
            // 初始化结果集
            Result<List<ReportDto>> result = new Result<List<ReportDto>>()
            {
                data = new List<ReportDto>()
            };

            // 校验分页参数
            int pageIndex = request.pageIndex;
            int pageSize = request.pageSize;
            if (pageIndex < 1 || pageSize < 0)
            {
                pageIndex = 1;
                pageSize = 10;
            }

            // 第一步：查询 Detail 表基础数据
            var query = (await _cashdetailRepository.WithDetailsAsync());
            query = query
                  .WhereIf(request.data.startdate.HasValue, q => q.cdate.Value.Date >= request.data.startdate.Value.Date)
                  .WhereIf(request.data.enddate.HasValue, q => q.cdate.Value.Date <= request.data.enddate.Value.Date)
                  .WhereIf(!string.IsNullOrEmpty(request.data.rno), q => q.rno == request.data.rno)
                  .WhereIf(!string.IsNullOrEmpty(request.data.cemplid), q => q.cuser.ToUpper() == request.data.cemplid.ToUpper())
                  //.WhereIf(!request.data.category.Any() && request.data.category.Count > 1, q => request.data.category.Contains(request.data.rno.Substring(0, 1)))
                  .WhereIf(!request.data.formcode.IsNullOrEmpty(), q => request.data.formcode.Contains(q.formcode))
                  .WhereIf(!request.data.company.IsNullOrEmpty(), q => request.data.company.Contains(q.company));


            var sql = query.ToQueryString();
            result.total = await query.CountAsync();

            if (result.total == 0)
            {
                return result;
            }
            var cashDetailPage = await query.OrderByDescending(w => w.cdate).Skip((pageIndex - 1) * pageSize).Take(pageSize).Select(w => new
            {
                w.rno,
                w.formcode,
                w.cuser,
                cdate = Convert.ToDateTime(w.cdate).Date,
                w.company,
                w.currency,
                w.rdate,
                w.deptid,
                w.expname,
                w.summary,
                w.seq
            })
            .AsNoTracking()
            .ToListAsync();


            var rnoList = cashDetailPage.Select(q => q.rno).Distinct().ToList();
            // 第二步：查询 Head 表数据
            var cashHeadData = (await _cashheadRepository.WithDetailsAsync())
                .Where(head => rnoList.Contains(head.rno)) // 通过 rno 过滤
                .Select(head => new
                {
                    rno = head.rno,
                    cname = head.cname,
                    cuser = head.cuser,
                    deptid = head.deptid,
                    cdate = Convert.ToDateTime(head.cdate).Date,
                    payment = head.payment,
                    paymentdate = head.paymentdate,
                    currency = head.currency,
                    actualamount = head.actualamount,
                    projectcode = head.projectcode,
                    status = head.status,
                    payeeId = head.payeeId,
                    payeename = head.payeename,
                    expensetype = head.expensetype

                })
                .AsNoTracking()
                .ToList();

            var cashHeadDict = cashHeadData
                .GroupBy(head => head.rno)
                .ToDictionary(
                g => g.Key, // rno 作为键
                g => g.First() // 每组中取第一条记录
                );
            // 第三步：将 Detail 和 Head 数据进行合并
            var combinedQuery = cashDetailPage
                .Select(detailItem =>
                {
                    // 根据 rno 从 Head 字典中获取对应的数据
                    var headData = cashHeadDict.GetValueOrDefault(detailItem.rno);


                    return new
                    {
                        // 从 Detail 表获取数据
                        detailItem.rno,
                        detailItem.formcode,
                        detailItem.rdate,
                        detailItem.deptid,
                        detailItem.expname,
                        detailItem.company,
                        detailItem.summary,
                        detailItem.seq,


                        // 从 Head 表补充数据
                        cuser = headData?.cuser,
                        cname = headData?.cname,
                        dept = headData?.deptid,
                        cdate = headData?.cdate,
                        payment = headData?.payment,
                        paymentdate = headData?.paymentdate,
                        currency = headData?.currency,
                        actualamount = headData?.actualamount,
                        projectcode = headData?.projectcode,
                        status = headData?.status,
                        payeeId = headData?.payeeId,
                        payeename = headData?.payeename,
                        expensetype = headData?.expensetype
                    };
                })
                // 根据条件过滤数据
                .WhereIf(!request.data.status.IsNullOrEmpty(), q => request.data.status.Contains(q.status))
                .OrderByDescending(q => q.cdate) // 按日期倒序
                .ToList();

            // 如果经过条件过滤后为空，则返回
            if (combinedQuery.Count == 0)
                return result;



            var voiceData = (await _invoiceRepository.WithDetailsAsync())
                            .AsQueryable()
                            .Where(w => rnoList.Contains(w.rno))
                            .Select(w => new
                            {
                                w.rno,
                                w.seq,
                                w.invcode,
                                w.invno,
                                w.taxamount,
                                w.oamount
                            })
                            .ToList();

            // Invoice 数据按 rno 和 seq 分组，并将重复数据用逗号拼接
            var voiceDict = voiceData
                .GroupBy(w => new { w.rno, w.seq }) // 按 rno 和 seq 分组
                .ToDictionary(
                    g => g.Key, // 键为 {rno, seq}
                    g => new
                    {
                        invcode = string.Join(",", g.Select(x => x.invcode).Distinct()),
                        invno = string.Join(",", g.Select(x => x.invno).Distinct()),
                        taxamount = string.Join(",", g.Select(x => x.taxamount).Distinct()), // 汇总税额
                        oamount = string.Join(",", g.Select(x => x.oamount).Distinct()), // 汇总税前金额
                    }
                );

            var approvalflowData = (await _approvalflowRepository.WithDetailsAsync())
                .AsQueryable()
                .Where(w => rnoList.Contains(w.rno))
                .Select(w => new { w.rno, w.step, w.stepname })
                .ToList();
            // 创建 rno -> step 和 stepname 的字典
            var approvalFlowDict = approvalflowData
                .GroupBy(w => w.rno)
                .ToDictionary(
                    g => g.Key,
                    g => g.FirstOrDefault() // 取分组的第一个值
                );

            var formcodes = cashDetailPage.Select(q => q.formcode).Distinct().ToList();
            var menukeData = (await _bdformRepository.WithDetailsAsync())
               .AsQueryable()
               .Where(w => formcodes.Contains(w.FormCode))
               .Select(w => new { w.FormCode, w.ApplicationMenuKey, w.SignMenuKey, w.FormName })
               .ToList();
            // 创建 rno -> step 和 stepname 的字典
            var menukeyDict = menukeData
                .GroupBy(w => w.FormCode)
             .ToDictionary(
                    g => g.Key,
                    g => new
                    {
                        ApplicationMenuKey = g.Select(x => x.ApplicationMenuKey).FirstOrDefault(),
                        SignMenuKey = g.Select(x => x.SignMenuKey).FirstOrDefault(),
                        formname = g.Select(x => x.FormName).FirstOrDefault()

                    }
                );
            // 第五步：映射到 DTO（返回最终数据格式）
            result.data = combinedQuery.Select(q =>
            {
                var approvalFlow = approvalFlowDict.GetValueOrDefault(q.rno);
                var voiceDataForRnoSeq = voiceDict.GetValueOrDefault(new { q.rno, q.seq },
                                 new { invcode = "", invno = "", taxamount = "", oamount = "" });
                var menukeys = menukeyDict.GetValueOrDefault(q.formcode, new { ApplicationMenuKey = "", SignMenuKey = "", formname = "" });
                string expdeptid = ProcessDeptId(q.deptid);


                return new ReportDto
                {
                    rno = q.rno,
                    formcode = q.formcode,
                    formname = menukeys.formname,
                    cname = q.cname,
                    deptid = q.deptid,
                    company = q.company,
                    status = q.status,
                    cdate = q.cdate,
                    cemplid = q.cuser,
                    step = approvalFlow?.step ?? 0,
                    stepname = approvalFlow?.stepname ?? null,
                    payment = q.payment,
                    expdeptid = expdeptid,
                    expname = q.expname,
                    currency = q.currency,
                    actamt = q.actualamount,
                    rdate = q.rdate,
                    expensetype = q.expensetype,
                    invno = voiceDataForRnoSeq.invno,
                    invcode = voiceDataForRnoSeq.invcode,
                    projectcode = q.projectcode,
                    taxamountDisplay = voiceDataForRnoSeq.taxamount,      // 修改后直接显示拼接的字符串
                    untaxamountDisplay = voiceDataForRnoSeq.oamount,              //税前总金额
                    apid = (q.status == "temporary" || q.status == "canceled")
                            ? menukeys.ApplicationMenuKey
                            : (q.status == "rejected"
                            ? menukeys.SignMenuKey
                            : menukeys.SignMenuKey),
                    payeeId = q.payeeId,
                    payeename = q.payeename,
                    summary = q.summary,

                };
            }).ToList();

            // 返回结果
            return result;
        }

        public async Task<Result<List<ReportDto>>> queryPageReportDetail(Request<ReportQueryDto> request)
        {
            Result<List<ReportDto>> result = new Result<List<ReportDto>>()
            {
                data = new List<ReportDto>()
            };

            // 校验分页参数
            int pageIndex = request.pageIndex;
            int pageSize = request.pageSize;
            if (pageIndex < 1 || pageSize < 0)
            {
                pageIndex = 1;
                pageSize = 10;
            }

            // 使用表连接查询数据
            var query = from detail in (await _cashdetailRepository.WithDetailsAsync())
                        where !detail.isdeleted 
                        join head in (await _cashheadRepository.WithDetailsAsync())
                            on detail.rno equals head.rno into headGroup
                        from head in headGroup.DefaultIfEmpty() 
                        where !head.isdeleted 
                        join form in (await _bdformRepository.WithDetailsAsync())
                            on head.formcode equals form.FormCode into formGroup
                        from form in formGroup.DefaultIfEmpty()
                        where !form.isdeleted
                        join cashAmount in (await _cashamountRepository.WithDetailsAsync())
                            on head.rno equals cashAmount.rno into cashAmountGroup
                        from cashAmount in cashAmountGroup
                            .Where(cashAmount => !cashAmount.isdeleted) // 对 cash_amount 表增加过滤条件
                            .DefaultIfEmpty()
                        where (
                            (!request.data.startdate.HasValue || (detail.cdate != null && detail.cdate.Value.Date >= request.data.startdate.GetValueOrDefault().Date))
                            && (!request.data.enddate.HasValue || (detail.cdate != null && detail.cdate.Value.Date <= request.data.enddate.GetValueOrDefault().Date)) && (string.IsNullOrEmpty(request.data.rno) || detail.rno == request.data.rno)
                            && (string.IsNullOrEmpty(request.data.cemplid) || (!string.IsNullOrEmpty(detail.cuser) && detail.cuser.ToUpper() == request.data.cemplid.ToUpper()))
                            && (request.data.formcode == null || request.data.formcode.Count == 0 || request.data.formcode.Contains(detail.formcode))
                            && (request.data.company == null || request.data.company.Count == 0 || request.data.company.Contains(detail.company))
                            && (request.data.status == null || request.data.status.Count == 0 || request.data.status.Contains(head.status))
                            )

                        select new
                        {
                            detail.rno,
                            detail.formcode,
                            detail.cuser,
                            cdate = detail != null ? detail.cdate : null,
                            detail.company,
                            hcurrency = detail.currency,
                            hactualamount = detail.amount,
                            acurrency = cashAmount != null ? cashAmount.currency : "",
                            aactamt = cashAmount != null ? cashAmount.actamt : 0,
                            detail.rdate,
                            detail.deptid,
                            detail.expname,
                            detail.summary,
                            detail.seq,
                            head.status,
                            head.payment,
                            head.paymentdate,
                            head.projectcode,
                            head.payeeId,
                            head.cname,
                            applyDept = head.deptid,
                            head.payeename,
                            head.expensetype,
                            ApplicationMenuKey = form != null ? form.ApplicationMenuKey : null,
                            SignMenuKey = form != null ? form.SignMenuKey : null,
                            FormName = form != null ? form.FormName : null
                        };

            result.total = await query.CountAsync();

            if (result.total == 0)
            {
                return result;
            }

            // 分页查询
            var pagedQuery = await query
                .OrderByDescending(x => x.rno)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var rnoList = pagedQuery.Select(q => q.rno).Distinct().ToList();
            var approvalflowData = (await _approvalflowRepository.WithDetailsAsync())
                .AsQueryable()
                .Where(w => rnoList.Contains(w.rno) && !w.isdeleted)
                .Select(w => new { w.rno, w.step, w.stepname })
                .ToList();
            // 创建 rno -> step 和 stepname 的字典
            var approvalFlowDict = approvalflowData
                .GroupBy(w => w.rno)
                .ToDictionary(
                    g => g.Key,
                    g => g.FirstOrDefault() // 取分组的第一个值
                );

            var seqList = pagedQuery.Select(q => q.seq).Distinct().ToList();

            // 获取 Invoice 数据
            var invoiceData = (await _invoiceRepository.WithDetailsAsync())
                 .Where(w => rnoList.Contains(w.rno) && !w.isdeleted)
                .GroupBy(w => new { w.rno, w.seq })
                .Select(g => new
                {
                    rno = g.Key.rno,
                    seq = g.Key.seq,
                    invcode = g.Count() > 1 ? $"{g.First().invcode},..." : g.First().invcode,
                    invno = g.Count() > 1 ? $"{g.First().invno},..." : g.First().invno,
                    taxamount = g.Count() > 1 ? $"{g.First().taxamount},..." : g.First().taxamount.ToString(),
                    oamount = g.Count() > 1 ? $"{g.First().oamount},..." : g.First().oamount.ToString(),
                })
                .ToDictionary(w => new { w.rno, w.seq });

            result.data = pagedQuery.Select(x =>
            {
                var invoice = invoiceData.GetValueOrDefault(new { rno = x.rno, seq = x.seq });
                var approvalFlow = approvalFlowDict.GetValueOrDefault(x.rno);
                string expdeptid = ProcessDeptId(x.deptid);
                return new ReportDto
                {
                    rno = x.rno,
                    formcode = x.formcode,
                    formname = x.FormName,
                    cname = x.cname,
                    deptid = x.applyDept,
                    company = x.company,
                    status = x.status,
                    cdate = x.cdate,
                    cemplid = x.cuser,
                    step = approvalFlow?.step ?? 0, // 添加空值检查
                    stepname = approvalFlow?.stepname,
                    payment = x.payment,
                    expdeptid = expdeptid,
                    expname = x.expname,
                    currency = (x.formcode == "CASH_3A" || x.formcode == "CASH_UBER")
                                ? x.hcurrency
                                : x.acurrency,
                    actamt = (x.formcode == "CASH_3A" || x.formcode == "CASH_UBER")
                                ? x.hactualamount
                                : x.aactamt,
                    rdate = x.rdate,
                    expensetype = x.expensetype,
                    projectcode = x.projectcode,
                    invcode = invoice?.invcode ?? "",
                    invno = invoice?.invno ?? "",
                    taxamountDisplay = invoice?.taxamount ?? "",
                    untaxamountDisplay = invoice?.oamount ?? "",
                    apid = (x.status == "temporary" || x.status == "canceled")
                        ? x.ApplicationMenuKey
                        : (x.status == "rejected"
                        ? x.SignMenuKey
                        : x.SignMenuKey),
                    payeeId = x.payeeId,
                    payeename = x.payeename,
                    summary = x.summary
                };
            }).ToList();

            return result;
        }

        // 下载已申请表单
        public async Task<XSSFWorkbook> DownloadReport(Request<ReportQueryDto> request)
        {
            var cashHeadCount = (await _cashheadRepository.WithDetailsAsync()).AsNoTracking().Count();
            // Request<ReportQueryDto> downloadRequest = new()
            // {
            //     pageIndex = 1,
            //     pageSize = eFormHeadCount,
            //     data = new()
            //     {
            //         category = new(),
            //         company = new(),
            //         formcode = new(),
            //         status = new()
            //     }
            // };
            // request.pageIndex = 1;
            request.pageSize = cashHeadCount;
            Result<List<ReportDto>> result = await queryPageReport(request);
            var invoices = (await _cashfileRepository.WithDetailsAsync()).Where(w => w.status != "F").ToList();
            var attachments = (await _cashfileRepository.WithDetailsAsync()).Where(w => w.status == "F").ToList();
            var abnormalQuery = (await _invoiceRepository.WithDetailsAsync()).Where(s => result.data.Select(w => w.rno).Contains(s.rno)).AsNoTracking().ToList();
            foreach (var item in result.data)
            {
                item.invoices = invoices.Where(w => w.rno == item.rno).Select(s => s.filename).ToList();
                item.attachments = attachments.Where(w => w.rno == item.rno).Select(s => s.filename).ToList();
                item.isAbnormal = abnormalQuery.Any(w => w.abnormal == "Y" && w.rno == item.rno) ? "Y" : "N";
                item.abnormalReson = abnormalQuery.Where(s => s.rno == item.rno).Select(w => w.abnormalmsg).ToList();
            }
            XSSFWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("sheet");
            if (result != null && result.data.Count() > 0)
            {
                // ISheet sheet = workbook.CreateSheet("sheet");
                for (int i = 0; i < result.data.Count; i++)
                {
                    IRow dataRow = sheet.CreateRow(i + 1);
                    dataRow.CreateCell(0).SetCellValue(result.data[i].company);
                    dataRow.CreateCell(1).SetCellValue(result.data[i].rno);
                    dataRow.CreateCell(2).SetCellValue(result.data[i].formname);
                    dataRow.CreateCell(3).SetCellValue(result.data[i].deptid);
                    dataRow.CreateCell(4).SetCellValue(result.data[i].cemplid);
                    dataRow.CreateCell(5).SetCellValue(result.data[i].cname);
                    dataRow.CreateCell(6).SetCellValue(Convert.ToDateTime(result.data[i].cdate).ToString("yyyy/MM/dd"));
                    dataRow.CreateCell(7).SetCellValue(result.data[i].expname);
                    dataRow.CreateCell(8).SetCellValue(result.data[i].expdeptid);
                    dataRow.CreateCell(9).SetCellValue(result.data[i].currency);
                    dataRow.CreateCell(10).SetCellValue(result.data[i].actamt.ToString());
                    dataRow.CreateCell(11).SetCellValue(result.data[i].stepname);
                    dataRow.CreateCell(12).SetCellValue((DateTime)result.data[i].payment);
                    string tInvoices = string.Join(";", result.data[i].invoices);
                    dataRow.CreateCell(13).SetCellValue(tInvoices);
                    string tAttachments = string.Join(";", result.data[i].attachments);
                    dataRow.CreateCell(14).SetCellValue(tAttachments);
                    dataRow.CreateCell(15).SetCellValue(result.data[i].isAbnormal);
                    //string abnormalResons = string.Join(";", result.data[i].abnormalReson);
                    string abnormalResons = string.Join(";", result.data[i].abnormalReson.Where(x => x != null));
                    dataRow.CreateCell(16).SetCellValue(result.data[i].isAbnormal == "Y" ? abnormalResons : string.Empty);
                }
            }
            else
            {
                IRow dataRow = sheet.CreateRow(0);
            }
            return workbook;
        }
        //下载已申请明细表单
        public async Task<XSSFWorkbook> DownloadAppliedDetailReport(Request<ReportQueryDto> request)
        {
            var cashHeadCount = (await _cashheadRepository.WithDetailsAsync()).AsNoTracking().Count();

            request.pageSize = cashHeadCount;
            Result<List<ReportDto>> result = await queryPageReportDetail(request);

            XSSFWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("sheet");
            if (result != null && result.data.Count() > 0)
            {
                // ISheet sheet = workbook.CreateSheet("sheet");
                for (int i = 0; i < result.data.Count; i++)
                {
                    IRow dataRow = sheet.CreateRow(i + 1);
                    dataRow.CreateCell(0).SetCellValue(result.data[i].company);
                    dataRow.CreateCell(1).SetCellValue(result.data[i].rno);
                    dataRow.CreateCell(2).SetCellValue(result.data[i].formcode);
                    dataRow.CreateCell(3).SetCellValue(result.data[i].deptid);
                    dataRow.CreateCell(4).SetCellValue(result.data[i].cemplid);
                    dataRow.CreateCell(5).SetCellValue(result.data[i].cname);
                    dataRow.CreateCell(6).SetCellValue(result.data[i].summary);
                    dataRow.CreateCell(7).SetCellValue(result.data[i].expname);
                    dataRow.CreateCell(8).SetCellValue(Convert.ToDateTime(result.data[i].rdate).ToString("yyyy/MM/dd"));
                    dataRow.CreateCell(9).SetCellValue(result.data[i].invno);
                    dataRow.CreateCell(10).SetCellValue(result.data[i].expdeptid);
                    dataRow.CreateCell(11).SetCellValue(result.data[i].currency);
                    dataRow.CreateCell(12).SetCellValue(result.data[i].actamt.ToString());
                    dataRow.CreateCell(13).SetCellValue(result.data[i].untaxamountDisplay.ToString());
                    dataRow.CreateCell(14).SetCellValue(result.data[i].taxamountDisplay.ToString());
                    dataRow.CreateCell(15).SetCellValue(result.data[i].invcode);
                    dataRow.CreateCell(16).SetCellValue(Convert.ToDateTime(result.data[i].cdate).ToString("yyyy/MM/dd"));
                    dataRow.CreateCell(17).SetCellValue(result.data[i].formname);
                    dataRow.CreateCell(18).SetCellValue(result.data[i].projectcode);
                    dataRow.CreateCell(19).SetCellValue(result.data[i].payeeId);
                    dataRow.CreateCell(20).SetCellValue(result.data[i].payeename);
                    dataRow.CreateCell(21).SetCellValue(result.data[i].stepname);
                    dataRow.CreateCell(22).SetCellValue(result.data[i].apid);
                    dataRow.CreateCell(23).SetCellValue(Convert.ToDateTime(result.data[i].payment).ToString("yyyy/MM/dd"));
                }
            }
            else
            {
                IRow dataRow = sheet.CreateRow(0);
            }
            return workbook;
        }

        //已簽核表單查詢
        public async Task<Result<List<ReportDto>>> QuerySignedReport(Request<QuerySignedReportDto> request)
        {
            Result<List<ReportDto>> result = new Result<List<ReportDto>>()
            {
                data = new List<ReportDto>()
            };

            if (request.pageIndex < 1 || request.pageSize < 0)
            {
                request.pageIndex = 1;
                request.pageSize = 10;
            }

            // 获取审批流程数据
            var approvalFlowdata = (await _ApprovalFlowRepository.WithDetailsAsync())
                .Where(w => w.status == "approved" && !w.isdeleted )
                .Select(w => new
                {
                    rno = w.rno,
                    aemplid = w.approveremplid,
                    adate = w.cdate,
                    status = w.status,
                    step = w.step,
                    stepname = w.stepname,
                    approvedate = w.approvedate
                })
                .AsNoTracking()
                .ToList()
                .WhereIf(request.data.sdate.HasValue, w => Convert.ToDateTime(w.approvedate).Date >= Convert.ToDateTime(request.data.sdate).ToLocalTime().Date)
                .WhereIf(request.data.edate.HasValue, w => Convert.ToDateTime(w.approvedate).Date <= Convert.ToDateTime(request.data.edate).ToLocalTime().Date)
                .WhereIf(!string.IsNullOrEmpty(request.data.aemplid), w => w.aemplid == request.data.aemplid)
                .GroupBy(w => w.rno) // 按 rno 分组
                .Select(g => g.OrderByDescending(x => x.approvedate).FirstOrDefault())
                .ToList();


            if (approvalFlowdata.Count == 0)
            {
                return result;
            }

            // 获取 CashHead 数据
            var cashHeadData = (await _cashheadRepository.WithDetailsAsync())
                .Where(w => approvalFlowdata.Select(g => g.rno).Contains(w.rno) && !w.isdeleted)
                .WhereIf(!string.IsNullOrEmpty(request.data.cuser), w => w.cuser == request.data.cuser)
                .WhereIf(request.data.status.Count > 0, w => request.data.status.Contains(w.status))
                //.WhereIf(request.data.category.Count > 0, w => request.data.category.Contains(w.rno.Substring(0, 1)))
                .WhereIf(request.data.company.Count > 0, w => request.data.company.Contains(w.company))
                .WhereIf(request.data.formcode.Count > 0, w => request.data.formcode.Contains(w.formcode))
                .WhereIf(!string.IsNullOrEmpty(request.data.rno), w => w.rno == request.data.rno)
                .Select(w => new
                {
                    formcode = w.formcode,
                    rno = w.rno,
                    cuser = w.cuser,
                    cname = w.cname,
                    company = w.company,
                    cdate = w.cdate,
                    status = w.status,
                    deptid = w.deptid,
                    payment = w.payment,
                    currency = w.currency,
                    actamt = w.actualamount,
                    paymentdate = w.paymentdate
                })
                .AsNoTracking()
                .ToList()
                .OrderByDescending(i => i.cdate)
                .ToList();

            result.total = cashHeadData.Count;
            cashHeadData = cashHeadData.Skip((request.pageIndex - 1) * request.pageSize).Take(request.pageSize).ToList();

            // 获取 CashDetail 数据
            var cashDetails = (await _cashdetailRepository.WithDetailsAsync())
                .Where(w => cashHeadData.Select(x => x.rno).Contains(w.rno) && !w.isdeleted)
                .Select(w => new
                {
                    rno = w.rno,
                    deptid = w.deptid,
                    expname = w.expname
                })
                .Distinct()
                .AsNoTracking()
                .ToList();

            // 获取 BDForm 数据
            var bdFormQuery = (await _bdformRepository.WithDetailsAsync())
                .Where(w => cashHeadData.Select(x => x.formcode).Contains(w.FormCode) && !w.isdeleted)
                .Select(w => new
                {
                    w.FormCode,
                    w.FormName,
                    w.ApplicationMenuKey,
                    w.SignMenuKey
                })
                .Distinct()
                .AsNoTracking()
                .ToList();
            var cashAmounts = (await _cashamountRepository.WithDetailsAsync())
            .Where(w => cashHeadData.Select(x => x.rno).Contains(w.rno) && !w.isdeleted)
            .Select(w => new
            {
                rno = w.rno,
                currency = w.currency,
                actamt = (w.formcode == "CASH_UBER" || w.formcode == "CASH_3") ? w.amount : w.actamt
            }).AsNoTracking().ToList();

            // 处理费用归属部门和费用类型
            var expDetidQuery = cashDetails.Select(w => new
            {
                expdeptid = (w.deptid != null && w.deptid.StartsWith("[") && w.deptid.EndsWith("]"))
                    ? string.Join(",", (JsonConvert.DeserializeObject<List<departCost>>(w.deptid)).Select(x => x.deptId).Distinct())
                    : w.deptid,
                rno = w.rno
            })
            .GroupBy(s => s.rno)
            .Select(g => new
            {
                rno = g.Key,
                expdeptid = string.Join(",", g.Select(i => i.expdeptid).Distinct())
            })
            .ToList();

            var expNameQuery = cashDetails.Select(w => new
            {
                expname = w.expname,
                rno = w.rno
            })
            .GroupBy(s => s.rno)
            .Select(g => new
            {
                rno = g.Key,
                expname = string.Join(",", g.Select(i => i.expname).Distinct())
            })
            .ToList();

            var FQuery = (from b in expDetidQuery
                          join c in expNameQuery
                          on b.rno equals c.rno
                          select new
                          {
                              rno = b.rno,
                              expdeptid = b.expdeptid,
                              expname = c.expname
                          }).ToList();
            var EDQuery = (from D in cashHeadData
                           join E in cashAmounts
                           on D.rno equals E.rno into amount
                           from data in amount.DefaultIfEmpty()
                           select new
                           {
                               rno = D.rno,
                               deptid = D.deptid,
                               currency = data?.currency ?? String.Empty,
                               actamt = data?.actamt ?? null,
                               payment = D.payment,
                               company = D.company
                           }).ToList();
            // 合并数据
            var resultQuery = (from cashHead in cashHeadData
                               join approvalFlow in approvalFlowdata
                               on cashHead.rno equals approvalFlow.rno into approvalGroup
                               from approval in approvalGroup.DefaultIfEmpty()
                               join F in FQuery
                               on cashHead.rno equals F.rno into FGroup
                               from FData in FGroup.DefaultIfEmpty()
                               join bdForm in bdFormQuery
                               on cashHead.formcode equals bdForm.FormCode into bdFormGroup
                               from bdFormData in bdFormGroup.DefaultIfEmpty()
                               join cashAmount in EDQuery
                               on cashHead.rno equals cashAmount.rno into cashamountGroup
                               from cashAmount in cashamountGroup.DefaultIfEmpty()

                               select new ReportDto
                               {
                                   rno = cashHead.rno,
                                   formcode = cashHead.formcode,
                                   formname = bdFormData?.FormName,
                                   deptid = cashHead.deptid,
                                   cemplid = cashHead.cuser,
                                   cname = cashHead.cname,
                                   company = cashHead.company,
                                   cdate = cashHead.cdate,
                                   status = cashHead.status,
                                   expdeptid = FData?.expdeptid,
                                   expname = FData?.expname,
                                   step = approval?.step,
                                   stepname = approval?.stepname,
                                   payment = cashHead.payment,
                                   currency = (cashHead.formcode == "CASH_3A" || cashHead.formcode == "CASH_UBER")
                                ? cashHead.currency
                                : cashAmount.currency,
                                   actamt = (cashHead.formcode == "CASH_3A" || cashHead.formcode == "CASH_UBER")
                                ? cashHead.actamt
                                : cashAmount.actamt,
                                   apid = (cashHead.status == "temporary" || cashHead.status == "canceled")
                                       ? bdFormData?.ApplicationMenuKey
                                       : (cashHead.status == "rejected"
                                       ? bdFormData?.SignMenuKey
                                       : bdFormData?.SignMenuKey)
                               }).ToList();
            foreach (var item in resultQuery)
            {
                List<string> depts = item.expdeptid.Split(',').ToList();
                List<string> expnames = item.expname.Split(',').ToList();
                depts = depts.Distinct().ToList();
                expnames = expnames.Distinct().ToList();
                if (depts.Count > 3)
                {
                    depts = depts.Take(3).ToList();
                    item.expdeptid = String.Join(",", depts) + "...";
                }
                if (expnames.Count > 3)
                {
                    expnames = expnames.Take(3).ToList();
                    item.expname = String.Join(",", expnames) + "...";
                }
            }
                result.data = resultQuery;
            return result;
        }
        //已签核表单明细查询
        public async Task<Result<List<ReportDto>>> QuerySignedReportDetail(Request<QuerySignedReportDto> request)
        {
            Result<List<ReportDto>> result = new Result<List<ReportDto>>()
            {
                data = new List<ReportDto>()
            };

            // 校验分页参数
            int pageIndex = request.pageIndex;
            int pageSize = request.pageSize;
            if (pageIndex < 1 || pageSize < 0)
            {
                pageIndex = 1;
                pageSize = 10;
            }

            var approvalFlowdata = (await _ApprovalFlowRepository.GetQueryableAsync())
    .Where(w => w.status == "approved" && !w.isdeleted)
    .Select(w => new
    {
        rno = w.rno,
        aemplid = w.approveremplid,
        adate = w.cdate,
        status = w.status,
        step = w.step,
        stepname = w.stepname,
        approvedate = w.approvedate
    })
    .AsNoTracking()
     .WhereIf(request.data.sdate.HasValue, w => w.approvedate.Value.Date >= request.data.sdate.Value.Date)
     .WhereIf(request.data.edate.HasValue, w => w.approvedate.Value.Date <= request.data.edate.Value.Date)
     .WhereIf(!string.IsNullOrEmpty(request.data.aemplid), w => w.aemplid == request.data.aemplid)
     .WhereIf(!string.IsNullOrEmpty(request.data.rno), w => w.rno == request.data.rno)
     .GroupBy(w => w.rno) // 按 rno 分组
     .Select(g => g.OrderByDescending(x => x.approvedate).FirstOrDefault())
     .ToList();

            if (approvalFlowdata.Count == 0)
            {
                return result;
            }

            var approvalFlowRnoList = approvalFlowdata.Select(x => x.rno).ToHashSet();

            // 使用表连接查询数据
            var query = from detail in (await _cashdetailRepository.WithDetailsAsync())
                        where !detail.isdeleted
                        join head in (await _cashheadRepository.WithDetailsAsync())
                            on detail.rno equals head.rno into headGroup
                        from head in headGroup.DefaultIfEmpty()
                        where !head.isdeleted
                        join form in (await _bdformRepository.WithDetailsAsync())
                            on detail.formcode equals form.FormCode into formGroup
                        from form in formGroup.DefaultIfEmpty()
                        where !form.isdeleted
                        join cashAmount in (await _cashamountRepository.WithDetailsAsync())
                            on head.rno equals cashAmount.rno into cashAmountGroup
                        from cashAmount in cashAmountGroup
                            .Where(cashAmount => !cashAmount.isdeleted)
                        where (
                            (!request.data.sdate.HasValue || (detail.cdate != null && detail.cdate.Value.Date >= request.data.sdate.GetValueOrDefault().Date))
                            && (!request.data.edate.HasValue || (detail.cdate != null && detail.cdate.Value.Date <= request.data.edate.GetValueOrDefault().Date))
                            && (string.IsNullOrEmpty(request.data.rno) || detail.rno == request.data.rno)
                            && (string.IsNullOrEmpty(request.data.cuser) || (!string.IsNullOrEmpty(detail.cuser) && detail.cuser.ToUpper() == request.data.cuser.ToUpper()))
                            && (request.data.formcode == null || request.data.formcode.Count == 0 || request.data.formcode.Contains(detail.formcode))
                            && (request.data.company == null || request.data.company.Count == 0 || request.data.company.Contains(detail.company))
                            && (request.data.status == null || request.data.status.Count == 0 || request.data.status.Contains(head.status))
                            && approvalFlowRnoList.Contains(detail.rno) 
                            )

                        select new
                        {
                            detail.rno,
                            detail.formcode,
                            detail.cuser,
                            cdate = detail != null ? detail.cdate : null,
                            detail.company,
                            hcurrency = detail.currency,
                            hactualamount = detail.amount,
                            acurrency = cashAmount != null ? cashAmount.currency : "",
                            aactamt = cashAmount != null ? cashAmount.actamt : 0,
                            detail.rdate,
                            detail.deptid,
                            detail.expname,
                            detail.summary,
                            detail.seq,
                            head.status,
                            head.payment,
                            head.paymentdate,
                            head.projectcode,
                            head.payeeId,
                            head.cname,
                            applyDept = head.deptid,
                            head.payeename,
                            head.expensetype,
                            ApplicationMenuKey = form != null ? form.ApplicationMenuKey : null,
                            SignMenuKey = form != null ? form.SignMenuKey : null,
                            FormName = form != null ? form.FormName : null
                        };

            result.total = await query.CountAsync();

            if (result.total == 0)
            {
                return result;
            }

            // 分页查询
            var pagedQuery = await query
                .OrderByDescending(x => x.rno)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var rnoList = pagedQuery.Select(q => q.rno).Distinct().ToList();
            var approvalflowData = (await _approvalflowRepository.WithDetailsAsync())
                .AsQueryable()
                .Where(w => rnoList.Contains(w.rno) && !w.isdeleted)
                .Select(w => new { w.rno, w.step, w.stepname })
                .ToList();
            // 创建 rno -> step 和 stepname 的字典
            var approvalFlowDict = approvalflowData
                .GroupBy(w => w.rno)
                .ToDictionary(
                    g => g.Key,
                    g => g.FirstOrDefault() // 取分组的第一个值
                );

            var seqList = pagedQuery.Select(q => q.seq).Distinct().ToList();

            // 获取 Invoice 数据
            var invoiceData = (await _invoiceRepository.WithDetailsAsync())
                 .Where(w => rnoList.Contains(w.rno) && !w.isdeleted)
                .GroupBy(w => new { w.rno, w.seq })
                .Select(g => new
                {
                    rno = g.Key.rno,
                    seq = g.Key.seq,
                    invcode = g.Count() > 1 ? $"{g.First().invcode},..." : g.First().invcode,
                    invno = g.Count() > 1 ? $"{g.First().invno},..." : g.First().invno,
                    taxamount = g.Count() > 1 ? $"{g.First().taxamount},..." : g.First().taxamount.ToString(),
                    oamount = g.Count() > 1 ? $"{g.First().oamount},..." : g.First().oamount.ToString(),
                })
                .ToDictionary(w => new { w.rno, w.seq });

            result.data = pagedQuery.Select(x =>
            {
                var invoice = invoiceData.GetValueOrDefault(new { rno = x.rno, seq = x.seq });
                var approvalFlow = approvalFlowDict.GetValueOrDefault(x.rno);
                string expdeptid = ProcessDeptId(x.deptid);
                return new ReportDto
                {
                    rno = x.rno,
                    formcode = x.formcode,
                    formname = x.FormName,
                    cname = x.cname,
                    deptid = x.applyDept,
                    company = x.company,
                    status = x.status,
                    cdate = x.cdate,
                    cemplid = x.cuser,
                    step = approvalFlow?.step ?? 0, // 添加空值检查
                    stepname = approvalFlow?.stepname,
                    payment = x.payment,
                    expdeptid = expdeptid,
                    expname = x.expname,
                    currency = (x.formcode == "CASH_3A" || x.formcode == "CASH_UBER")
                                ? x.hcurrency
                                : x.acurrency,
                    actamt = (x.formcode == "CASH_3A" || x.formcode == "CASH_UBER")
                                ? x.hactualamount
                                : x.aactamt,
                    rdate = x.rdate,
                    expensetype = x.expensetype,
                    projectcode = x.projectcode,
                    invcode = invoice?.invcode ?? "",
                    invno = invoice?.invno ?? "",
                    taxamountDisplay = invoice?.taxamount ?? "",
                    untaxamountDisplay = invoice?.oamount ?? "",
                    apid = (x.status == "temporary" || x.status == "canceled")
                        ? x.ApplicationMenuKey
                        : (x.status == "rejected"
                        ? x.SignMenuKey
                        : x.SignMenuKey),
                    payeeId = x.payeeId,
                    payeename = x.payeename,
                    summary = x.summary
                };
            }).ToList();

            return result;
        }
        //下载已签核表单
        public async Task<XSSFWorkbook> DownloadSignedReport(Request<QuerySignedReportDto> request)
        {
            var cashHeadCount = (await _cashheadRepository.WithDetailsAsync()).AsNoTracking().Count();
            // Request<ReportQueryDto> downloadRequest = new()
            // {
            //     pageIndex = 1,
            //     pageSize = eFormHeadCount,
            //     data = new()
            //     {
            //         category = new(),
            //         company = new(),
            //         formcode = new(),
            //         status = new()
            //     }
            // };
            // request.pageIndex = 1;
            request.pageSize = cashHeadCount;
            Result<List<ReportDto>> result = await QuerySignedReport(request);
            var invoices = (await _cashfileRepository.WithDetailsAsync()).Where(w => w.status != "F").ToList();
            var attachments = (await _cashfileRepository.WithDetailsAsync()).Where(w => w.status == "F").ToList();
            var abnormalQuery = (await _invoiceRepository.WithDetailsAsync()).Where(s => result.data.Select(w => w.rno).Contains(s.rno)).AsNoTracking().ToList();
            foreach (var item in result.data)
            {
                item.invoices = invoices.Where(w => w.rno == item.rno).Select(s => s.filename).ToList();
                item.attachments = attachments.Where(w => w.rno == item.rno).Select(s => s.filename).ToList();
                item.isAbnormal = abnormalQuery.Any(w => w.abnormal == "Y" && w.rno == item.rno) ? "Y" : "N";
                item.abnormalReson = abnormalQuery.Where(s => s.rno == item.rno).Select(w => w.abnormalmsg).ToList();
            }
            XSSFWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("sheet");
            if (result != null && result.data.Count() > 0)
            {
                // ISheet sheet = workbook.CreateSheet("sheet");
                for (int i = 0; i < result.data.Count; i++)
                {
                    IRow dataRow = sheet.CreateRow(i + 1);
                    dataRow.CreateCell(0).SetCellValue(result.data[i].company);
                    dataRow.CreateCell(1).SetCellValue(result.data[i].rno);
                    dataRow.CreateCell(2).SetCellValue(result.data[i].formname);
                    dataRow.CreateCell(3).SetCellValue(result.data[i].deptid);
                    dataRow.CreateCell(4).SetCellValue(result.data[i].cemplid);
                    dataRow.CreateCell(5).SetCellValue(result.data[i].cname);
                    dataRow.CreateCell(6).SetCellValue(Convert.ToDateTime(result.data[i].cdate).ToString("yyyy/MM/dd"));
                    dataRow.CreateCell(7).SetCellValue(result.data[i].expname);
                    dataRow.CreateCell(8).SetCellValue(result.data[i].expdeptid);
                    dataRow.CreateCell(9).SetCellValue(result.data[i].currency);
                    dataRow.CreateCell(10).SetCellValue(result.data[i].actamt.ToString());
                    dataRow.CreateCell(11).SetCellValue(result.data[i].stepname);
                    dataRow.CreateCell(12).SetCellValue((DateTime)result.data[i].payment);
                    string tInvoices = string.Join(";", result.data[i].invoices);
                    dataRow.CreateCell(13).SetCellValue(tInvoices);
                    string tAttachments = string.Join(";", result.data[i].attachments);
                    dataRow.CreateCell(14).SetCellValue(tAttachments);
                    dataRow.CreateCell(15).SetCellValue(result.data[i].isAbnormal);
                    //string abnormalResons = string.Join(";", result.data[i].abnormalReson);
                    string abnormalResons = string.Join(";", result.data[i].abnormalReson.Where(x => x != null));
                    dataRow.CreateCell(16).SetCellValue(result.data[i].isAbnormal == "Y" ? abnormalResons : string.Empty);
                }
            }
            else
            {
                IRow dataRow = sheet.CreateRow(0);
            }
            return workbook;
        }
        //下载已签核明细表单
        public async Task<XSSFWorkbook> DownloadSignedDetailReport(Request<QuerySignedReportDto> request)
        {
            var cashHeadCount = (await _cashheadRepository.WithDetailsAsync()).AsNoTracking().Count();

            request.pageSize = cashHeadCount;
            Result<List<ReportDto>> result = await QuerySignedReportDetail(request);
            //try
            //{
                XSSFWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("sheet");
            if (result != null && result.data.Count() > 0)
            {
                // ISheet sheet = workbook.CreateSheet("sheet");
                for (int i = 0; i < result.data.Count; i++)
                {
                    IRow dataRow = sheet.CreateRow(i + 1);
                    dataRow.CreateCell(0).SetCellValue(result.data[i].company);
                    dataRow.CreateCell(1).SetCellValue(result.data[i].rno);
                    dataRow.CreateCell(2).SetCellValue(result.data[i].formcode);
                    dataRow.CreateCell(3).SetCellValue(result.data[i].deptid);
                    dataRow.CreateCell(4).SetCellValue(result.data[i].cemplid);
                    dataRow.CreateCell(5).SetCellValue(result.data[i].cname);
                    dataRow.CreateCell(6).SetCellValue(result.data[i].summary);
                    dataRow.CreateCell(7).SetCellValue(result.data[i].expname);
                    dataRow.CreateCell(8).SetCellValue(Convert.ToDateTime(result.data[i].rdate).ToString("yyyy/MM/dd"));
                    dataRow.CreateCell(9).SetCellValue(result.data[i].invno);
                    dataRow.CreateCell(10).SetCellValue(result.data[i].expdeptid);
                    dataRow.CreateCell(11).SetCellValue(result.data[i].currency);
                    dataRow.CreateCell(12).SetCellValue(result.data[i].actamt.ToString());
                    dataRow.CreateCell(13).SetCellValue(result.data[i].untaxamountDisplay);
                    dataRow.CreateCell(14).SetCellValue(result.data[i].taxamountDisplay);
                    dataRow.CreateCell(15).SetCellValue(result.data[i].invcode);
                    dataRow.CreateCell(16).SetCellValue(Convert.ToDateTime(result.data[i].cdate).ToString("yyyy/MM/dd"));
                    dataRow.CreateCell(17).SetCellValue(result.data[i].formname);
                    dataRow.CreateCell(18).SetCellValue(result.data[i].projectcode);
                    dataRow.CreateCell(19).SetCellValue(result.data[i].payeeId);
                    dataRow.CreateCell(20).SetCellValue(result.data[i].payeename);
                    dataRow.CreateCell(21).SetCellValue(result.data[i].stepname);
                    dataRow.CreateCell(22).SetCellValue(result.data[i].apid);
                    dataRow.CreateCell(23).SetCellValue(Convert.ToDateTime(result.data[i].payment).ToString("yyyy/MM/dd"));
                    }
            }
            else
            {
                IRow dataRow = sheet.CreateRow(0);
            }
            return workbook;
            }
            //catch (Exception ex)
            //{
            //    // 记录异常信息
            //    Console.WriteLine($"Error in DownloadSignedDetailReport: {ex.Message}");
            //    throw; // 或根据需要处理异常
            //}

        //}



        // 条件查询纸本单据（分页）
        public async Task<Result<List<ReportDto>>> QueryPaperReport(Request<ReportQueryDto> request)
        {
            Result<List<ReportDto>> result = new Result<List<ReportDto>>()
            {
                data = new List<ReportDto>()
            };
            var efpaperList = (await _eformpaperRepository.WithDetailsAsync())
            .Select(w => new
            {
                rno = w.rno,
                status = w.status,
                formcode = w.formcode,
                company = w.company,
                cdate = w.cdate,
                cuser = w.cuser,
                emplid = w.emplid//纸本单签核人
            })
            .WhereIf(!string.IsNullOrEmpty(request.data.rno), q => q.rno == request.data.rno)
            .WhereIf(!request.data.status.IsNullOrEmpty(), q => request.data.status.Contains(q.status))
            .WhereIf(!request.data.category.IsNullOrEmpty() && request.data.category.Count > 1, q => request.data.category.Contains(request.data.rno.Substring(0, 1)))
            .WhereIf(!request.data.formcode.IsNullOrEmpty(), q => request.data.formcode.Contains(q.formcode))
            .WhereIf(!request.data.company.IsNullOrEmpty(), q => request.data.company.Contains(q.company))
            .WhereIf(request.data.startdate.HasValue, q => q.cdate.Value.Date >= Convert.ToDateTime(request.data.startdate).ToLocalTime().Date)
            .WhereIf(request.data.enddate.HasValue, q => q.cdate.Value.Date <= Convert.ToDateTime(request.data.enddate).ToLocalTime().Date)
            .WhereIf(!string.IsNullOrEmpty(request.data.cemplid), q => q.cuser.ToUpper() == request.data.cemplid.ToUpper())
            .AsNoTracking()
            .ToList();
            if (efpaperList.Count == 0)
            {
                return result;
            }
            result.total = efpaperList.Count;
            if (request.pageIndex <= 0 || request.pageSize <= 0)
            {
                request.pageIndex = 1;
                request.pageSize = 10;
            }
            efpaperList = efpaperList.Skip((request.pageIndex - 1) * request.pageSize).Take(request.pageSize).ToList();
            var efheadList = (await _eformheadRepository.WithDetailsAsync())
            .Where(g => efpaperList.Select(w => w.rno).Contains(g.rno))
            .Select(w => new
            {
                rno = w.rno,
                cname = w.cname,
                step = w.step,
                cdate = w.cdate,
                company = w.company
            })
            .AsNoTracking()
            .OrderByDescending(i => i.cdate)
            .ToList();
            //費用歸屬部門可能會出現問題，測試單號：C23021700002 qas
            var cashDetailQuery = (await _cashdetailRepository.WithDetailsAsync())
            .Where(w => efpaperList.Select(g => g.rno).Contains(w.rno))
            .Select(s => new
            {
                rno = s.rno,
                deptid = s.deptid,
                expname = s.expname
            }).Distinct().AsNoTracking().ToList();
            var expDepts = cashDetailQuery.Select(w => new
            {
                rno = w.rno,
                deptid = (w.deptid != null && w.deptid.StartsWith("[") && w.deptid.EndsWith("]")) ? string.Join(",", (JsonConvert.DeserializeObject<List<departCost>>(w.deptid)).Select(x => x.deptId).Distinct()) : w.deptid
            }).GroupBy(s => s.rno)
            .Select(g => new
            {
                rno = g.Key,
                expdeptid = string.Join(",", g.Select(i => i.deptid).Distinct())
            }).ToList();
            var expNameQuery = cashDetailQuery.Select(w => new
            {
                expname = w.expname,
                rno = w.rno
            })
            .GroupBy(s => s.rno)
            .Select(g => new
            {
                rno = g.Key,
                expname = string.Join(",", g.Select(i => i.expname).Distinct())
            }).ToList();
            var FQuery = (from b in expDepts
                          join c in expNameQuery
                          on b.rno equals c.rno
                          select new
                          {
                              rno = b.rno,
                              expdeptid = b.expdeptid,
                              expname = c.expname
                          }).ToList();
            // var cashdetailList = (await _cashdetailRepository.WithDetailsAsync())
            // .Where(w => efpaperList.Select(g => g.rno).Contains(w.rno))
            // .ToList()
            // .Select(s => new
            // {
            //     rno = s.rno,
            //     expdeptid = (s.deptid != null && s.deptid.StartsWith("[") && s.deptid.EndsWith("]")) ? string.Join(",", (JsonConvert.DeserializeObject<List<departCost>>(s.deptid)).Select(x => x.deptId).Distinct()) : s.deptid,
            //     expname = string.Join(",", s.expname)
            // })
            // .GroupBy(g => g.rno)
            // .Select(i => new {
            //     rno = i.Key,
            //     expdeptid = String.Join("," , i.Select(a => a.expdeptid).Distinct()),
            //     expname = i.FirstOrDefault().expname
            // })
            // .ToList();
            var cashheadList = (await _cashheadRepository.WithDetailsAsync())
            .Where(w => efpaperList.Select(s => s.rno).Contains(w.rno))
            .Select(g => new
            {
                rno = g.rno,
                deptid = g.deptid,
                cdate = g.cdate
            })
            .AsNoTracking()
            .ToList();
            var cashamountList = (await _cashamountRepository.WithDetailsAsync())
            .Where(w => efpaperList.Select(g => g.rno).Contains(w.rno))
            .Select(s => new
            {
                rno = s.rno,
                currency = s.currency,
                actamt = s.actamt
            }).ToList();
            List<CashFile> filequery = new List<CashFile>();
            List<CashFileDto> filesData = new List<CashFileDto>();
            List<CashFileDto> invlist = new List<CashFileDto>();
            //查询发票清单
            foreach (var item in efheadList)
            {
                filequery = (await _cashfileRepository.GetByNo(item.rno)).Where(f => f.ishead == "N").ToList();
                filesData = _objectMapper.Map<List<CashFile>, List<CashFileDto>>(filequery);
                foreach (var file in filesData)
                {
                    //todo 查询单据 要根据创建人来看文件去哪里取
                    string area = "";
                    file.url = !String.IsNullOrEmpty(file.path) ? await _MinioRepository.PresignedGetObjectAsync(file.path, area) : String.Empty;
                }
                invlist.AddRange(filesData);
            }
            var eformList = (await _bdformRepository.WithDetailsAsync()).Where(w => efpaperList.Select(s => s.formcode).Contains(w.FormCode)).ToList();
            List<ReportDto> resultQuery = (from efp in efpaperList
                                           select new ReportDto
                                           {
                                               rno = efp.rno,
                                               formcode = efp.formcode,
                                               cdate = efp.cdate,
                                               cemplid = efp.cuser,
                                               company = efp.company,
                                               status = efp.status,
                                               expdeptid = FQuery.FirstOrDefault(w => w.rno == efp.rno)?.expdeptid,
                                               expname = FQuery.FirstOrDefault(w => w.rno == efp.rno)?.expname,
                                               deptid = cashheadList.FirstOrDefault(w => w.rno == efp.rno)?.deptid,
                                               cname = efheadList.FirstOrDefault(w => w.rno == efp.rno)?.cname,
                                               step = efheadList.FirstOrDefault(w => w.rno == efp.rno)?.step,
                                               //apid = efheadList.FirstOrDefault(w => w.rno == efp.rno)?.apid,
                                               currency = cashamountList.FirstOrDefault(w => w.rno == efp.rno)?.currency,
                                               actamt = cashamountList.FirstOrDefault(w => w.rno == efp.rno)?.actamt,
                                               invlist = invlist.Where(w => w.rno == efp.rno).OrderBy(i => i.item).ToList(),
                                               formname = eformList.FirstOrDefault(w => w.FormCode == efp.formcode)?.FormName,
                                               stepname = L["PaperReport-Stepname"] + "/" + efp.emplid
                                           })
                                           .OrderByDescending(i => i.cdate)
                                           .ToList();
            foreach (var item in resultQuery)
            {
                List<string> depts = item.expdeptid.Split(',').Distinct().ToList();
                List<string> expnames = item.expname.Split(',').Distinct().ToList();
                if (depts.Count > 3)
                {
                    depts = depts.Take(3).ToList();
                    item.expdeptid = String.Join(",", depts) + "...";
                }
                if (expnames.Count > 3)
                {
                    expnames = expnames.Take(3).ToList();
                    item.expname = String.Join(",", expnames) + "...";
                }
            }
            result.data = resultQuery;
            return result;
        }
        // 纸本单据查询数据下载
        public async Task<XSSFWorkbook> DownloadPaper(Request<ReportQueryDto> request)
        {
            var efpaperCount = (await _eformpaperRepository.WithDetailsAsync()).AsNoTracking().Count();
            request.pageIndex = 1;
            request.pageSize = efpaperCount;
            Result<List<ReportDto>> result = await QueryPaperReport(request);
            XSSFWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("sheet");
            if (result != null && result.data.Count() > 0)
            {
                for (int i = 0; i < result.data.Count; i++)
                {
                    IRow dataRow = sheet.CreateRow(i + 1);
                    dataRow.CreateCell(0).SetCellValue(result.data[i].company);
                    dataRow.CreateCell(1).SetCellValue(result.data[i].rno);
                    dataRow.CreateCell(2).SetCellValue(result.data[i].formname);
                    dataRow.CreateCell(3).SetCellValue(result.data[i].deptid);
                    dataRow.CreateCell(4).SetCellValue(result.data[i].cemplid);
                    dataRow.CreateCell(5).SetCellValue(result.data[i].cname);
                    dataRow.CreateCell(6).SetCellValue(Convert.ToDateTime(result.data[i].cdate).ToString("yyyy/MM/dd"));
                    dataRow.CreateCell(7).SetCellValue(result.data[i].expname);
                    dataRow.CreateCell(8).SetCellValue(result.data[i].expdeptid);
                    dataRow.CreateCell(9).SetCellValue(result.data[i].currency);
                    dataRow.CreateCell(10).SetCellValue(result.data[i].actamt.ToString());
                    dataRow.CreateCell(11).SetCellValue(result.data[i].stepname);
                }
            }
            else
            {
                IRow dataRow = sheet.CreateRow(0);
            }
            return workbook;
        }
        // 预支冲账查询（分页）
        public async Task<Result<List<AdvOffsetDto>>> QueryAdvOffsetReport(Request<AdvOffsetQueryDto> request)
        {
            // 預支金單號、公司別、申請人姓名、申請人工號、收款人姓名、收款人工號
            var headQuery = (from ch in (await _cashheadRepository.WithDetailsAsync())
                             where ch.formcode == "CASH_3"
                             select new
                             {
                                 rno = ch.rno,
                                 company = ch.company,
                                 cname = ch.cname,
                                 cuser = ch.cuser,
                                 payname = ch.payeename,
                                 payeeid = ch.payeeId,
                                 cdate = ch.cdate
                             }).ToList();
            // 預支（報銷）場景、摘要
            var detailQuery = (from ch in headQuery
                               join cd in await _cashdetailRepository.WithDetailsAsync()
                               on ch.rno equals cd.rno
                               where cd.formcode == "CASH_3"
                               select new
                               {
                                   rno = ch.rno,
                                   company = ch.company,
                                   cname = ch.cname,
                                   cuser = ch.cuser,
                                   payname = ch.payname,
                                   payeeid = ch.payeeid,
                                   cdate = ch.cdate,
                                   expname = cd.expname,
                                   summary = cd.summary
                               }).ToList();
            //申請金額 已沖賬金額
            var amountQuery = (from dq in detailQuery
                               join aq in (await _cashamountRepository.WithDetailsAsync())
                               on dq.rno equals aq.rno
                               where aq.formcode == "CASH_3"
                               select new
                               {
                                   rno = dq.rno,
                                   company = dq.company,
                                   cname = dq.cname,
                                   cuser = dq.cuser,
                                   payname = dq.payname,
                                   payeeid = dq.payeeid,
                                   cdate = dq.cdate,
                                   expname = dq.expname,
                                   summary = dq.summary,
                                   actamt = aq.actamt,
                                   amount = aq.amount,
                                   reverseamt = aq.amount - aq.actamt
                               }).ToList();
            //已繳款金額、未沖賬金額
            var BDCashQuery = (from aq in amountQuery
                               join bcq in await _bdcashreturnRepository.WithDetailsAsync()
                               on aq.rno equals bcq.rno
                               select new
                               {
                                   rno = aq.rno,
                                   company = aq.company,
                                   cname = aq.cname,
                                   cuser = aq.cuser,
                                   //    payname = aq.payname,
                                   //    payeeid = aq.payeeid,
                                   payeename = aq.payname,
                                   payeeId = aq.payeeid,
                                   cdate = aq.cdate,
                                   expname = aq.expname,
                                   summary = aq.summary,
                                   amount = aq.amount,
                                   reverseamt = aq.reverseamt,
                                   payamt = bcq.amount,
                                   // 未沖賬金額(unreversamt)=申請金額(CashAmount actamt)-已沖賬金額(reverseamt)-已繳款金額(payamt BDCashReturn-amount)
                                   unreversamt = aq.actamt - bcq.amount
                               }).ToList();
            //沖賬ERS單號
            var ersrnoQuery = from bq in BDCashQuery
                              join ca in (await _chargeagainstRepository.WithDetailsAsync())
                              on bq.rno equals ca.rno into gj
                              from sub in gj.DefaultIfEmpty()
                              select new
                              {
                                  rno = bq.rno,
                                  company = bq.company,
                                  cname = bq.cname,
                                  cuser = bq.cuser,
                                  payeename = bq.payeename,
                                  payeeId = bq.payeeId,
                                  cdate = bq.cdate,
                                  expname = bq.expname,
                                  summary = bq.summary,
                                  amount = bq.amount,
                                  reverseamt = bq.reverseamt,
                                  payamt = bq.amount,
                                  unreversamt = bq.unreversamt,
                                  reverserno = sub?.rerno ?? String.Empty
                              };
            //申請延期日期
            var delaydateQuery = from eq in ersrnoQuery
                                 join eh in (await _eformheadRepository.WithDetailsAsync())
                                 on eq.rno equals eh.rno into se
                                 from sub in se.DefaultIfEmpty()
                                 select new AdvOffsetDto
                                 {
                                     rno = eq.rno,
                                     company = eq.company,
                                     cname = eq.cname,
                                     cuser = eq.cuser,
                                     payeename = eq.payeename,
                                     payeeId = eq.payeeId,
                                     cdate = eq.cdate,
                                     expname = eq.expname,
                                     summary = eq.summary,
                                     amount = eq.amount,
                                     reverseamt = eq.reverseamt,
                                     payamt = eq.amount,
                                     unreversamt = eq.unreversamt,
                                     reverserno = eq?.reverserno ?? String.Empty,
                                     applyextdate = sub?.cdate ?? null
                                 };
            List<AdvOffsetDto> selectDetails = new List<AdvOffsetDto>();
            if (request.data != null)
            {
                string rno = request.data.rno;
                string cuser = request.data.cuser;
                string status = request.data.status;
                List<string> companylist = request.data.company;
                selectDetails = delaydateQuery
                                .WhereIf(!string.IsNullOrEmpty(rno), q => q.rno == rno)
                                .WhereIf(!string.IsNullOrEmpty(cuser), q => q.cuser == cuser)
                                .WhereIf(status == "Y", q => q.unreversamt == 0)
                                .WhereIf(status == "N", q => q.unreversamt != 0)
                                .WhereIf(!companylist.IsNullOrEmpty(), q => companylist.Contains(q.company))
                                .ToList();
            }
            else
            {
                selectDetails = delaydateQuery.ToList();
            }
            int pageIndex = request.pageIndex;
            int pageSize = request.pageSize;
            if (pageIndex < 1 || pageSize < 0)
            {
                pageIndex = 1;
                pageSize = 10;
            }
            int count = selectDetails.Count();
            selectDetails = selectDetails.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            Result<List<AdvOffsetDto>> result = new Result<List<AdvOffsetDto>>()
            {
                data = selectDetails,
                total = count
            };
            return result;
        }
        public async Task<Result<ExcelDto<EntertainOverspendDetailDto>>> QueryOverspendDetail(GetOverSpendExcelDto input)
        {
            Result<ExcelDto<EntertainOverspendDetailDto>> result = new Result<ExcelDto<EntertainOverspendDetailDto>>();
            ExcelDto<EntertainOverspendDetailDto> excelData = new ExcelDto<EntertainOverspendDetailDto>();
            var unionquery = (from cd in (await _cashdetailRepository.WithDetailsAsync())
                              join ch in (await _cashheadRepository.WithDetailsAsync())
                              on cd.rno equals ch.rno
                              where cd.formcode == "CASH_2" && cd.processmethod == "1"
                              select new
                              {
                                  company = cd.company,
                                  rno = cd.rno,
                                  expdeptid = cd.deptid,
                                  overbudget = cd.overbudget,
                                  payeeId = ch.payeeId,
                                  payeename = ch.payeename
                              })
                              .Where(w => input.companyList.Contains(w.company))
                              .ToList();
            var cashPaylistQuery = (await _CashPaylistRepository.WithDetailsAsync())
                            .Where(w => unionquery.Select(w => w.rno)
                            .Contains(w.Rno))
                            .Select(w => new { w.Rno, w.DocNo, w.PostDate }).ToList();
            var picQuery = (await _approvalflowRepository.WithDetailsAsync())
                            .Where(w => unionquery
                            .Select(w => w.rno)
                            .Contains(w.rno) && w.step == 99)
                            .Select(w => new { w.rno, w.approvername }).ToList();
            DateTime sDate = new DateTime();
            DateTime eDate = new DateTime();
            if (input.startDate.HasValue)
            {
                sDate = Convert.ToDateTime(Convert.ToDateTime(input.startDate).ToString("yyyy/MM/dd"));
            }
            if (input.endDate.HasValue)
            {
                eDate = Convert.ToDateTime(Convert.ToDateTime(input.endDate).ToString("yyyy/MM/dd"));
            }
            var resultQuery = (from a in unionquery
                               join b in cashPaylistQuery
                               on a.rno equals b.Rno
                               join c in picQuery
                               on a.rno equals c.rno
                               select new EntertainOverspendDetailDto
                               {
                                   company = a.company,
                                   rno = a.rno,
                                   expdeptid = a.expdeptid,
                                   overbudget = a.overbudget,
                                   payeeId = a.payeeId,
                                   payeename = a.payeename,
                                   chargesite = a.company,
                                   docno = b.DocNo,
                                   postdate = Convert.ToDateTime(b.PostDate).ToString("yyyy/MM/dd"),
                                   FinancialPic = c.approvername
                               })
                              .WhereIf(input.startDate.HasValue, w => Convert.ToDateTime(w.postdate) >= sDate)
                              .WhereIf(input.endDate.HasValue, w => Convert.ToDateTime(w.postdate) <= eDate)
                              .ToList();
            string[] header = new string[]
            {
                L["CompanyCode"],
                L["Report-payeename"],
                L["Report-payeeId"],
                L["Report-rno"],
                L["Report-docno"],
                L["Report-postingDate"],
                L["Report-chargeSite"],
                L["Report-chargeDeptid"],
                L["Report-overBudget"],
                L["Report-FinancePIC"]
            };
            excelData.header = header;
            excelData.body = resultQuery;
            result.data = excelData;
            return result;
        }

        public async Task<Result<List<BDFormDto>>> QueryFormType()
        {
            Result<List<BDFormDto>> result = new Result<List<BDFormDto>>();

            var formList = (await _bdformRepository.WithDetailsAsync())
                .Select(x => new BDFormDto
                {
                    formcode = x.FormCode,
                    formname = x.FormName
                })
                .ToList();

            result.data = formList;
            return result;
        }
        private string ProcessDeptId(string deptid)
        {
            if (deptid == null) return string.Empty;

            // 判断是否为 JSON 格式
            if (deptid.StartsWith("[") && deptid.EndsWith("]"))
            {
                try
                {
                    var deptList = JsonConvert.DeserializeObject<List<departCost>>(deptid);
                    return string.Join(",", deptList.Select(d => d.deptId).Distinct());
                }
                catch (JsonSerializationException)
                {
                    // 解析失败，返回原始字符串
                    return deptid;
                }
            }

            // 如果不是 JSON 格式，直接返回原始值
            return deptid;
        }

    }

}
