using ERS.Application.Contracts.DTO.Application;
using ERS.Domain.IDomainServices;
using ERS.DTO;
using ERS.DTO.Application;
using ERS.DTO.BDCar;
using ERS.Entities;
using ERS.IDomainServices;
using ERS.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.ObjectMapping;
using ERS.Localization;
using System.IO;
using ERS.DTO.Invoice;
using ERS.DTO.MobileSign;
namespace ERS.DomainServices
{
    public class Cash1DomainService : CommonDomainService, ICash1DomainService
    {
        private IObjectMapper _ObjectMapper;
        private IAutoNoRepository _AutoNoRepository;
        private ICashHeadRepository _CashHeadRepository;
        private ICashDetailRepository _CashDetailRepository;
        private ICashFileRepository _CashFileRepository;
        private ICashAccountRepository _CashAccountRepository;
        private IInvoiceRepository _InvoiceRepository;
        private ICashAmountRepository _CashAmountRepository;
        private IMinioDomainService _MinioDomainService;
        private IEFormHeadRepository _EFormHeadRepository;
        private IBDExpRepository _bdexpRepository;
        private IEmpOrgRepository _emporgRepository;
        private IBDCarRepository _bdcarRepository;
        private IConfiguration _Configuration;
        private IApprovalPaperDomainService _EFormPaperDomainService;
        private IInvoiceDomainService _InvoiceDomainService;
        private IApprovalDomainService _ApprovalDomainService;
        private IBDMealAreaRepository _BDMealAreaRepository;
        private ICompanyRepository _CompanyRepository;
        private ICurrencyDomainService _CurrencyDomainService; private IBDExpDomainService _BDExpDomainService;
        private IEFormAlistRepository _EFormAlistRepository;
        private IEFormAuserRepository _EFormAuserRepository;
        private IBDInvoiceFolderRepository _BDInvoiceFolderRepository;
        private IApprovalPaperRepository _EFormPaperRepository;
        private IConfiguration _configuration;
        private IMobileSignService _mobileSignService;
        private IApprovalFlowDomainService _iApprovalFlowDomainService;
        private IMinioDomainService _minioDomainService;
        public Cash1DomainService(
            IAutoNoRepository AutoNoRepository,
            IObjectMapper ObjectMapper,
            ICashHeadRepository CashHeadRepository,
            ICashDetailRepository CashDetailRepository,
            ICashFileRepository CashFileRepository,
            ICashAccountRepository CashAccountRepository,
            IInvoiceRepository InvoiceRepository,
            ICashAmountRepository CashAmountRepository,
            IMinioDomainService MinioDomainService,
            IEFormHeadRepository EFormHeadRepository,
            IBDExpRepository bdexpRepository,
            IEmpOrgRepository emporgRepository,
            IBDCarRepository bdcarRepository,
            IConfiguration Configuration,
            IApprovalPaperDomainService EFormPaperDomainService,
            IInvoiceDomainService InvoiceDomainService,
            IApprovalDomainService ApprovalDomainService,
            IBDMealAreaRepository BDMealAreaRepository,
            ICompanyRepository CompanyRepository,
            ICurrencyDomainService CurrencyDomainService,
            IApprovalPaperRepository EFormPaperRepository,
            IBDExpDomainService BDExpDomainService,
            IEFormAlistRepository EFormAlistRepository,
            IEFormAuserRepository EFormAuserRepository,
            IBDInvoiceFolderRepository BDInvoiceFolderRepository,
            IConfiguration configuration,
            IMobileSignService mobileSignService,
            IApprovalFlowDomainService approvalFlowDomainService,
            IMinioDomainService minioDomainService

)
        {
            _ObjectMapper = ObjectMapper;
            _AutoNoRepository = AutoNoRepository;
            _CashHeadRepository = CashHeadRepository;
            _CashDetailRepository = CashDetailRepository;
            _CashFileRepository = CashFileRepository;
            _CashAccountRepository = CashAccountRepository;
            _InvoiceRepository = InvoiceRepository;
            _CashAmountRepository = CashAmountRepository;
            _MinioDomainService = MinioDomainService;
            _EFormHeadRepository = EFormHeadRepository;
            _bdexpRepository = bdexpRepository;
            _emporgRepository = emporgRepository;
            _bdcarRepository = bdcarRepository;
            _Configuration = Configuration;
            _EFormPaperDomainService = EFormPaperDomainService;
            _InvoiceDomainService = InvoiceDomainService;
            _ApprovalDomainService = ApprovalDomainService;
            LocalizationResource = typeof(ERSResource);
            _BDMealAreaRepository = BDMealAreaRepository;
            _CompanyRepository = CompanyRepository;
            _CurrencyDomainService = CurrencyDomainService;
            _BDExpDomainService = BDExpDomainService;
            _EFormAlistRepository = EFormAlistRepository;
            _EFormAuserRepository = EFormAuserRepository;
            _BDInvoiceFolderRepository = BDInvoiceFolderRepository;
            _EFormPaperRepository = EFormPaperRepository;
            _configuration = configuration;
            _mobileSignService = mobileSignService;
            _iApprovalFlowDomainService = approvalFlowDomainService;
            _minioDomainService = minioDomainService;
        }
        public async Task<Result<CashResult>> Submit(IFormCollection formCollection, string user, string token, string status = "P")
        {
            Result<CashResult> result = new();
            CashResult cash = new();
            result.status = 2;
            CashHead list = TransformAndSetData(formCollection, user);
            if (!string.IsNullOrEmpty(list.rno))
            {
                //判断单据是否在签核中
                if (await _EFormHeadRepository.IsSigned(list.rno))
                {
                    result.message = L["008"];
                    return result;
                }
                if (await _CashHeadRepository.IsSelfCreate(list.rno, user))
                {
                    result.message = L["009"];
                    return result;
                }
            }
            // 检查选择发票是否有异常
            List<Guid?> Iids = list.InvoiceList.Where(i => i.invoiceid != null).Select(i => i.invoiceid).Distinct().ToList();
            if (status.Equals("P") && await _BDInvoiceFolderRepository.CheckInvoiceIsAbnormal(Iids, list.payeeId))
            {
                result.message = L["InvoiceIsAbnormal"];
                return result;
            }
            list.status = status;
            list.SetPayeeAccount(await _CashAccountRepository.GetAccount(list.payeeId));
            if (status == "T")
                list.SetKeepStatus("RQ101");
            if (!list.CheckIsExistRno())
            {
                string rno = await _AutoNoRepository.CreateCash1RNo();//CreateCash1No();
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
                temp.SetCompany(list.EFormHead.company);
                list.UpdateEFormHead(temp);
            }
            await SaveInvoiceFolder(list, Iids);
            //检查发票是否可请款
            Result<CashResult> invoviceResult = await _InvoiceDomainService.checkInvoicePaid(list, token);
            if (invoviceResult.status == 2)
            {
                return invoviceResult;
            }
            if (status.Equals("P"))
                await _BDInvoiceFolderRepository.InsertRno(Iids, list.rno);
            List<string> codes = list.CashDetailList.Select(i => i.expcode).Distinct().ToList();
            List<string> acctcodes = list.CashDetailList.Select(w => w.acctcode).Distinct().ToList();
            string type = await _BDExpDomainService.GetSignDType(list.company, codes, acctcodes);
            list.Setdtype(type);
            // file
            await SaveFile(formCollection, list, Iids);
            cash.Stat = false;
            if (status.Equals("P") && invoviceResult.data.Stat)
            {
                await _EFormPaperDomainService.addPaper(list.rno, list.formcode, user, list.company);
                cash.Stat = true;
            }
            // 冲账
            if (status.Equals("P"))
            {
                Result<string> temp = await _CashAmountRepository.Reversal(list);
                if (temp.status == 2)
                {
                    result.status = 2;
                    result.message = String.Format(L["reversalError"], temp.data);
                    return result;
                }
            }
            bool saveFlag=await InsertData(list);
            if (saveFlag)
            {
                if (status.Equals("P"))
                {
                    //新签核flow

                    Result<string> signFlowResult = await _iApprovalFlowDomainService.ApplyApprovalFlow(list, list.CashDetailList.ToList());


                    //await _ApprovalDomainService.CreateSignSummary(list, invoviceResult.data.ElecInvStat, token);
                    if (list.InvoiceList.Count > 0 && _configuration.GetSection("isDev").Value != "true")
                    {
                        List<UpdatePayStatDto> invs = _ObjectMapper.Map<List<Invoice>, List<UpdatePayStatDto>>(list.InvoiceList.ToList());
                        ERSApplyDto eRSApplyDto = new()
                        {
                            Invoices = _ObjectMapper.Map<List<UpdatePayStatDto>, List<ERSInvDto>>(invs),
                            ERSRno = list.rno,
                            ERSUser = list.cuser,
                            ERSComCode = list.company
                        };
                        await _InvoiceDomainService.UpdateInvStatToRequested(eRSApplyDto, token);
                        //await _InvoiceDomainService.UpdateInvoiceToRequested(invs, token);
                    }
                    //抛手机签核
                    //await _mobileSignService.SendMobileSignXMLData(list.rno);
                }
            }
            cash.rno = list.rno;
            result.data = cash;
            result.message = (cash.Stat ? L["AddPaperTipMessage"] : L["UnAddPaperTipMessage"]) + (invoviceResult.data.ElecButNotPdf ? L["AddElecInvTipMessage"] : "");
            result.status = 1;
            return result;
        }
        async Task SaveInvoiceFolder(CashHead list, List<Guid?> ids)
        {
            string area = await _minioDomainService.GetMinioArea(list.cuser);
            List<BDInvoiceFolder> invoices = await _BDInvoiceFolderRepository.GetInvInfoListById(ids);
            List<CashFile> fileList = new();
            foreach (var invoice in invoices)
            {
                var temp = list.InvoiceList.Where(i => i.invoiceid == invoice.Id).First();
                string path = "";
                if (!string.IsNullOrEmpty(invoice.filepath))
                    path = await _MinioDomainService.CopyInvoiceAsync(list.rno, invoice.filepath, invoice.invno,area);
                CashFile cashFile = new CashFile()
                {
                    rno = list.rno,
                    seq = temp.seq,
                    item = temp.item,
                    category = invoice.invtype,
                    filetype = !string.IsNullOrEmpty(invoice.filepath) ? invoice.category : "",
                    path = path,
                    filename = !string.IsNullOrEmpty(invoice.filepath) ? invoice.invtype + invoice.amount.ToString() + Path.GetExtension(invoice.filepath) : "",
                    formcode = list.formcode,
                    tofn = !string.IsNullOrEmpty(invoice.filepath) ? Path.GetFileName(invoice.filepath) : "",
                    ishead = "N",
                    company = list.company,
                    cdate = DateTime.Now,
                    cuser = list.cuser
                };
                list.AddCashFile(cashFile);
            }
        }
        async Task SaveFile(IFormCollection formCollection, CashHead list, List<Guid?> ids)
        {
            string area = await _minioDomainService.GetMinioArea(list.cuser);
            //List<BDInvoiceFolder> invoices = await _BDInvoiceFolderRepository.GetInvInfoListById(ids);
            //foreach (var invoice in invoices)
            //{
            //    if (!string.IsNullOrEmpty(invoice.filepath))
            //    {
            //        string path = await _MinioDomainService.CopyInvoiceAsync(list.rno, invoice.filepath);
            //        list.SetOriginalFileNameAndSavePathByInvoiceFolder(invoice.Id, path, Path.GetFileName(invoice.filepath));
            //    }
            //}
            Dictionary<string, int> fileCount = new Dictionary<string, int>();
            List<string> fileNames = formCollection.Files.Select(w => w.FileName).ToList();
            foreach (var file in formCollection.Files)
            {
                string path = "";
                string type = "";
                if (!string.IsNullOrEmpty(_Configuration.GetSection("UnitTest")?.Value))
                    type = "image/jpeg";
                else
                    type = file.ContentType;
                string fileName = file.FileName;
                using (var steam = file.OpenReadStream())
                {
                    if (fileCount.ContainsKey(fileName))
                    {
                        int count = fileCount[fileName];
                        fileCount[fileName] = count + 1;
                        fileName = $"{Path.GetFileNameWithoutExtension(file.FileName)} ({count}){Path.GetExtension(file.FileName)}";
                    }
                    else
                    {
                        fileCount.Add(fileName, 1);
                    }
                    path = await _MinioDomainService.PutObjectAsync(list.rno, fileName, steam, type,area);
                }
                list.SetOriginalFileNameAndSavePath(file.Name, path, fileName, type);
            }
        }
        CashHead TransformAndSetData(IFormCollection formCollection, string user)
        {
            string head = formCollection["head"];
            string detail = formCollection["detail"];
            string file = formCollection["file"];
            string inv = formCollection["inv"];
            string amount = formCollection["amount"];
            CashHeadDto hData = JsonConvert.DeserializeObject<CashHeadDto>(head);
            IList<CashDetailDto> dData = JsonConvert.DeserializeObject<IList<CashDetailDto>>(detail);
            IList<CashFileDto> fData = JsonConvert.DeserializeObject<IList<CashFileDto>>(file);
            IList<InvoiceDto> iData = JsonConvert.DeserializeObject<IList<InvoiceDto>>(inv);
            CashAmountDto aData = JsonConvert.DeserializeObject<CashAmountDto>(amount);
            CashHead list = _ObjectMapper.Map<CashHeadDto, CashHead>(hData);
            IList<CashDetail> detailData = _ObjectMapper.Map<IList<CashDetailDto>, IList<CashDetail>>(dData);
            IList<CashFile> fileData = _ObjectMapper.Map<IList<CashFileDto>, IList<CashFile>>(fData);
            IList<Invoice> invData = _ObjectMapper.Map<IList<InvoiceDto>, IList<Invoice>>(iData);
            CashAmount amountData = _ObjectMapper.Map<CashAmountDto, CashAmount>(aData);
            list.SetCash1(list, user);
            list.AddCash1Detail(detailData);
            list.AddFile(fileData);
            list.AddInvoice(invData);
            list.AddCashAmount(amountData);
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
            await _InvoiceRepository.DeleteManyAsync(await _InvoiceRepository.GetByNo(list.rno));
            await _CashFileRepository.DeleteManyAsync(await _CashFileRepository.GetByNo(list.rno));
            await _CashAmountRepository.DeleteAsync(await _CashAmountRepository.GetByNo(list.rno));
            List<ApprovalPaper> eFormPapers = (await _EFormPaperRepository.GetByNo(list.rno)).ToList();
            if (eFormPapers.Count > 0)
            {
                await _EFormPaperRepository.DeleteManyAsync(eFormPapers);
            }
        }
        async Task<bool> InsertData(CashHead list)
        {
            try
            {
                await _CashHeadRepository.InsertAsync(list);
                await _CashDetailRepository.InsertManyAsync(list.CashDetailList);
                await _InvoiceRepository.InsertManyAsync(list.InvoiceList);
                await _CashFileRepository.InsertManyAsync(list.CashFileList);
                await _CashAmountRepository.InsertAsync(list.CashAmount);
                return true;
            }
            catch (Exception ex) {
                return false;
            }
        }
        //public async Task<TravelDto> GetMealExpense(TravelDto data) => await _BDMealAreaRepository.GetExpense(data);
        public async Task<Result<TravelDto>> GetMealExpense(TravelDto data)
        {
            Result<TravelDto> result = new();
            result.data = await _BDMealAreaRepository.GetExpense(data);
            return result;
        }
        public async Task<Result<List<SelfDriveCostDto>>> GetSelfDriveCostListFromExcel(IFormFile excelFile, string company)
        {
            Result<List<SelfDriveCostDto>> results = new Result<List<SelfDriveCostDto>>()
            {
                data = new List<SelfDriveCostDto>()
            };
            DataTable dt = NPOIHelper.GetDataTableFromExcel(excelFile);
            List<string> expnamepquery = (await _bdexpRepository.WithDetailsAsync()).Where(y => y.company == company).Select(x => x.expname).ToList();
            List<string> cartypequery = (await _bdcarRepository.WithDetailsAsync()).Select(x => x.name).Distinct().ToList();
            var carvaluequery = (await _bdcarRepository.WithDetailsAsync()).ToList();
            var list = dt.Rows.Cast<DataRow>().Select((s, i) => new
            {
                deptid = s[0].ToString().Trim(),
                departureplace = s[1].ToString().Trim(),
                cdate = s[2].ToString().Trim(),
                vehicletype = s[3].ToString().Trim(),
                kilometers = s[4].ToString().Trim(),
                summary = s[5].ToString().Trim()
            }).ToList();
            var emporgquery = (await _emporgRepository.WithDetailsAsync()).Where(w => list.Select(s => s.deptid).Contains(w.deptid)).Select(x => new { x.deptid, x.tree_level_num }).ToList();
            foreach (var item in list)
            {
                SelfDriveCostDto result = new SelfDriveCostDto();
                // if (expnamepquery.Contains(item.expname))
                // {
                //     result.expname = item.expname;
                //     result.expcode = (await _bdexpRepository.WithDetailsAsync()).Where(y => y.expname == item.expname).Select(x => x.expcode).FirstOrDefault();
                // }
                // else
                // {
                //     results.message = L["NoReimbursementScene"];
                //     results.status = 2;
                //     return results;
                // }
                if (emporgquery.Select(s => s.deptid).Contains(item.deptid))
                {
                    if (!String.IsNullOrEmpty(item.deptid) && emporgquery.Where(s => s.deptid == item.deptid).FirstOrDefault().tree_level_num > 7)
                    {
                        results.message = String.Format(L["ExpDeptidNotMinisterial"], item.deptid);
                        results.data = null;
                        results.status = 2;
                        return results;
                    }
                    result.deptid = item.deptid;
                }
                else
                {
                    results.message = String.Format(L["NoExpDeptid"], item.deptid);
                    results.data = null;
                    results.status = 2;
                    return results;
                }
                result.departureplace = item.departureplace;
                result.summary = item.summary;
                DateTime dtDate;
                if (DateTime.TryParse(item.cdate, out dtDate))
                {
                    result.cdate = dtDate;
                }
                else
                {
                    results.message = String.Format(L["IncorrectDate"], item.cdate);
                    results.data = null;
                    results.status = 2;
                    return results;
                }
                if (cartypequery.Contains(item.vehicletype))
                {
                    result.vehicletype = item.vehicletype;
                    result.vehiclevalue = carvaluequery.Where(x => x.name == item.vehicletype).Select(x => x.type).FirstOrDefault();
                }
                else
                {
                    results.message = L["NoVehicleType"];
                    results.data = null;
                    results.status = 2;
                    return results;
                }
                decimal k;
                if (Decimal.TryParse(item.kilometers, out k))
                {
                    if (k <= 0)
                    {
                        results.message = L["IncorrectKilometers"];
                        results.data = null;
                        results.status = 2;
                        return results;
                    }
                    result.kilometers = k;
                }
                else
                {
                    results.message = L["IncorrectKilometers"];
                    results.data = null;
                    results.status = 2;
                    return results;
                }
                result.total = carvaluequery.Where(x => x.name == item.vehicletype).Distinct().Select(x => x.amount).FirstOrDefault() * result.kilometers;
                result.rate = 1;
                results.data.Add(result);
            }
            return results;
        }
        public async Task<Result<List<OvertimeMealDto>>> GetOvertimeMealCostListFromExcel(IFormFile excelFile, string company)
        {
            Result<List<OvertimeMealDto>> results = new Result<List<OvertimeMealDto>>()
            {
                data = new List<OvertimeMealDto>()
            };
            DataTable dt = NPOIHelper.GetDataTableFromExcel(excelFile);
            var bdexpquery = (await _bdexpRepository.WithDetailsAsync()).Where(y => y.company == company).Select(x => new { x.expcode, x.expname }).ToList();
            List<string> expnamelist = bdexpquery.Select(x => x.expname).ToList();
            string basecurr = (await _CompanyRepository.WithDetailsAsync()).Where(x => x.company == company).Select(x => x.BaseCurrency).FirstOrDefault();
            var list = dt.Rows.Cast<DataRow>().Select((s, i) => new
            {
                deptid = s[0].ToString().Trim(),
                businesscity = s[1].ToString().Trim(),
                cdate = s[2],
                departuretime = s[3],
                backtime = s[4],
                summary = s[5].ToString().Trim()
            }).ToList();
            var emporgquery = (await _emporgRepository.WithDetailsAsync()).Where(w => list.Select(s => s.deptid).Contains(w.deptid)).Select(x => new { x.deptid, x.tree_level_num }).ToList();
            List<string> citylist = list.Select(x => x.businesscity).ToList();
            var data = (await _BDMealAreaRepository.WithDetailsAsync()).Where(x => citylist.Contains(x.city)).ToList();
            foreach (var item in list)
            {
                OvertimeMealDto result = new OvertimeMealDto();
                // //报销场景校验
                // if (expnamelist.Contains(item.expname))
                // {
                //     result.expname = item.expname;
                //     result.expcode = bdexpquery.Where(y => y.expname == item.expname).Select(x => x.expcode).FirstOrDefault();
                // }
                // else
                // {
                //     results.message = L["NoReimbursementScene"] + "：" + item.expname;
                //     results.data = null;
                //     results.status = 2;
                //     return results;
                // }
                //费用归属部门校验
                if (emporgquery.Select(w => w.deptid).Contains(item.deptid))
                {
                    if (!String.IsNullOrEmpty(item.deptid) && emporgquery.Where(s => s.deptid == item.deptid).FirstOrDefault().tree_level_num > 7)
                    {
                        results.message = String.Format(L["ExpDeptidNotMinisterial"], item.deptid);
                        results.data = null;
                        results.status = 2;
                        return results;
                    }
                    result.deptid = item.deptid;
                }
                else
                {
                    results.message = String.Format(L["NoExpDeptid"], item.deptid);
                    results.data = null;
                    results.status = 2;
                    return results;
                }
                //费用发生日期校验
                DateTime dtDate;
                if (DateTime.TryParse(item.cdate.ToString(), out dtDate))
                {
                    result.cdate = dtDate;
                }
                else
                {
                    results.message = String.Format(L["IncorrectDate"], item.cdate);
                    results.data = null;
                    results.status = 2;
                    return results;
                }
                //出发时点
                DateTime startTime;
                if (DateTime.TryParse(item.departuretime.ToString(), out startTime))
                {
                    result.departuretime = startTime;
                }
                else
                {
                    results.message = String.Format(L["IncorrectDate"], item.departuretime);
                    results.data = null;
                    results.status = 2;
                    return results;
                }
                //回到时点
                DateTime endTime;
                if (DateTime.TryParse(item.backtime.ToString(), out endTime))
                {
                    result.backtime = endTime;
                }
                else
                {
                    results.message = String.Format(L["IncorrectDate"], item.backtime);
                    results.data = null;
                    results.status = 2;
                    return results;
                }
                //摘要
                result.summary = item.summary;
                //公出城市
                result.businesscity = item.businesscity;
                List<BDMealArea> bDMealAreas = data.Where(x => x.city == item.businesscity).ToList();
                if (bDMealAreas.Count == 0)
                {
                    results.message = L["NoCity"] + "：" + item.businesscity;
                    results.data = null;
                    results.status = 2;
                    return results;
                }
                TravelDto travelDto = new TravelDto()
                {
                    city = item.businesscity,
                    gotime = Convert.ToDateTime(result.cdate.ToString("yyyy/MM/dd") + " " + result.departuretime.ToString("HH:mm")),
                    backtime = Convert.ToDateTime(result.cdate.ToString("yyyy/MM/dd") + " " + result.backtime.ToString("HH:mm")),
                    company = company
                };
                TravelDto calresult = _BDMealAreaRepository.TravelCount(bDMealAreas, travelDto);
                if (calresult == null && calresult.amount == 0)
                {
                    result.currency = basecurr;
                }
                else
                {
                    result.amount = calresult.amount;
                    result.currency = calresult.currency;
                }
                if (calresult.currency == basecurr)
                {
                    result.rate = 1;
                    result.baseamt = result.amount;
                }
                else if (calresult.currency != null)
                {
                    result.rate = (await _CurrencyDomainService.queryRate(result.currency, basecurr)).ccurrate;
                    result.baseamt = result.amount * result.rate;
                }
                results.data.Add(result);
            }
            return results;
        }
        //根据公司别获取城市
        public async Task<Result<IList<string>>> GetCityByCompany(string company)
        {
            Result<IList<string>> result = new();
            IList<string> query = await _BDMealAreaRepository.GetCity(company);
            result.data = query;
            result.total = query.Count;
            return result;
        }
    }
}
