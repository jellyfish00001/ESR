using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ERS.DTO;
using ERS.DTO.Print;
using ERS.Entities;
using ERS.IDomainServices;
using Volo.Abp.Domain.Repositories;
using System.IO;
using System.Text;
using ERS.DTO.Application;
using Volo.Abp.ObjectMapping;
using ERS.IRepositories;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
namespace ERS.DomainServices
{
    public class PrintDomainService : CommonDomainService, IPrintDomainService
    {
        private IRepository<EFormHead, Guid> _eformheadRepository;
        private IRepository<BDForm, Guid> _bdformRepository;
        private ICashDetailRepository _cashdetailRepository;
        private ICashHeadRepository _cashheadRepository;
        private ICashAmountRepository _cashamountRepository;
        private IInvoiceRepository _invoiceRepository;
        private IEFormSignlogRepository _EFormSignlogRepository;
        private IEmployeeRepository _EmployeeRepository;
        private IEFormAlistRepository _EFormAlistRepository;
        private IEFormAuserRepository _EFormAuserRepository;
        private IApprovalDomainService _ApprovalDomainService;
        private IObjectMapper _objectMapper;
        private IConfiguration _configuration;
        public PrintDomainService(
            IRepository<EFormHead, Guid> eformheadRepository,
            IRepository<BDForm, Guid> bdformRepository,
            ICashHeadRepository cashheadRepository,
            ICashDetailRepository cashdetailRepository,
            ICashAmountRepository cashamountRepository,
            IInvoiceRepository invoiceRepository,
            IEFormSignlogRepository EFormSignlogRepository,
            IEmployeeRepository EmployeeRepository,
            IEFormAlistRepository EFormAlistRepository,
            IEFormAuserRepository EFormAuserRepository,
            IApprovalDomainService ApprovalDomainService,
            IObjectMapper objectMapper,
            IConfiguration configuration
        )
        {
            _eformheadRepository = eformheadRepository;
            _bdformRepository = bdformRepository;
            _cashheadRepository = cashheadRepository;
            _cashdetailRepository = cashdetailRepository;
            _cashamountRepository = cashamountRepository;
            _invoiceRepository = invoiceRepository;
            _EFormSignlogRepository = EFormSignlogRepository;
            _EmployeeRepository = EmployeeRepository;
            _EFormAlistRepository = EFormAlistRepository;
            _EFormAuserRepository = EFormAuserRepository;
            _ApprovalDomainService = ApprovalDomainService;
            _objectMapper = objectMapper;
            _configuration = configuration;
        }
        // 列印查询（分页）
        public async Task<Result<List<PrintDto>>> QueryPagePrint(Request<PrintQueryDto> request)
        {
            Result<List<PrintDto>> result = new Result<List<PrintDto>>();
            var efheadquery = (from efh in (await _eformheadRepository.WithDetailsAsync())
                               select new
                               {
                                   rno = efh.rno,
                                   formcode = efh.formcode,
                                   cuser = efh.cuser,
                                   cdate = efh.cdate,
                                   cname = efh.cname,
                                   step = efh.step,
                                   status = efh.status,
                                   apid = efh.apid,
                                   nemplid = efh.nemplid,
                                   company = efh.company,
                                   stepname = (efh.status == "B" ? "Return" : (efh.status == "R" ? "Rejected" : (efh.status == "E" ? "EDIT" : (efh.status == "A" ? "Approved" : (efh.status == "T" ? "Temporary" : (efh.status == "C" ? "Cancel" : "Invite")))))),
                               })
                               .Where(w => w.formcode != "CASH_3A")
                               .WhereIf(request.data.startdate.HasValue, q => q.cdate.Value.Date >= Convert.ToDateTime(request.data.startdate).ToLocalTime().Date)
                               .WhereIf(request.data.enddate.HasValue, q => q.cdate.Value.Date <= Convert.ToDateTime(request.data.enddate).ToLocalTime().Date)
                               .WhereIf(!string.IsNullOrEmpty(request.data.rno), q => q.rno == request.data.rno)
                               .WhereIf(!string.IsNullOrEmpty(request.data.cuser), q => q.cuser == request.data.cuser)
                               .WhereIf(!request.data.status.IsNullOrEmpty(), q => request.data.status.Contains(q.status))
                               .WhereIf(!request.data.formcode.IsNullOrEmpty(), q => request.data.formcode.Contains(q.formcode))
                               .WhereIf(!request.data.company.IsNullOrEmpty(), q => request.data.company.Contains(q.company))
                               .ToList();
            if (!String.IsNullOrEmpty(request.data.aemplid))
            {
                var signlogs = (await _EFormSignlogRepository.WithDetailsAsync())
                .Where(w => efheadquery.Select(s => s.rno).ToList().Contains(w.rno) && w.aemplid == request.data.aemplid)
                .ToList();
                efheadquery = efheadquery.Where(w => signlogs.Select(s => s.rno).ToList().Contains(w.rno)).ToList();
            }
            if (efheadquery.Count > 0)
            {
                var eformalistQuery = (await _EFormAlistRepository.WithDetailsAsync()).Where(w => efheadquery.Select(w => w.rno).ToList().Contains(w.rno)).AsNoTracking().ToList();
                var eformauserQuery = (await _EFormAuserRepository.WithDetailsAsync()).Where(w => efheadquery.Select(w => w.rno).ToList().Contains(w.rno)).AsNoTracking().ToList();
                var eformquery = (await _bdformRepository.WithDetailsAsync()).Select(x => new { formcode = x.FormCode, formname = x.FormName }).ToList();// 取表单名
                var cashheadquery = (await _cashheadRepository.WithDetailsAsync())
                                    .Select(p => new
                                    {
                                        rno = p.rno,
                                        company = p.company,
                                        paymentdate = p.payment
                                    }).ToList();
                var resultquery = (from rq in efheadquery
                                   select new PrintDto
                                   {
                                       stepname = rq.stepname,
                                       rno = rq.rno,
                                       formcode = rq.formcode,
                                       cuser = rq.cuser,
                                       cdate = Convert.ToDateTime(rq.cdate?.ToString("yyyy/MM/dd HH:mm:ss")),
                                       cname = rq.cname,
                                       step = rq.step,
                                       status = rq.status,
                                       apid = rq.apid,
                                       formname = eformquery.Where(x => x.formcode == rq.formcode).Select(y => y.formname).FirstOrDefault(),
                                       company = cashheadquery.Where(x => x.rno == rq.rno).Select(y => y.company).FirstOrDefault(),
                                       paymentdate = cashheadquery.Where(x => x.rno == rq.rno).Select(y => y.paymentdate).FirstOrDefault()
                                   }).OrderByDescending(w => w.cdate).ToList();
                foreach (var item in resultquery)
                {
                    if (item.status == "P")
                    {
                        string cemplid = eformauserQuery.Where(w => w.rno == item.rno).Select(q => q.cemplid).FirstOrDefault();
                        string stepname = eformalistQuery.Where(w => w.rno == item.rno && w.step == item.step && w.cemplid == cemplid).FirstOrDefault()?.stepname;
                        item.stepname = String.IsNullOrEmpty(stepname) ? "Invite" : stepname;
                    }
                    if (item.status == "A")
                    {
                        item.payment = item.paymentdate == null ? L["WaitPayment"] : Convert.ToDateTime(item.paymentdate).ToString("yyyy/MM/dd");
                        item.stepname = item.paymentdate != null ? L["StepName-Finish"] : item.stepname;
                    }
                }
                int pageIndex = request.pageIndex;
                int pageSize = request.pageSize;
                if (pageIndex < 1 || pageSize < 0)
                {
                    pageIndex = 1;
                    pageSize = 10;
                }
                int count = resultquery.Count();
                resultquery = resultquery.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                result.data = resultquery;
                result.total = count;
            }
            return result;
        }
        /// <summary>
        /// 一般交际费列印(html)
        /// </summary>
        /// <param name="template">模板文件</param>
        /// <param name="path">生成的文件目录</param>
        /// <param name="dic">字典</param>
        /// <returns></returns>
        public async Task<string> EntertainmentExpPrintAsync(string rno, string token)
        {
            string result = "";
            //读取模板
            string templatepath = Path.Combine(Path.Combine("Files", "Print", "PrintTemplates", "PrintTemplate.html"));
            Encoding encode = Encoding.UTF8;
            StringBuilder html = new StringBuilder();
            //讀取模板轉成字符串 readhtml（模板字符串）
            string readhtml = File.ReadAllText(templatepath, encode);
            //截取body標籤內容
            //存放替换后的主体内容
            string htmlresult = "";
            StringBuilder htmlcontent = new StringBuilder();
            string bodyhtml = readhtml.Substring(readhtml.IndexOf("<body>") + "<body>".Length, readhtml.IndexOf("</body>") - (readhtml.IndexOf("<body>") + "<body>".Length));
            htmlcontent.Clear();
            htmlcontent.Append(bodyhtml);
            //信息查詢
            CashHead chquery = await _cashheadRepository.GetCashHeadByNo(rno);
            CashHeadDto chdata = _objectMapper.Map<CashHead, CashHeadDto>(chquery);
            List<CashDetail> cdquery = (await _cashdetailRepository.WithDetailsAsync()).Where(w => w.rno == rno).OrderBy(w => w.seq).ToList();
            List<CashDetailDto> cddata = _objectMapper.Map<List<CashDetail>, List<CashDetailDto>>(cdquery);
            CashAmount amquery = await _cashamountRepository.GetCashAmountByNo(rno);
            CashAmountDto amdata = _objectMapper.Map<CashAmount, CashAmountDto>(amquery);
            //获取签核数据
            var signlogs = await _ApprovalDomainService.GetSignLogs(rno, token);
            string phone = (await _EmployeeRepository.WithDetailsAsync()).Where(w => w.cname == chdata.cname).Select(w => w.phone).FirstOrDefault();
            if (chquery != null && cdquery != null && amquery != null)
            {
                //单据标题填充
                string documentheader = L["EntertainmentExpense"];
                string header = "";
                header += @$"<tr>
                                <td>{L["CompanyCode"]}：" + chdata.company + @"</td>" +
                                @$"<td>{L["ExpenseClassNo"]}：" + chdata.dtype + @"</td>" +
                                @"<td>Project Code：" + chdata.projectcode + @"</td>" +
                                @$"<td>{L["RequestNo"]}：" + chdata.rno + @"</td>" +
                            @"</tr>";
                //一般交际费主体content = 费用表格 + 分隔线 + 费用汇总
                //一般交际费单据费用（表格、费用汇总）数据填充
                //费用表格表头
                string feeheader = @$"<table style='border-collapse: collapse;border-width:0px; border-color:#d3d3d3' align='center' cellspacing='0' border='1'>
                                <tbody>
                                    <tr>
                                        <td>{L["ExpenseDate"]}</td>
                                        <td>{L["currency"]}</td>
                                        <td>{L["expenseamount"]}</td>
                                        <td>{L["convertcurrency"]}</td>
                                        <td>{L["expensedept"]}</td>
                                        <td>{L["entertainobject"]}</td>
                                        <td>{L["companyescort"]}</td>
                                        <td>{L["remarks"]}</td>
                                        <td>{L["PersonalTaxLiability"]}</td>
                                        <td>{L["ActualExpenseAmount"]}</td>
                                    </tr>";
                //费用表格数据
                string feecontent = "";
                for (int i = 0; i < cddata.Count; i++)
                {
                    feecontent += @"<tr><td>" +
                    cddata[i].rdate?.ToString("yyyy/MM/dd") + @"</td>" +
                    @"<td>" + cddata[i].currency + " " + @"</td>" +
                    @"<td>" + String.Format("{0:N2}", cddata[i].amount1) + @"</td>" +
                    @"<td>" + String.Format("{0:N2}", cddata[i].baseamt) + @"</td>" +
                    @"<td>" + cddata[i].deptid + @"</td>" + @"<td>" + cddata[i].@object + @"</td>" +
                    @"<td>" + cddata[i].keep + @"</td>" + @"<td>" + cddata[i].remarks + @"</td>" +
                    @"<td>" + String.Format("{0:N2}", cddata[i].baseamt - cddata[i].amount) + @"</td>" +
                    @"<td>" + String.Format("{0:N2}", cddata[i].amount) +
                    @"</td></tr>";
                }
                string feeendtag = @"</tbody></table>";
                string feetable = feeheader + feecontent + feeendtag;
                //费用汇总
                string feesummary = "";
                string feesummary1 = "";
                feesummary += @$"<tr>
                                    <td>{L["ExpenseAmountCount"]}：" + amdata.currency + " " + String.Format("{0:N2}", amdata.amount) + @"</td>" + @"
                                </tr>" +
                                $@"<tr>
                                    <td>{L["ActualPayAmount"]}：" + amdata.currency + " " + String.Format("{0:N2}", amdata.actamt) + $@"</td>
                                    <td>{L["ActPayment"]}：" + amdata.currency + " " + String.Format("{0:N2}", amdata.actamt) +
                                    @$"<td>{L["PersonalTaxLiability"]}：" + amdata.currency + " " + String.Format("{0:N2}", (amdata.amount - amdata.actamt)) +
                                    @"</td>
                                </tr>";
                feesummary1 += @$"<tr>
                                    <td>{L["payeeid"]}：" + chdata.payeeId + @"</td>" +
                                    @$"<td>{L["payeename"]}：" + chdata.payeename + @"</td>" +
                                    @$"<td>{L["Bank"]}：" + chdata.bank + @"</td>" +
                                @"</tr>";
                string summary = feesummary + feesummary1;
                //汇总部分结束
                //一般交际费单据主体内容合并（表格、汇总部分）
                string doccontent = @"<table style='width:85%; border-collapse: collapse; border-style:hidden;' align='center' border='1' cellspacing='0' cellpadding='0'>
            <tr>
                <td colspan='22'>" + feetable + @"</td>
            </tr>
        </table>
        <table align='center' border='0'>
            <tr style='height: 10px;'></tr>
            <tr>
                <td style='display: flex; justify-content: center;'>
                    <div style='width: 90%; border-width:0px;border-top:1pt solid Black'></div>
                </td>
            </tr>
            <tr style='height: 10px;'></tr>
        </table>
        <table align='center' border='0' style='width:85%;'>
            <tr>
                <td colspan='22'>
                    <table cellspacing='0' cellpadding='0' align='center'>
                        <tbody>" + summary + @"</tbody>
                    </table>
                </td>
            </tr>
        </table>";
                //簽核步驟內容填充
                string signtable = "";
                string signheader = @$"<tr>
                                <td>#</td>
                                <td>{L["signstep"]}</td>
                                <td>{L["signdept"]}</td>
                                <td>{L["approvingofficer"]}</td>
                                <td>{L["signdate"]}</td>
                                <td>{L["signresult"]}</td>
                                <td>{L["signopinion"]}</td>
                            </tr>";
                string signcontent = "";
                if (signlogs.data != null)
                {
                    foreach (var item in signlogs.data.signLog)
                    {
                        signcontent += @$"<tr>
                                <td>{signlogs.data.signLog.IndexOf(item) + 1}</td>
                                <td>{item.astepname}</td>
                                <td>{item.deptid}</td>
                                <td>{item.aname}({item.aename})</td>
                                <td>{item.adate.ToString("yyyy/MM/dd hh:mm:ss")}</td>
                                <td>{item.aresult}</td>
                                <td>{item.aremark}</td>
                            </tr>";
                    }
                }
                // signcontent =
                //             @"<tr>
                //                 <td>1</td>
                //                 <td>會計初審(一)</td>
                //                 <td>MZF350</td>
                //                 <td>朱秀菱(MAGGIE ZHU)</td>
                //                 <td>2020/05/19 09:43:25</td>
                //                 <td>同意</td>
                //                 <td>&nbsp;</td>
                //             </tr>
                //             <tr>
                //                 <td>2</td>
                //                 <td>會計初審(一)</td>
                //                 <td>MZF350</td>
                //                 <td>朱秀菱(MAGGIE ZHU)</td>
                //                 <td>2020/05/19 09:43:25</td>
                //                 <td>同意</td>
                //                 <td>&nbsp;</td>
                //             </tr>";
                signtable = signheader + signcontent;
                //申请人、部门数据填充
                string applyuser = "";
                applyuser += $"{L["applicant"]}：" + chdata.deptid + " " + chdata.cname + " #" + phone;
                //頁碼
                //string pagination = (rnolist.IndexOf(rno) + 1).ToString() + "/" + rnolist.Count();
                //申请日期数据填充
                string applydate = "";
                applydate += "Request date:" + chdata.cdate;
                //替换字典
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("header", header);
                dic.Add("doccontent", doccontent);
                // dic.Add("feetable", feetable);//费用表格
                // dic.Add("feesummary", summary);//费用汇总
                dic.Add("applyuser", applyuser);
                dic.Add("applydate", applydate);
                //dic.Add("pagination", pagination);
                dic.Add("signtable", signtable);
                dic.Add("documentheader", documentheader);
                foreach (KeyValuePair<string, string> d in dic)
                {
                    //替换数据
                    htmlcontent.Replace(
                        string.Format("${0}$", d.Key),
                        d.Value);
                }
                // if(actualrnolist.IndexOf(rno) + 1 == actualrnolist.Count())//判斷是否為最後一張單據
                // {
                //     scontent.Replace("always","auto");
                // }
                string s = htmlcontent.ToString();
                s = s.Replace("\n", "");
                s = s.Replace("\r", "");
                s = Regex.Replace(s, @"""(.+?)""", m => "" + m.Groups[1].Value + "");
                html.Append(htmlcontent.ToString());
            }
            //html.Replace(bodyhtml,"");
            htmlresult = html.ToString();
            //对转义字符进行处理
            htmlresult = htmlresult.Replace("\n", "");
            htmlresult = htmlresult.Replace("\r", "");
            htmlresult = Regex.Replace(htmlresult, @"""(.+?)""", m => "" + m.Groups[1].Value + "");
            //去除body标签内所有内容
            result = readhtml.Replace(bodyhtml, "");
            result = result.Replace("<body>", "<body>" + htmlresult);
            result = result.Replace("\n", "");
            result = result.Replace("\r", "");
            result = Regex.Replace(result, @"""(.+?)""", m => "" + m.Groups[1].Value + "");
            return htmlresult;
        }
        /// <summary>
        /// 交际费（餐饮宴客）列印(html)
        /// </summary>
        /// <param name="rno">单号</param>
        /// <param name="token">用于获取签核历史的token</param>
        /// <returns></returns>
        public async Task<string> CateringGuestsPrintAsync(string rno, string token)
        {
            string result = "";
            //List<string> actualrnolist = new List<string>();//存放有數據的單號
            //读取模板
            string templatepath = Path.Combine(Path.Combine("Files", "Print", "PrintTemplates", "PrintTemplate.html"));
            Encoding encode = Encoding.UTF8;
            StringBuilder html = new StringBuilder();
            //讀取模板轉成字符串 readhtml（模板字符串）
            string readhtml = File.ReadAllText(templatepath, encode);
            //存放替换后的主体内容
            string htmlresult = "";
            StringBuilder htmlcontent = new StringBuilder();
            //截取body标签内容
            string bodyhtml = readhtml.Substring(readhtml.IndexOf("<body>") + "<body>".Length, readhtml.IndexOf("</body>") - (readhtml.IndexOf("<body>") + "<body>".Length));
            //遍历传入单号
            htmlcontent.Clear();
            htmlcontent.Append(bodyhtml);
            //查询数据
            CashHead chquery = await _cashheadRepository.GetCashHeadByNo(rno);
            CashDetail cdquery = await _cashdetailRepository.GetCashDetailByNo(rno);
            CashAmount amquery = await _cashamountRepository.GetCashAmountByNo(rno);
            //List<EFormSignlog> signlogs = await _EFormSignlogRepository.GetListByRno(rno);
            //获取签核数据
            var signlogs = await _ApprovalDomainService.GetSignLogs(rno, token);
            if (chquery != null && cdquery != null && amquery != null)
            {
                CashHeadDto chdata = _objectMapper.Map<CashHead, CashHeadDto>(chquery);
                CashDetailDto cddata = _objectMapper.Map<CashDetail, CashDetailDto>(cdquery);
                CashAmountDto amdata = _objectMapper.Map<CashAmount, CashAmountDto>(amquery);
                string phone = (await _EmployeeRepository.WithDetailsAsync()).Where(w => w.cname == chdata.cname).Select(w => w.phone).FirstOrDefault();
                //单据标题
                string documentheader = L["CateringAndBanqueting"];
                //放入有数据的单号
                // actualrnolist.Add(rno);
                //单据头部信息填充
                string header = "";
                header += @$"<tr>
                                <td>{L["CompanyCode"]}：" + chdata.company + @"</td>" +
                                @$"<td>{L["ExpenseClassNo"]}：" + chdata.dtype + @"</td>" +
                                @"<td>Project Code：" + chdata.projectcode + @"</td>" +
                                @$"<td>{L["RequestNo"]}：" + chdata.rno + @"</td>" +
                            @"</tr>";
                //单据主体内容填充
                /*交际费（餐饮宴客）单据主体内容(doccontent) = 是否取得權責主管核准（approve） + 餐饮宴客时间段（timequantum） +
                宴客明細表格（entertaintable） + 客戶參加人員（clienttable） + 公司參加人員（companytable） + (是否符合我方人數規範 + 费用汇总表格)(feesummary)*/
                string doccontent = "";
                //是否取得權責主管核准
                string approve = "";
                if (chdata.whetherapprove == "Y")
                {
                    approve =
                    @$"<table style='width:80%; margin-inline-start: 10%; margin-top: 10x; margin-bottom: 50px;' align='center' border='0'>
                            <tr>
                                <td colspan='22'>
                                    <table align='center' cellspacing='0' cellpadding='0'>
                                        <tbody>
                                            <tr>
                                                <td>{L["WHETHERAPPROVE0"]}</td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </td>
                            </tr>
                        </table>";
                }
                else if (chdata.whetherapprove == "N")
                {
                    approve =
                    @$"<table style='width:80%; margin-inline-start: 10%; margin-top: 10x; margin-bottom: 50px;' align='center' border='0'>
                            <tr>
                                <td colspan='22'>
                                    <table align='center' cellspacing='0' cellpadding='0'>
                                        <tbody>
                                            <tr>
                                                <td>{L["WHETHERAPPROVE1"]}" + chdata.approvereason + @"</td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </td>
                            </tr>
                        </table>";
                }
                //餐饮宴客时间段
                string timequantum = "";
                timequantum = @$"<table style='width:80%; margin-inline-start: 10%; margin-bottom: 10px;' align='center' border='0'>
                                        <tr>
                                            <td colspan='22'>
                                                <table align='center' cellspacing='0' cellpadding='0'>
                                                    <tbody>
                                                        <tr>
                                                            <td>{L["treattime"]}" +
                                                        cddata.treattime +
                                                        @"</td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>";
                //宴客明細表格
                StringBuilder entertaintable = new StringBuilder();
                //宴客明細标题
                string entertaintitle = @$"<table  style='width:80%; margin-inline-start: 10%;' align='center' border='0'>
                                                <tr>
                                                    <td colspan='22'>
                                                        <table align='center' cellspacing='0' cellpadding='0'>
                                                            <tbody>
                                                                <tr>
                                                                    <td rowspan='5'>{L["banquetdetail"]}：</td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>";
                //宴客明细表头
                string entertainheader = @$"<table style='width:80%; margin-bottom: 15px; margin-inline-start: 10%; border-collapse: collapse; border-style:hidden;' align='center' border='1' cellspacing='0' cellpadding='0'>
                                                    <tr>
                                                        <td colspan='22'>
                                                            <table style='border-collapse: collapse;border-width:0px; border-color:#d3d3d3' align='center' cellspacing='0' border='1'>
                                                                <tbody>
                                                                    <tr>
                                                                        <td>{L["Banquetcustomer"]}</td>
                                                                        <td>{L["Banquetplace"]}</td>
                                                                        <td>{L["expdeptid"]}</td>
                                                                        <td>{L["Banquetdate"]}</td>
                                                                        <td>{L["currency"]}</td>
                                                                    </tr>";
                //宴客明细数据
                string entertaindata = "";
                entertaindata += $@"<tr>
                                    <td>{cddata.@object}</td>
                                    <td>{cddata.treataddress}</td>
                                    <td>{cddata.deptid}</td>
                                    <td>{Convert.ToDateTime(cddata.rdate).ToString("yyyy/MM/dd")}</td>
                                    <td>{cddata.currency}</td>
                                </tr>";
                //宴客明细表尾
                string entertaintag = @"</tbody></table></td></tr></table>";
                //宴客明细表格拼接
                entertaintable.Append(entertaintitle);
                entertaintable.Append(entertainheader);
                entertaintable.Append(entertaindata);
                entertaintable.Append(entertaintag);
                //客户参加人员表格
                StringBuilder clienttable = new StringBuilder();
                string clienttitle = @$"<table style='width:80%; margin-inline-start: 10%;' align='center' border='0'>
                                                <tr>
                                                    <td colspan='22'>
                                                        <table align='center' cellspacing='0' cellpadding='0'>
                                                            <tbody>
                                                                <tr>
                                                                    <td rowspan='5'>{L["Customerparticipants"]}:</td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>";
                string clientheader = @$"<table style='width:80%; margin-bottom: 15px; margin-inline-start: 10%; border-collapse: collapse; border-style:hidden;' align='center' border='1' cellspacing='0' cellpadding='0'>
                                                <tr>
                                                    <td colspan='22'>
                                                        <table style='border-collapse: collapse;border-width:0px; border-color:#d3d3d3' align='center' cellspacing='0' border='1'>
                                                            <tbody>
                                                                <tr>
                                                                    <td>{L["Name/titleOfCustomerTopSupervisor"]}</td>
                                                                    <td>{L["OtherCustomerMembers"]}</td>
                                                                    <td>{L["NumberOfCustomer"]}</td>
                                                                </tr>";
                string clientdata = "";
                clientdata += @$"<tr>
                                        <td>{cddata.custsuperme}</td>
                                        <td>{cddata.otherobject}</td>
                                        <td>{string.Format("{0:#}", cddata.objectsum)}</td>
                                     </tr>";
                string clienttag = @"</tbody></table></td></tr></table>";
                clienttable.Append(clienttitle);
                clienttable.Append(clientheader);
                clienttable.Append(clientdata);
                clienttable.Append(clienttag);
                StringBuilder companytable = new StringBuilder();
                string companytitle = @$"<table style='width:80%; margin-inline-start: 10%;' align='center' border='0'>
                                                <tr>
                                                    <td colspan='22'>
                                                        <table align='center' cellspacing='0' cellpadding='0'>
                                                            <tbody>
                                                                <tr>
                                                                    <td rowspan='5'>{L["Companyparticipants"]}:</td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>";
                string companyheader = @$"<table style='width:80%; margin-bottom: 15px; margin-inline-start: 10%; border-collapse: collapse; border-style:hidden;' align='center' border='1' cellspacing='0' cellpadding='0'>
                                                <tr>
                                                    <td colspan='22'>
                                                        <table style='border-collapse: collapse;border-width:0px; border-color:#d3d3d3' align='center' cellspacing='0' border='1'>
                                                            <tbody>
                                                                <tr>
                                                                    <td>{L["Name/titleOfCompanyTopSupervisor"]}</td>
                                                                    <td>{L["CategoryOfCompanyTopSupervisor"]}</td>
                                                                    <td>{L["othercompanymembers"]}</td>
                                                                    <td>{L["NumberOfCompany"]}</td>
                                                                </tr>";
                string companydata = "";
                string keepcname = await _EmployeeRepository.GetCnameByEmplid(cddata.keep);
                companydata += @$"<tr>
                                        <td>{cddata.keep}/{keepcname}</td>
                                        <td>{cddata.keepcategory}</td>
                                        <td>{cddata.otherkeep}</td>
                                        <td>{string.Format("{0:#}", cddata.keepsum)}</td>
                                     </tr>";
                string companytag = @"</tbody></table></td></tr></table>";
                companytable.Append(companytitle);
                companytable.Append(companyheader);
                companytable.Append(companydata);
                companytable.Append(companytag);
                string isaccordnumber = "";//是否符合人数规范
                if (cddata.isaccordnumber == "Y")
                {
                    isaccordnumber = @$"<table style='width:80%; margin-inline-start: 10%;' align='center' border='0'>
                                            <tr>
                                                <td colspan='22'>
                                                    <table align='center' cellspacing='0' cellpadding='0'>
                                                        <tbody>
                                                            <tr>
                                                                <td rowspan='5'>{L["ISACCORDNUMBER0"]}</td>
                                                            </tr>
                                                        </tbody>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>";
                }
                else if (cddata.isaccordnumber == "N")
                {
                    isaccordnumber = @$"<table style='width:80%; margin-inline-start: 10%;' align='center' border='0'>
                                            <tr>
                                                <td colspan='22'>
                                                    <table align='center' cellspacing='0' cellpadding='0'>
                                                        <tbody>
                                                            <tr>
                                                                <td rowspan='5'>{L["ISACCORDNUMBER1"]}" + $"{cddata.notaccordreason}" + @"</td>
                                                            </tr>
                                                        </tbody>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>";
                }
                string isaccordcost = "";//是否符合费用规范
                if (cddata.isaccordcost == "Y")
                {
                    isaccordcost = @$"<td>{L["ISACCORDCOST0"]}</td>";
                }
                else if (cddata.isaccordcost == "N")
                {
                    if (cddata.processmethod == "self")
                    {
                        isaccordcost = @$"<td>{L["ISACCORDCOST1"]}{cddata.overbudget}</td>";
                    }
                    else if (cddata.processmethod == "company")
                    {
                        isaccordcost = @$"<td>{L["ISACCORDCOST1"]}{cddata.overbudget}</td>";
                    }
                }
                string feesummary = isaccordnumber + @$"
        <table style='width:80%; margin-inline-start: 10%;' align='center' border='0'>
            <tr>
                <td colspan='22'>
                    <table cellspacing='0' cellpadding='0' align='center'>
                        <tbody>
                            <tr>
                                <td>{L["TotalBudgetbyQty"]}：{cddata.currency}{" "}{String.Format("{0:N2}", cddata.amount1)}</td>
                                <td>{L["ActualExpenditure"]}：{cddata.currency}{" "}{String.Format("{0:N2}", cddata.actualexpense)}</td>" +
                            isaccordcost +
                        @$"</tr>
                            <tr>
                                <td>{L["ActualPayAmount"]}：{amdata.currency}{" "}{String.Format("{0:N2}", cddata.amount)}</td>
                                <td>{L["PersonalTaxLiability"]}：{amdata.currency}{" "}{String.Format("{0:N2}", (amdata.amount - amdata.actamt))}</td>
                            </tr>
                            <tr>
                                <td>{L["payeeid"]}：{chdata.payeeId}</td>
                                <td>{L["payeename"]}：{chdata.payeename}</td>
                                <td>{L["Bank"]}：{chdata.bank}</td>
                            </tr>
                        </tbody>
                    </table>
                </td>
            </tr>
        </table>";
                doccontent = approve + timequantum + entertaintable.ToString() + clienttable.ToString() + companytable.ToString() + feesummary;
                //签核表格
                string signtable = "";
                string signheader = @$"<tr>
                                <td>#</td>
                                <td>{L["signstep"]}</td>
                                <td>{L["signdept"]}</td>
                                <td>{L["approvingofficer"]}</td>
                                <td>{L["signdate"]}</td>
                                <td>{L["signresult"]}</td>
                                <td>{L["signopinion"]}</td>
                            </tr>";
                string signcontent = "";
                if (signlogs.data != null)
                {
                    foreach (var item in signlogs.data?.signLog)
                    {
                        signcontent += @$"<tr>
                                <td>{signlogs.data.signLog.IndexOf(item) + 1}</td>
                                <td>{item.astepname}</td>
                                <td>{item.deptid}</td>
                                <td>{item.aname}({item.aename})</td>
                                <td>{item.adate.ToString("yyyy/MM/dd hh:mm:ss")}</td>
                                <td>{item.aresult}</td>
                                <td>{item.aremark}</td>
                            </tr>";
                    }
                }
                signtable = signheader + signcontent;
                //申请人、部门数据
                string applyuser = "";
                applyuser += $"{L["applicant"]}：" + chdata.deptid + " " + chdata.cname + " #" + phone;
                //申请日期数据填充
                string applydate = "";
                applydate += "Request date:" + chdata.cdate;
                //替换字典
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("documentheader", documentheader);
                dic.Add("header", header);
                dic.Add("doccontent", doccontent);
                dic.Add("signtable", signtable);
                dic.Add("applyuser", applyuser);
                dic.Add("applydate", applydate);
                foreach (KeyValuePair<string, string> d in dic)
                {
                    //替换数据
                    htmlcontent.Replace(
                        string.Format("${0}$", d.Key),
                        d.Value);
                }
                html.Append(htmlcontent.ToString());
            }
            htmlresult = html.ToString();
            //对转义字符进行处理
            htmlresult = htmlresult.Replace("\n", "");
            htmlresult = htmlresult.Replace("\r", "");
            htmlresult = Regex.Replace(htmlresult, @"""(.+?)""", m => "" + m.Groups[1].Value + "");
            //去除body标签内所有内容
            result = readhtml.Replace(bodyhtml, "");
            result = result.Replace("<body>", "<body>" + htmlresult);
            result = result.Replace("\n", "");
            result = result.Replace("\r", "");
            result = Regex.Replace(result, @"""(.+?)""", m => "" + m.Groups[1].Value + "");
            return htmlresult;
        }
        /// <summary>
        /// 一般费用报销列印(html)
        /// </summary>
        public async Task<string> GENCommExpPrintAsync(string rno, string token)
        {
            string result = "";
            string templatepath = Path.Combine(Path.Combine("Files", "Print", "PrintTemplates", "PrintTemplate.html"));
            Encoding encode = Encoding.UTF8;
            StringBuilder html = new StringBuilder();
            string readhtml = File.ReadAllText(templatepath, encode);
            string htmlresult = "";
            StringBuilder htmlcontent = new StringBuilder();
            string bodyhtml = readhtml.Substring(readhtml.IndexOf("<body>") + "<body>".Length, readhtml.IndexOf("</body>") - (readhtml.IndexOf("<body>") + "<body>".Length));
            htmlcontent.Clear();
            htmlcontent.Append(bodyhtml);
            //信息查詢
            CashHead chquery = await _cashheadRepository.GetCashHeadByNo(rno);
            List<CashDetail> cdquery = (await _cashdetailRepository.WithDetailsAsync()).OrderBy(w => w.seq).Where(w => w.rno == rno).ToList();
            CashAmount amquery = await _cashamountRepository.GetCashAmountByNo(rno);
            List<Invoice> invquery = (await _invoiceRepository.WithDetailsAsync()).Where(w => w.rno == rno).ToList();
            // List<EFormSignlog> signdata = await _EFormSignlogRepository.GetListByRno(rno);
            //获取签核数据
            var signlogs = await _ApprovalDomainService.GetSignLogs(rno, token);
            if (chquery != null && cdquery != null && amquery != null)
            {
                CashHeadDto chdata = _objectMapper.Map<CashHead, CashHeadDto>(chquery);
                List<CashDetailDto> cddata = _objectMapper.Map<List<CashDetail>, List<CashDetailDto>>(cdquery);
                CashAmountDto amdata = _objectMapper.Map<CashAmount, CashAmountDto>(amquery);
                string phone = (await _EmployeeRepository.WithDetailsAsync()).Where(w => w.cname == chdata.cname).Select(w => w.phone).FirstOrDefault();
                //单据标题填充
                string documentheader = L["GeneralExpenses"];
                //单据头部信息填充
                string header = "";
                header += @$"<tr>
                                <td>{L["CompanyCode"]}：" + chdata.company + @"</td>" +
                                @$"<td>{L["ExpenseClassNo"]}：" + chdata.dtype + @"</td>" +
                                @"<td>Project Code：" + chdata.projectcode + @"</td>" +
                                @$"<td>{L["RequestNo"]}：" + chdata.rno + @"</td>" +
                            @"</tr>";
                //费用表格数据
                StringBuilder feecontent = new StringBuilder();
                //费用表格表头
                string feeheader = "";
                decimal selfTaxSum = 0;//汇总个人承担税金
                for (int i = 0; i < cddata.Count; i++)
                {
                    decimal tempSelfTax = 0;
                    if (cddata[i].deptid.StartsWith("[") && cddata[i].deptid.EndsWith("]"))
                    {
                        feeheader = @$"<table style='border-collapse: collapse;border-width:0px; border-color:#d3d3d3' align='center' cellspacing='0' border='1'>
                                <tbody>
                                    <tr>
                                        <td>{L["ExpenseDate"]}</td>
                                        <td>{L["ReimbursementScene"]}</td>
                                        <td>{L["Summary"]}</td>
                                        <td>{L["expensedept"]}</td>
                                        <td>{L["Print-Percent"]}</td>
                                        <td>{L["currency"]}</td>
                                        <td>{L["Applyamount"]}</td>
                                        <td>{L["convertcurrency"]}</td>
                                        <td>{L["PersonalTaxLiability"]}</td>
                                        <td>{L["ActualPayAmount"]}</td>
                                    </tr>";
                        List<departCost> costData = JsonConvert.DeserializeObject<List<departCost>>(cddata[i].deptid);
                        feecontent.Append(@"<tr><td>");
                        feecontent.Append(cddata[i].rdate?.ToString("yyyy/MM/dd"));
                        feecontent.Append(@"</td>");
                        feecontent.Append(@"<td>");
                        feecontent.Append(cddata[i].expname);
                        feecontent.Append(@"</td>");
                        feecontent.Append(@"<td>");
                        feecontent.Append(cddata[i].summary);
                        feecontent.Append(@"</td>");
                        feecontent.Append(@"<td>");
                        foreach (var item in costData)
                        {
                            feecontent.Append(@"<span>");
                            feecontent.Append(item.deptId);
                            feecontent.Append(@"</span><br>");
                        }
                        feecontent.Append(@"</td>");
                        feecontent.Append(@"<td>");
                        foreach (var item in costData)
                        {
                            feecontent.Append(@"<span>");
                            feecontent.Append(item.percent);
                            feecontent.Append(@"</span><br>");
                        }
                        feecontent.Append(@"</td>");
                        feecontent.Append(@"<td>");
                        feecontent.Append(cddata[i].currency + " ");
                        feecontent.Append(@"</td>");
                        feecontent.Append(@"<td>");
                        foreach (var item in costData)
                        {
                            feecontent.Append(@"<span>");
                            feecontent.Append(String.Format("{0:N2}", item.amount));
                            feecontent.Append(@"</span><br>");
                        }
                        feecontent.Append(@"</td>");
                        feecontent.Append(@"<td>");
                        foreach (var item in costData)
                        {
                            feecontent.Append(@"<span>");
                            feecontent.Append(String.Format("{0:N2}", item.baseamount));
                            feecontent.Append(@"</span><br>");
                        }
                        feecontent.Append(@"</td>");
                        feecontent.Append(@"<td>");
                        //如果選擇預支金進行報銷 個人承擔稅金從加總invoice的taxloss(undertaker == "self")
                        if (!String.IsNullOrEmpty(cddata[i].advancerno))
                        {
                            tempSelfTax = invquery.Where(w => w.seq == cddata[i].seq && w.undertaker == "self").Sum(s => s.taxloss);
                            if (invquery.Where(w => w.seq == cddata[i].seq).Count() > 0)
                            {
                                feecontent.Append(String.Format("{0:N2}", tempSelfTax));
                                selfTaxSum += tempSelfTax;
                            }
                            else
                            {
                                feecontent.Append(String.Format("{0:N2}", 0));
                            }
                        }
                        else
                        {
                            feecontent.Append(String.Format("{0:N2}", cddata[i].baseamt - cddata[i].amount));
                            tempSelfTax = (cddata[i].baseamt - cddata[i].amount) != null ? Convert.ToDecimal((cddata[i].baseamt - cddata[i].amount)) : 0;
                            selfTaxSum += tempSelfTax;
                        }
                        feecontent.Append(@"</td>");
                        feecontent.Append(@"<td>");
                        feecontent.Append(String.Format("{0:N2}", cddata[i].amount));
                        feecontent.Append(@"</td></tr>");
                    }
                    else
                    {
                        feeheader = @$"<table style='border-collapse: collapse;border-width:0px; border-color:#d3d3d3' align='center' cellspacing='0' border='1'>
                                <tbody>
                                    <tr>
                                        <td>{L["ExpenseDate"]}</td>
                                        <td>{L["ReimbursementScene"]}</td>
                                        <td>{L["Summary"]}</td>
                                        <td>{L["expensedept"]}</td>
                                        <td>{L["currency"]}</td>
                                        <td>{L["Applyamount"]}</td>
                                        <td>{L["convertcurrency"]}</td>
                                    </tr>";
                        feecontent.Append(@"<tr><td>");
                        feecontent.Append(cddata[i].rdate?.ToString("yyyy/MM/dd"));
                        feecontent.Append(@"</td>");
                        feecontent.Append(@"<td>");
                        feecontent.Append(cddata[i].expname);
                        feecontent.Append(@"</td>");
                        feecontent.Append(@"<td>");
                        feecontent.Append(cddata[i].summary);
                        feecontent.Append(@"</td>");
                        feecontent.Append(@"<td>");
                        feecontent.Append(cddata[i].deptid);
                        feecontent.Append(@"</td>");
                        feecontent.Append(@"<td>");
                        feecontent.Append(cddata[i].currency + " ");
                        feecontent.Append(@"</td>");
                        feecontent.Append(@"<td>");
                        feecontent.Append(String.Format("{0:N2}", cddata[i].amount1));
                        feecontent.Append(@"</td>");
                        feecontent.Append(@"<td>");
                        feecontent.Append(String.Format("{0:N2}", cddata[i].baseamt));
                        feecontent.Append(@"<td>");
                        //如果選擇預支金進行報銷 個人承擔稅金從加總invoice的taxloss(undertaker == "self")
                        if (!String.IsNullOrEmpty(cddata[i].advancerno))
                        {
                            tempSelfTax = invquery.Where(w => w.seq == cddata[i].seq && w.undertaker == "self").Sum(s => s.taxloss);
                            if (invquery.Where(w => w.seq == cddata[i].seq).Count() > 0)
                            {
                                feecontent.Append(String.Format("{0:N2}", tempSelfTax));
                                selfTaxSum += tempSelfTax;
                            }
                            else
                            {
                                feecontent.Append(String.Format("{0:N2}", 0));
                            }
                        }
                        else
                        {
                            tempSelfTax = (cddata[i].baseamt - cddata[i].amount) != null ? Convert.ToDecimal((cddata[i].baseamt - cddata[i].amount)) : 0;
                            feecontent.Append(String.Format("{0:N2}", cddata[i].baseamt - cddata[i].amount));
                            selfTaxSum += tempSelfTax;
                        }
                        feecontent.Append(@"</td>");
                        feecontent.Append(@"</td></tr>");
                    }
                }
                string feeendtag = @"</tbody></table>";
                string feetable = feeheader + feecontent + feeendtag;
                //费用汇总
                string feesummary = "";
                string feesummary1 = "";
                feesummary += @$"<tr>
                                    <td>{L["ReimbursableAmount"]}：" + amdata.currency + " " + amdata.amount + @"</td>" +
                                    @"<td>" + @"</td>
                                </tr>" +
                                $@"<tr>
                                    <td>{L["ActualReimbursableAmount"]}：" + amdata.currency + " " + amdata.actamt + @"</td>" +
                                    $@"<td>{L["ActPayment"]}：" + amdata.currency + " " + amdata.actamt + @"</td>" +
                                    $@"<td>{L["PersonalTaxLiability"]}：" + amdata.currency + " " + String.Format("{0:N2}", selfTaxSum) + @"</td>
                                </tr>";
                feesummary1 += @$"<tr>
                                    <td>{L["payeeid"]}：" + chdata.payeeId + @"</td>" +
                                    @$"<td>{L["payeename"]}：" + chdata.payeename + @"</td>" +
                                    @$"<td>{L["Bank"]}：" + chdata.bank + @"</td>" +
                                @"</tr>";
                string summary = feesummary + feesummary1;
                string doccontent = @"<table style='text-align: center; width:85%; border-collapse: collapse; border-style:hidden;' align='center' border='1' cellspacing='0' cellpadding='0'>
            <tr>
                <td colspan='22'>" + feetable + @"</td>
            </tr>
        </table>
        <table align='center' border='0'>
            <tr style='height: 10px;'></tr>
            <tr>
                <td style='display: flex; justify-content: center;'>
                    <div style='width: 90%; border-width:0px;border-top:2pt solid Black'></div>
                </td>
            </tr>
            <tr style='height: 10px;'></tr>
        </table>
        <table align='center' border='0' style='width:85%;'>
            <tr>
                <td colspan='22'>
                    <table style='border-collapse: separate; border-spacing: 0px 20px;' cellspacing='0' cellpadding='0' align='center'>
                        <tbody>" + summary + @"</tbody>
                    </table>
                </td>
            </tr>
        </table>";
                //簽核步驟內容填充
                string signtable = "";
                string signheader = @$"<tr>
                                <td>#</td>
                                <td>{L["signstep"]}</td>
                                <td>{L["signdept"]}</td>
                                <td>{L["approvingofficer"]}</td>
                                <td>{L["signdate"]}</td>
                                <td>{L["signresult"]}</td>
                                <td>{L["signopinion"]}</td>
                            </tr>";
                string signcontent = "";
                if (signlogs.data != null)
                {
                    foreach (var item in signlogs.data.signLog)
                    {
                        signcontent += @$"<tr>
                                <td>{signlogs.data.signLog.IndexOf(item) + 1}</td>
                                <td>{item.astepname}</td>
                                <td>{item.deptid}</td>
                                <td>{item.aname}({item.aename})</td>
                                <td>{item.adate.ToString("yyyy/MM/dd hh:mm:ss")}</td>
                                <td>{item.aresult}</td>
                                <td>{item.aremark}</td>
                            </tr>";
                    }
                }
                signtable = signheader + signcontent;
                //申请人、部门数据填充
                string applyuser = "";
                applyuser += $"{L["applicant"]}：" + chdata.deptid + " " + chdata.cname + " #" + phone;
                //申请日期数据填充
                string applydate = "";
                applydate += "Request date:" + chdata.cdate;
                //替换字典
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("header", header);
                dic.Add("doccontent", doccontent);
                dic.Add("applyuser", applyuser);
                dic.Add("applydate", applydate);
                dic.Add("signtable", signtable);
                dic.Add("documentheader", documentheader);
                foreach (KeyValuePair<string, string> d in dic)
                {
                    //替换数据
                    htmlcontent.Replace(
                        string.Format("${0}$", d.Key),
                        d.Value);
                }
                html.Append(htmlcontent.ToString());
            }
            htmlresult = html.ToString();
            //对转义字符进行处理
            htmlresult = htmlresult.Replace("\n", "");
            htmlresult = htmlresult.Replace("\r", "");
            htmlresult = Regex.Replace(htmlresult, @"""(.+?)""", m => "" + m.Groups[1].Value + "");
            return htmlresult;
        }
        /// <summary>
        /// 預支金列印(html)
        /// </summary>
        public async Task<string> AdvancePaymentPrintAsync(string rno, string token)
        {
            string result = "";
            //读取模板
            string templatepath = Path.Combine(Path.Combine("Files", "Print", "PrintTemplates", "PrintTemplate.html"));
            Encoding encode = Encoding.UTF8;
            StringBuilder html = new StringBuilder();
            //模板轉成字符串 readhtml（模板字符串）
            string readhtml = File.ReadAllText(templatepath, encode);
            //存放替换后的主体内容
            string htmlresult = "";
            StringBuilder htmlcontent = new StringBuilder();
            string bodyhtml = readhtml.Substring(readhtml.IndexOf("<body>") + "<body>".Length, readhtml.IndexOf("</body>") - (readhtml.IndexOf("<body>") + "<body>".Length));
            htmlcontent.Clear();
            htmlcontent.Append(bodyhtml);
            //查询相关数据
            CashHead chquery = await _cashheadRepository.GetCashHeadByNo(rno);
            List<CashDetail> cdquery = (await _cashdetailRepository.WithDetailsAsync()).Where(w => w.rno == rno).OrderBy(w => w.seq).ToList();
            CashAmount amquery = await _cashamountRepository.GetCashAmountByNo(rno);
            //获取签核数据
            var signlogs = await _ApprovalDomainService.GetSignLogs(rno, token);
            // List<EFormSignlog> signdata = await _EFormSignlogRepository.GetListByRno(rno);
            if (chquery != null && cdquery != null && amquery != null)
            {
                CashHeadDto chdata = _objectMapper.Map<CashHead, CashHeadDto>(chquery);
                List<CashDetailDto> cddata = _objectMapper.Map<List<CashDetail>, List<CashDetailDto>>(cdquery);
                CashAmountDto amdata = _objectMapper.Map<CashAmount, CashAmountDto>(amquery);
                string phone = (await _EmployeeRepository.WithDetailsAsync()).Where(w => w.cname == chdata.cname).Select(w => w.phone).FirstOrDefault();
                //单据标题
                string documentheader = L["AdvancePayment"];
                //单据头部信息
                string header = "";
                header += @$"<tr>
                                <td>{L["CompanyCode"]}：" + chdata.company + @"</td>" +
                                @$"<td>{L["ExpenseClassNo"]}：" + chdata.dtype + @"</td>" +
                                @"<td>Project Code：" + chdata.projectcode + @"</td>" +
                                @$"<td>{L["RequestNo"]}：" + chdata.rno + @"</td>" +
                            @"</tr>";
                //预支金申请单主体内容 = 费用表格 + 分隔线 + 费用汇总
                //费用表格表头 revsdate（预定冲账日期）
                string feeheader = @$"<table style='border-collapse: collapse;border-width:0px; border-color:#d3d3d3' align='center' cellspacing='0' border='1'>
                                <tbody>
                                    <tr>
                                        <td>{L["AdvanceDate"]}</td>
                                        <td>{L["Summary"]}</td>
                                        <td>{L["AdvanceScene"]}</td>
                                        <td>{L["ChargeDate"]}</td>
                                        <td>{L["PaymentMethod"]}</td>
                                        <td>{L["currency"]}</td>
                                        <td>{L["Applyamount"]}</td>
                                        <td>{L["convertcurrency"]}</td>
                                        <td>{L["remarks"]}</td>
                                    </tr>";
                //费用表格数据
                string feecontent = "";
                for (int i = 0; i < cddata.Count; i++)
                {
                    feecontent += @"<tr><td>" + cddata[i].rdate?.ToString("yyyy/MM/dd") + @"</td>" + @"<td>" + cddata[i].summary + @"</td>" + @"<td>" + cddata[i].expname + @"</td>" + @"<td>" + cddata[i].revsdate?.ToString("yyyy/MM/dd") + @"</td>" + @"<td>" + cddata[i].payname + @"</td>" + @"<td>" + cddata[i].currency + " " + @"</td>" + @"<td>" + cddata[i].amount1 + @"</td>" + @"<td>" + cddata[i].baseamt + @"</td>" + @"<td>" + cddata[i].remarks + @"</td></tr>";
                }
                string feeendtag = @"</tbody></table>";
                string feetable = feeheader + feecontent + feeendtag;
                //费用汇总
                string feesummary = "";
                string feesummary1 = "";
                feesummary += @$"<tr>
                                    <td>{L["TotalReimbursement"]}：" + amdata.currency + " " + String.Format("{0:N2}", amdata.amount) + @"</td>" +
                                    @"<td></td>" +
                                    $@"<td>{L["ActPayment"]}：" + amdata.currency + " " + String.Format("{0:N2}", amdata.actamt) + @"</td>
                                </tr>";
                feesummary1 += @$"<tr>
                                    <td>{L["payeeid"]}：" + chdata.payeeId + @"</td>" +
                                    @$"<td>{L["payeename"]}：" + chdata.payeename + @"</td>" +
                                    @$"<td>{L["Bank"]}：" + chdata.bank + @"</td>" +
                                @"</tr>";
                string summary = feesummary + feesummary1;
                //单据主体内容合并（表格、汇总部分）
                string doccontent = @"<table style='width:85%; border-collapse: collapse; border-style:hidden;' align='center' border='1' cellspacing='0' cellpadding='0'>
            <tr>
                <td colspan='22'>" + feetable + @"</td>
            </tr>
        </table>
        <table align='center' border='0'>
            <tr style='height: 10px;'></tr>
            <tr>
                <td style='display: flex; justify-content: center;'>
                    <div style='width: 90%; border-width:0px;border-top:1pt solid Black'></div>
                </td>
            </tr>
            <tr style='height: 10px;'></tr>
        </table>
        <table align='center' border='0' style='width:85%;'>
            <tr>
                <td colspan='22'>
                    <table cellspacing='0' style='border-collapse: separate; border-spacing: 0px 20px;' cellpadding='0' align='center'>
                        <tbody>" + summary + @"</tbody>
                    </table>
                </td>
            </tr>
        </table>";
                //簽核步驟內容填充
                string signtable = "";
                string signheader = @$"<tr>
                                <td>#</td>
                                <td>{L["signstep"]}</td>
                                <td>{L["signdept"]}</td>
                                <td>{L["approvingofficer"]}</td>
                                <td>{L["signdate"]}</td>
                                <td>{L["signresult"]}</td>
                                <td>{L["signopinion"]}</td>
                            </tr>";
                string signcontent = "";
                if (signlogs.data != null)
                {
                    foreach (var item in signlogs.data.signLog)
                    {
                        signcontent += @$"<tr>
                                <td>{signlogs.data.signLog.IndexOf(item) + 1}</td>
                                <td>{item.astepname}</td>
                                <td>{item.deptid}</td>
                                <td>{item.aname}({item.aename})</td>
                                <td>{item.adate.ToString("yyyy/MM/dd hh:mm:ss")}</td>
                                <td>{item.aresult}</td>
                                <td>{item.aremark}</td>
                            </tr>";
                    }
                }
                // @"<tr>
                //     <td>1</td>
                //     <td>會計初審(一)</td>
                //     <td>MZF350</td>
                //     <td>朱秀菱(MAGGIE ZHU)</td>
                //     <td>2020/05/19 09:43:25</td>
                //     <td>同意</td>
                //     <td>&nbsp;</td>
                // </tr>
                // <tr>
                //     <td>2</td>
                //     <td>會計初審(一)</td>
                //     <td>MZF350</td>
                //     <td>朱秀菱(MAGGIE ZHU)</td>
                //     <td>2020/05/19 09:43:25</td>
                //     <td>同意</td>
                //     <td>&nbsp;</td>
                // </tr>";
                signtable = signheader + signcontent;
                //申请人、部门数据填充
                string applyuser = "";
                applyuser += $"{L["applicant"]}：" + chdata.deptid + " " + chdata.cname + " #" + phone;
                //頁碼
                //string pagination = (rnolist.IndexOf(rno) + 1).ToString() + "/" + rnolist.Count();
                //申请日期数据填充
                string applydate = "";
                applydate += "Request date:" + chdata.cdate;
                //替换字典
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("header", header);
                dic.Add("doccontent", doccontent);
                dic.Add("applyuser", applyuser);
                dic.Add("applydate", applydate);
                dic.Add("signtable", signtable);
                dic.Add("documentheader", documentheader);
                foreach (KeyValuePair<string, string> d in dic)
                {
                    //替换数据
                    htmlcontent.Replace(
                        string.Format("${0}$", d.Key),
                        d.Value);
                }
                html.Append(htmlcontent.ToString());
            }
            htmlresult = html.ToString();
            //对转义字符进行处理
            htmlresult = htmlresult.Replace("\n", "");
            htmlresult = htmlresult.Replace("\r", "");
            htmlresult = Regex.Replace(htmlresult, @"""(.+?)""", m => "" + m.Groups[1].Value + "");
            return htmlresult;
        }
        /// <summary>
        /// 批量报销列印（html）
        /// </summary>
        /// <param name="rno"></param>
        /// <returns></returns>
        public async Task<string> BatchReimbursementPrintAsync(string rno, string token)
        {
            string result = "";
            //读取模板
            string templatepath = Path.Combine(Path.Combine("Files", "Print", "PrintTemplates", "PrintTemplate.html"));
            Encoding encode = Encoding.UTF8;
            StringBuilder html = new StringBuilder();
            //模板轉成字符串 readhtml（模板字符串）
            string readhtml = File.ReadAllText(templatepath, encode);
            //存放替换后的主体内容
            string htmlresult = "";
            StringBuilder htmlcontent = new StringBuilder();
            string bodyhtml = readhtml.Substring(readhtml.IndexOf("<body>") + "<body>".Length, readhtml.IndexOf("</body>") - (readhtml.IndexOf("<body>") + "<body>".Length));
            htmlcontent.Clear();
            htmlcontent.Append(bodyhtml);
            //查询相关数据
            CashHead chquery = await _cashheadRepository.GetCashHeadByNo(rno);
            List<CashDetail> cdquery = (await _cashdetailRepository.WithDetailsAsync()).Where(w => w.rno == rno).OrderBy(w => w.seq).ToList();
            CashAmount amquery = await _cashamountRepository.GetCashAmountByNo(rno);
            // List<EFormSignlog> signdata = await _EFormSignlogRepository.GetListByRno(rno);
            //获取签核数据
            var signlogs = await _ApprovalDomainService.GetSignLogs(rno, token);
            if (chquery != null && cdquery != null && amquery != null)
            {
                CashHeadDto chdata = _objectMapper.Map<CashHead, CashHeadDto>(chquery);
                List<CashDetailDto> cddata = _objectMapper.Map<List<CashDetail>, List<CashDetailDto>>(cdquery);
                CashAmountDto amdata = _objectMapper.Map<CashAmount, CashAmountDto>(amquery);
                string phone = (await _EmployeeRepository.WithDetailsAsync()).Where(w => w.cname == chdata.cname).Select(w => w.phone).FirstOrDefault();
                //单据标题
                string documentheader = L["BatchReimbursement"];
                //单据头部信息填充：公司別、費用簽核項目、ProjectCode、報銷單號
                string header = "";
                header += @$"<tr>
                                <td>{L["CompanyCode"]}：" + chdata.company + @"</td>" +
                                @$"<td>{L["ExpenseClassNo"]}：" + chdata.dtype + @"</td>" +
                                @"<td>Project Code：" + chdata.projectcode + @"</td>" +
                                @$"<td>{L["RequestNo"]}：" + chdata.rno + @"</td>" +
                            @"</tr>";
                //费用表格表头：費用發生日期、受款人工號、受款人名稱、報銷情景、銀行名稱、摘要、費用歸屬部門、幣別、申請金額、折算本位幣
                string feeheader = @$"<table style='border-collapse: collapse;border-width:0px; border-color:#d3d3d3' align='center' cellspacing='0' border='1'>
                                <tbody>
                                    <tr>
                                        <td>{L["ExpenseDate"]}</td>
                                        <td>{L["payeeid"]}</td>
                                        <td>{L["payeename"]}</td>
                                        <td>{L["ReimbursementScene"]}</td>
                                        <td>{L["Bank"]}</td>
                                        <td>{L["Summary"]}</td>
                                        <td>{L["expensedept"]}</td>
                                        <td>{L["currency"]}</td>
                                        <td>{L["Applyamount"]}</td>
                                        <td>{L["convertcurrency"]}</td>
                                        <td>{L["PersonalTaxLiability"]}</td>
                                        <td>{L["ActualExpenseAmount"]}</td>
                                    </tr>";
                //费用表格数据
                string feecontent = "";
                for (int i = 0; i < cddata.Count; i++)
                {
                    feecontent += @"<tr><td>" + cddata[i].rdate?.ToString("yyyy/MM/dd") + @"</td>" +
                                @"<td>" + cddata[i].payeeid + @"</td>" +
                                @"<td>" + cddata[i].payeename + @"</td>" +
                                @"<td>" + cddata[i].expname + @"</td>" +
                                @"<td>" + cddata[i].bank + @"</td>" +
                                @"<td>" + cddata[i].summary + @"</td>" +
                                @"<td>" + cddata[i].deptid + @"</td>" +
                                @"<td>" + cddata[i].currency + " " + @"</td>" +
                                @"<td>" + String.Format("{0:N2}", cddata[i].amount1) + @"</td>" +
                                @"<td>" + String.Format("{0:N2}", cddata[i].baseamt) +
                                @"<td>" + String.Format("{0:N2}", cddata[i].baseamt - cddata[i].amount) + @"</td>" +
                                @"<td>" + String.Format("{0:N2}", cddata[i].amount) +
                                @"</td></tr>";
                }
                string feeendtag = @"</tbody></table>";
                string feetable = feeheader + feecontent + feeendtag;
                //费用汇总
                string feesummary = "";
                string feesummary1 = "";
                //wei標記了此處
                feesummary += @$"<tr>
                                    <td>{L["ActualExpenseAmount"]}：" + amdata.currency + " " + String.Format("{0:N2}", amdata.amount) + @"</td>"
                                     + @"</tr>"
                                     +
                                $@"<tr><td>
                                </td></tr>";
                feesummary1 += @$"<tr>
                                    <td>{L["ActualReimbursableAmount"]}：" + amdata.currency + " " + String.Format("{0:N2}", amdata.actamt) + @"</td>" +
                                    @$"<td>{L["PersonalTaxLiability"]}" + amdata.currency + " " + String.Format("{0:N2}", (amdata.amount - amdata.actamt)) + @"</td>" +
                                    @$"<td>{L["ActPayment"]}：" + amdata.currency + " " + String.Format("{0:N2}", amdata.actamt) + @"</td>" +
                                @"</tr>";
                string summary = feesummary + feesummary1;
                string doccontent = @"<table style='width:85%; border-collapse: collapse; border-style:hidden;' align='center' border='1' cellspacing='0' cellpadding='0'>
            <tr>
                <td colspan='22'>" + feetable + @"</td>
            </tr>
        </table>
        <table align='center' border='0'>
            <tr style='height: 10px;'></tr>
            <tr>
                <td style='display: flex; justify-content: center;'>
                    <div style='width: 90%; border-width:0px;border-top:1pt solid Black'></div>
                </td>
            </tr>
            <tr style='height: 10px;'></tr>
        </table>
        <table align='center' border='0' style='width:85%;'>
            <tr>
                <td colspan='22'>
                    <table style='border-collapse: separate; border-spacing: 0px 15px;' cellspacing='0' cellpadding='0' align='center'>
                        <tbody>" + summary + @"</tbody>
                    </table>
                </td>
            </tr>
        </table>";
                //簽核步驟內容填充
                string signtable = "";
                string signheader = @$"<tr>
                                <td>#</td>
                                <td>{L["signstep"]}</td>
                                <td>{L["signdept"]}</td>
                                <td>{L["approvingofficer"]}</td>
                                <td>{L["signdate"]}</td>
                                <td>{L["signresult"]}</td>
                                <td>{L["signopinion"]}</td>
                            </tr>";
                string signcontent = "";
                if (signlogs.data != null)
                {
                    foreach (var item in signlogs.data.signLog)
                    {
                        signcontent += @$"<tr>
                                <td>{signlogs.data.signLog.IndexOf(item) + 1}</td>
                                <td>{item.astepname}</td>
                                <td>{item.deptid}</td>
                                <td>{item.aname}({item.aename})</td>
                                <td>{item.adate.ToString("yyyy/MM/dd hh:mm:ss")}</td>
                                <td>{item.aresult}</td>
                                <td>{item.aremark}</td>
                            </tr>";
                    }
                }
                // string signcontent =
                //             @"<tr>
                //                 <td>1</td>
                //                 <td>會計初審(一)</td>
                //                 <td>MZF350</td>
                //                 <td>朱秀菱(MAGGIE ZHU)</td>
                //                 <td>2020/05/19 09:43:25</td>
                //                 <td>同意</td>
                //                 <td>&nbsp;</td>
                //             </tr>
                //             <tr>
                //                 <td>2</td>
                //                 <td>會計初審(一)</td>
                //                 <td>MZF350</td>
                //                 <td>朱秀菱(MAGGIE ZHU)</td>
                //                 <td>2020/05/19 09:43:25</td>
                //                 <td>同意</td>
                //                 <td>&nbsp;</td>
                //             </tr>";
                signtable = signheader + signcontent;
                //申请人、部门数据填充
                string applyuser = "";
                applyuser += $"{L["applicant"]}：" + chdata.deptid + " " + chdata.cname + " #" + phone;
                //申请日期数据填充
                string applydate = "";
                applydate += "Request date:" + chdata.cdate;
                //替换字典
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("header", header);
                dic.Add("doccontent", doccontent);
                dic.Add("applyuser", applyuser);
                dic.Add("applydate", applydate);
                dic.Add("signtable", signtable);
                dic.Add("documentheader", documentheader);
                foreach (KeyValuePair<string, string> d in dic)
                {
                    //替换数据
                    htmlcontent.Replace(
                        string.Format("${0}$", d.Key),
                        d.Value);
                }
                html.Append(htmlcontent.ToString());
            }
            htmlresult = html.ToString();
            htmlresult = htmlresult.Replace("\n", "");
            htmlresult = htmlresult.Replace("\r", "");
            htmlresult = Regex.Replace(htmlresult, @"""(.+?)""", m => "" + m.Groups[1].Value + "");
            return htmlresult;
        }
        /// <summary>
        /// 返台会议单据列印
        /// </summary>
        /// <param name="rno"></param>
        /// <returns></returns>
        public async Task<string> ReturnTaiwanMeetingPrint(string rno, string token)
        {
            //读取模板
            string templatepath = Path.Combine(Path.Combine("Files", "Print", "PrintTemplates", "PrintTemplate.html"));
            Encoding encode = Encoding.UTF8;
            StringBuilder html = new StringBuilder();
            //模板轉成字符串 readhtml（模板字符串）
            string readhtml = File.ReadAllText(templatepath, encode);
            //存放替换后的主体内容
            string htmlresult = "";
            StringBuilder htmlcontent = new StringBuilder();
            string bodyhtml = readhtml.Substring(readhtml.IndexOf("<body>") + "<body>".Length, readhtml.IndexOf("</body>") - (readhtml.IndexOf("<body>") + "<body>".Length));
            htmlcontent.Clear();
            htmlcontent.Append(bodyhtml);
            //查询相关数据
            CashHead chquery = await _cashheadRepository.GetCashHeadByNo(rno);
            //CashDetail cdquery = await _cashdetailRepository.GetCashDetailByNo(rno);
            List<CashDetail> cdquery = (await _cashdetailRepository.WithDetailsAsync()).Where(w => w.rno == rno).OrderBy(w => w.seq).ToList();
            CashAmount amquery = await _cashamountRepository.GetCashAmountByNo(rno);
            List<EFormSignlog> signdata = await _EFormSignlogRepository.GetListByRno(rno);
            if (chquery != null && cdquery.Count > 0 && amquery != null)
            {
                CashHeadDto chdata = _objectMapper.Map<CashHead, CashHeadDto>(chquery);
                List<CashDetailDto> cddata = _objectMapper.Map<List<CashDetail>, List<CashDetailDto>>(cdquery);
                CashAmountDto amdata = _objectMapper.Map<CashAmount, CashAmountDto>(amquery);
                string phone = (await _EmployeeRepository.WithDetailsAsync()).Where(w => w.cname == chdata.cname).Select(w => w.phone).FirstOrDefault();
                //获取签核数据
                var signlogs = await _ApprovalDomainService.GetSignLogs(rno, token);
                //单据标题
                string documentheader = L["Print-ReurnTaiwanMeeting"];
                //单据头部信息填充
                string header = "";
                header += @$"<tr>
                                <td>{L["CompanyCode"]}：" + chdata.company + @"</td>" +
                                @$"<td>{L["ExpenseClassNo"]}：" + chdata.dtype + @"</td>" +
                                @"<td>Project Code：" + chdata.projectcode + @"</td>" +
                                @$"<td>{L["RequestNo"]}：" + chdata.rno + @"</td>" +
                            @"</tr>";
                //费用表格表头
                string feeheader = "";
                //费用表格数据
                StringBuilder feecontent = new StringBuilder();
                feeheader = @$"<table style='border-collapse: collapse;border-width:0px; border-color:#d3d3d3' align='center' cellspacing='0' border='1'>
                                <tbody>
                                    <tr>
                                        <td>{L["ExpenseDate"]}</td>
                                        <td>{L["ReimbursementScene"]}</td>
                                        <td>{L["Summary"]}</td>
                                        <td>{L["expensedept"]}</td>
                                        <td>{L["currency"]}</td>
                                        <td>{L["Applyamount"]}</td>
                                        <td>{L["convertcurrency"]}</td>
                                        <td>{L["PersonalTaxLiability"]}</td>
                                        <td>{L["ActualExpenseAmount"]}</td>
                                    </tr>";
                for (int i = 0; i < cddata.Count; i++)
                {
                    feecontent.Append(@"<tr><td>");
                    feecontent.Append(cddata[i].rdate?.ToString("yyyy/MM/dd"));
                    feecontent.Append(@"</td>");
                    feecontent.Append(@"<td>");
                    feecontent.Append(cddata[i].expname);
                    feecontent.Append(@"</td>");
                    feecontent.Append(@"<td>");
                    feecontent.Append(cddata[i].summary);
                    feecontent.Append(@"</td>");
                    feecontent.Append(@"<td>");
                    feecontent.Append(cddata[i].deptid);
                    feecontent.Append(@"</td>");
                    feecontent.Append(@"<td>");
                    feecontent.Append(cddata[i].currency + " ");
                    feecontent.Append(@"</td>");
                    feecontent.Append(@"<td>");
                    feecontent.Append(String.Format("{0:N2}", cddata[i].amount1));
                    feecontent.Append(@"</td>");
                    feecontent.Append(@"<td>");
                    feecontent.Append(String.Format("{0:N2}", cddata[i].baseamt));
                    feecontent.Append(@"</td>");
                    feecontent.Append(@"<td>");
                    feecontent.Append(String.Format("{0:N2}", cddata[i].baseamt - cddata[i].amount));
                    feecontent.Append(@"</td>");
                    feecontent.Append(@"<td>");
                    feecontent.Append(String.Format("{0:N2}", cddata[i].amount));
                    feecontent.Append(@"</td></tr>");
                }
                string feeendtag = @"</tbody></table>";
                string feetable = feeheader + feecontent + feeendtag;
                //费用汇总
                string feesummary = "";
                string feesummary1 = "";
                feesummary += @$"<tr>
                                    <td>{L["ExpenseAmountCount"]}：" + amdata.currency + " " + amdata.amount + @"</td>" + @"</td>
                                </tr>" +
                                $@"<tr>
                                    <td>{L["ActualReimbursableAmount"]}：" + amdata.currency + " " + amdata.actamt + $@"</td>" +
                                    $@"<td>{L["PersonalTaxLiability"]}" + amdata.currency + " " + (amdata.amount - amdata.actamt) +
                                    $@"<td>{L["ActPayment"]}：" + amdata.currency + " " + amdata.actamt + @"</td>
                                </tr>";
                feesummary1 += @$"<tr>
                                    <td>{L["payeeid"]}：" + chdata.payeeId + @"</td>" +
                                    @$"<td>{L["payeename"]}：" + chdata.payeename + @"</td>" +
                                    @$"<td>{L["Bank"]}：" + chdata.bank + @"</td>" +
                                @"</tr>";
                string summary = feesummary + feesummary1;
                string doccontent = @"<table style='text-align: center; width:85%; border-collapse: collapse; border-style:hidden;' align='center' border='1' cellspacing='0' cellpadding='0'>
            <tr>
                <td colspan='22'>" + feetable + @"</td>
            </tr>
        </table>
        <table align='center' border='0'>
            <tr style='height: 10px;'></tr>
            <tr>
                <td style='display: flex; justify-content: center;'>
                    <div style='width: 90%; border-width:0px;border-top:2pt solid Black'></div>
                </td>
            </tr>
            <tr style='height: 10px;'></tr>
        </table>
        <table align='center' border='0' style='width:85%;'>
            <tr>
                <td colspan='22'>
                    <table style='border-collapse: separate; border-spacing: 0px 20px;' cellspacing='0' cellpadding='0' align='center'>
                        <tbody>" + summary + @"</tbody>
                    </table>
                </td>
            </tr>
        </table>";
                //簽核步驟內容填充
                string signtable = "";
                string signheader = @$"<tr>
                                <td>#</td>
                                <td>{L["signstep"]}</td>
                                <td>{L["signdept"]}</td>
                                <td>{L["approvingofficer"]}</td>
                                <td>{L["signdate"]}</td>
                                <td>{L["signresult"]}</td>
                                <td>{L["signopinion"]}</td>
                            </tr>";
                string signcontent = "";
                if (signlogs.data != null)
                {
                    foreach (var item in signlogs.data.signLog)
                    {
                        signcontent += @$"<tr>
                                <td>{signlogs.data.signLog.IndexOf(item) + 1}</td>
                                <td>{item.astepname}</td>
                                <td>{item.deptid}</td>
                                <td>{item.aname}({item.aename})</td>
                                <td>{item.adate.ToString("yyyy/MM/dd hh:mm:ss")}</td>
                                <td>{item.aresult}</td>
                                <td>{item.aremark}</td>
                            </tr>";
                    }
                }
                signtable = signheader + signcontent;
                //申请人、部门数据填充
                string applyuser = "";
                applyuser += $"{L["applicant"]}：" + chdata.deptid + " " + chdata.cname + " #" + phone;
                //申请日期数据填充
                string applydate = "";
                applydate += "Request date:" + chdata.cdate;
                //替换字典
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("header", header);
                dic.Add("doccontent", doccontent);
                dic.Add("applyuser", applyuser);
                dic.Add("applydate", applydate);
                dic.Add("signtable", signtable);
                dic.Add("documentheader", documentheader);
                foreach (KeyValuePair<string, string> d in dic)
                {
                    //替换数据
                    htmlcontent.Replace(
                        string.Format("${0}$", d.Key),
                        d.Value);
                }
                html.Append(htmlcontent.ToString());
            }
            htmlresult = html.ToString();
            //对转义字符进行处理
            htmlresult = htmlresult.Replace("\n", "");
            htmlresult = htmlresult.Replace("\r", "");
            htmlresult = Regex.Replace(htmlresult, @"""(.+?)""", m => "" + m.Groups[1].Value + "");
            return htmlresult;
        }
        /// <summary>
        /// 薪資請款列印
        /// </summary>
        /// <param name="rno"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<string> PayrollRequestPrintAsync(string rno, string token)
        {
            //读取模板
            string templatepath = Path.Combine(Path.Combine("Files", "Print", "PrintTemplates", "PrintTemplate.html"));
            Encoding encode = Encoding.UTF8;
            StringBuilder html = new StringBuilder();
            //模板轉成字符串 readhtml（模板字符串）
            string readhtml = File.ReadAllText(templatepath, encode);
            //存放替换后的主体内容
            string htmlresult = "";
            StringBuilder htmlcontent = new StringBuilder();
            string bodyhtml = readhtml.Substring(readhtml.IndexOf("<body>") + "<body>".Length, readhtml.IndexOf("</body>") - (readhtml.IndexOf("<body>") + "<body>".Length));
            htmlcontent.Clear();
            htmlcontent.Append(bodyhtml);
            //查询相关数据
            CashHead chquery = await _cashheadRepository.GetCashHeadByNo(rno);
            List<CashDetail> cdquery = (await _cashdetailRepository.WithDetailsAsync()).Where(w => w.rno == rno).OrderBy(w => w.seq).ToList();
            CashAmount amquery = await _cashamountRepository.GetCashAmountByNo(rno);
            //获取签核数据
            var signlogs = await _ApprovalDomainService.GetSignLogs(rno, token);
            if (chquery != null && cdquery != null && amquery != null)
            {
                CashHeadDto chdata = _objectMapper.Map<CashHead, CashHeadDto>(chquery);
                List<CashDetailDto> cddata = _objectMapper.Map<List<CashDetail>, List<CashDetailDto>>(cdquery);
                CashAmountDto amdata = _objectMapper.Map<CashAmount, CashAmountDto>(amquery);
                string phone = (await _EmployeeRepository.WithDetailsAsync()).Where(w => w.cname == chdata.cname).Select(w => w.phone).FirstOrDefault();
                //单据标题
                string documentheader = L["PayrollRequest"];
                //单据头部信息填充：公司別、費用簽核項目、ProjectCode、報銷單號
                string header = "";
                header += @$"<tr>
                                <td>{L["CompanyCode"]}：" + chdata.company + @"</td>" +
                                @$"<td>{L["ExpenseClassNo"]}：" + chdata.dtype + @"</td>" +
                                @"<td>Project Code：" + chdata.projectcode + @"</td>" +
                                @$"<td>{L["RequestNo"]}：" + chdata.rno + @"</td>" +
                            @"</tr>";
                //费用表格表头：companycode、報銷情景（detail的expname）、需款日期、銀行、請款方式、幣別、報銷金額、摘要（detail的remarks）
                string feeheader = @$"<table style='border-collapse: collapse;border-width:0px; border-color:#d3d3d3' align='center' cellspacing='0' border='1'>
                                <tbody>
                                    <tr>
                                        <td>{L["CashX-CompanyCode"]}</td>
                                        <td>{L["CashX-Print-ExpName"]}</td>
                                        <td>{L["CashX-ExpenseDate"]}</td>
                                        <td>{L["CashX-SalaryPeriod"]}</td>
                                        <td>{L["CashX-Print-Bank"]}</td>
                                        <td>{L["PaymentMethod"]}</td>
                                        <td>{L["currency"]}</td>
                                        <td style='text-align:right;' >{L["Applyamount"]}</td>
                                        <td style='text-align:right;' >{L["CashX-Payamount"]}</td>
                                        <td>{L["Summary"]}</td>
                                    </tr>";
                //费用表格数据
                string feecontent = "";
                for (int i = 0; i < cddata.Count; i++)
                {
                    feecontent += @"<tr><td>" + cddata[i].companycode + @"</td>" +
                                @"<td>" + cddata[i].expname + @"</td>" +
                                @"<td>" + Convert.ToDateTime(cddata[i].rdate).ToString("yyyy/MM/dd") + @"</td>" +
                                 @"<td>" + cddata[i].summary + @"</td>" +
                                @"<td>" + cddata[i].bank + @"</td>" +
                                @"<td>" + cddata[i].payname + @"</td>" +
                                @"<td>" + cddata[i].currency + " " + @"</td>" +
                                @"<td style='text-align:right;' >" + String.Format("{0:N2}", cddata[i].baseamt) + @"</td>" +
                                @"<td style='text-align:right;' >" + String.Format("{0:N2}", cddata[i].amount) + @"</td>" +
                                @"<td>" + cddata[i].remarks +
                                @"</td></tr>";
                }
                string feeendtag = @"</tbody></table>";
                string feetable = feeheader + feecontent + feeendtag;
                //费用汇总
                string feesummary = "";
                feesummary += @$"<tr>
                                    <td>{L["CashX-Print-Amount"]}：" + amdata.currency + " " + String.Format("{0:N2}", amdata.amount) + @"</td>" +
                                    @$"<td>{L["CashX-Print-Actamt"]}：" + amdata.currency + " " + String.Format("{0:N2}", amdata.actamt) + @"</td>" +
                                @"</tr>";
                string summary = feesummary;
                string doccontent = @"<table style='width:85%; border-collapse: collapse; border-style:hidden;' align='center' border='1' cellspacing='0' cellpadding='0'>
            <tr>
                <td colspan='22'>" + feetable + @"</td>
            </tr>
        </table>
        <table align='center' border='0'>
            <tr style='height: 10px;'></tr>
            <tr>
                <td style='display: flex; justify-content: center;'>
                    <div style='width: 90%; border-width:0px;border-top:1pt solid Black'></div>
                </td>
            </tr>
            <tr style='height: 10px;'></tr>
        </table>
        <table align='center' border='0' style='width:85%;'>
            <tr>
                <td colspan='22'>
                    <table style='border-collapse: separate; border-spacing: 0px 15px;' cellspacing='0' cellpadding='0' align='center'>
                        <tbody>" + summary + @"</tbody>
                    </table>
                </td>
            </tr>
        </table>";
                //簽核步驟內容填充
                string signtable = "";
                string signheader = @$"<tr>
                                <td>#</td>
                                <td>{L["signstep"]}</td>
                                <td>{L["signdept"]}</td>
                                <td>{L["approvingofficer"]}</td>
                                <td>{L["signdate"]}</td>
                                <td>{L["signresult"]}</td>
                                <td>{L["signopinion"]}</td>
                            </tr>";
                string signcontent = "";
                if (signlogs.data != null)
                {
                    foreach (var item in signlogs.data.signLog)
                    {
                        signcontent += @$"<tr>
                                <td>{signlogs.data.signLog.IndexOf(item) + 1}</td>
                                <td>{item.astepname}</td>
                                <td>{item.deptid}</td>
                                <td>{item.aname}({item.aename})</td>
                                <td>{item.adate.ToString("yyyy/MM/dd hh:mm:ss")}</td>
                                <td>{item.aresult}</td>
                                <td>{item.aremark}</td>
                            </tr>";
                    }
                }
                signtable = signheader + signcontent;
                //申请人、部门数据填充
                string applyuser = "";
                applyuser += $"{L["applicant"]}：" + chdata.deptid + " " + chdata.cname + " #" + phone;
                //申请日期数据填充
                string applydate = "";
                applydate += "Request date:" + chdata.cdate;
                //替换字典
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("header", header);
                dic.Add("doccontent", doccontent);
                dic.Add("applyuser", applyuser);
                dic.Add("applydate", applydate);
                dic.Add("signtable", signtable);
                dic.Add("documentheader", documentheader);
                foreach (KeyValuePair<string, string> d in dic)
                {
                    //替换数据
                    htmlcontent.Replace(
                        string.Format("${0}$", d.Key),
                        d.Value);
                }
                html.Append(htmlcontent.ToString());
            }
            htmlresult = html.ToString();
            htmlresult = htmlresult.Replace("\n", "");
            htmlresult = htmlresult.Replace("\r", "");
            htmlresult = Regex.Replace(htmlresult, @"""(.+?)""", m => "" + m.Groups[1].Value + "");
            return htmlresult;
        }
    }
}
