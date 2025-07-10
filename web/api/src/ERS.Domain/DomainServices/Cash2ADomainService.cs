using System.Linq;
using ERS.IDomainServices;
using ERS.DTO;
using ERS.DTO.Application;
using ERS.Entities;
using ERS.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;
using System;
using ERS.Localization;
using ERS.Domain.IDomainServices;
using ERS.Application.Contracts.DTO.Application;
using System.IO;
using ERS.DTO.Invoice;
namespace ERS.DomainServices
{
    public class Cash2ADomainService : CommonDomainService, ICash2ADomainService
    {
        private IObjectMapper _ObjectMapper;
        private IAutoNoRepository _AutoNoRepository;
        private IUnitOfWorkManager _UnitOfWorkManager;
        private ICashHeadRepository _CashHeadRepository;
        private ICashDetailRepository _CashDetailRepository;
        private ICashFileRepository _CashFileRepository;
        private ICashAccountRepository _CashAccountRepository;
        private IBDExpDomainService _BDExpDomainService;
        private IInvoiceRepository _InvoiceRepository;
        private ICashAmountRepository _CashAmountRepository;
        private IMinioDomainService _MinioDomainService;
        private IEFormHeadRepository _EFormHeadRepository;
        private IConfiguration _Configuration;
        private IApprovalPaperRepository _EFormPaperRepository;
        private IApprovalPaperDomainService _EFormPaperDomainService;
        private IApprovalDomainService _ApprovalDomainService;
        private IEFormAlistRepository _EFormAlistRepository;
        private IEFormAuserRepository _EFormAuserRepository;
        Type LocalizationResource { get; set; }
        private IBDInvoiceFolderRepository _BDInvoiceFolderRepository;
        private IConfiguration _configuration;
        private IInvoiceDomainService _InvoiceDomainService;
        private IMinioDomainService _minioDomainService;
        private IApprovalFlowDomainService _iApprovalFlowDomainService;
        public Cash2ADomainService(IUnitOfWorkManager UnitOfWorkManager, IAutoNoRepository AutoNoRepository, IObjectMapper ObjectMapper,
         ICashHeadRepository CashHeadRepository, ICashDetailRepository CashDetailRepository, ICashFileRepository CashFileRepository,
          ICashAccountRepository CashAccountRepository, IBDExpDomainService BDExpDomainService, IInvoiceRepository InvoiceRepository, IApprovalPaperRepository EFormPaperRepository,
          ICashAmountRepository CashAmountRepository, IMinioDomainService MinioDomainService, IEFormHeadRepository EFormHeadRepository,
           IConfiguration Configuration, IInvoiceDomainService InvoiceDomainService,IApprovalPaperDomainService EFormPaperDomainService,
           IApprovalDomainService ApprovalDomainService, IEFormAlistRepository EFormAlistRepository, IEFormAuserRepository EFormAuserRepository,
           IBDInvoiceFolderRepository BDInvoiceFolderRepository, IConfiguration configuration, IMinioDomainService minioDomainService, IApprovalFlowDomainService iApprovalFlowDomainService)
        {
            //IInvoiceDomainService InvoiceDomainService
            _InvoiceDomainService = InvoiceDomainService;
            _ObjectMapper = ObjectMapper;
            _AutoNoRepository = AutoNoRepository;
            _UnitOfWorkManager = UnitOfWorkManager;
            _CashHeadRepository = CashHeadRepository;
            _CashDetailRepository = CashDetailRepository;
            _CashFileRepository = CashFileRepository;
            _CashAccountRepository = CashAccountRepository;
            _BDExpDomainService = BDExpDomainService;
            _InvoiceRepository = InvoiceRepository;
            _CashAmountRepository = CashAmountRepository;
            _MinioDomainService = MinioDomainService;
            _EFormHeadRepository = EFormHeadRepository;
            _Configuration = Configuration;
            LocalizationResource = typeof(ERSResource);
            _EFormPaperDomainService = EFormPaperDomainService;
            _ApprovalDomainService = ApprovalDomainService;
            _EFormAlistRepository = EFormAlistRepository;
            _EFormAuserRepository = EFormAuserRepository;
            _BDInvoiceFolderRepository = BDInvoiceFolderRepository;
            _configuration = configuration;
            _EFormPaperRepository = EFormPaperRepository;
            _minioDomainService = minioDomainService;
            _iApprovalFlowDomainService = iApprovalFlowDomainService;
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
            ExpAcctDto expacct = await _BDExpDomainService.GetExpAcct(list.CashDetailList[0].expcode, list.company);
            list.SetDetailExp(expacct);
            list.SetPayeeAccount(await _CashAccountRepository.GetAccount(list.payeeId));
            if (status == "T")
                list.SetKeepStatus("RQ201A");
            if (!list.CheckIsExistRno())
            {
                string rno = await _AutoNoRepository.CreateCash2No();
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
            await SaveInvoiceFolder(list, Iids);
            //检查发票是否可请款
            Result<CashResult> invoviceResult = await _InvoiceDomainService.checkInvoicePaid(list, token);
            if (invoviceResult.status == 2)
            {
                return invoviceResult;
            }
            if (status.Equals("P"))
                await _BDInvoiceFolderRepository.InsertRno(Iids, list.rno);
            // file
            await SaveFile(formCollection, list, Iids);
            cash.Stat = false;
            if (status.Equals("P") && invoviceResult.data.Stat)
            {
                await _EFormPaperDomainService.addPaper(list.rno, list.formcode, user, list.company);
                cash.Stat = true;
            }
            bool saveFlag = await InsertData(list);
            if (saveFlag)
            {
                if (status.Equals("P"))
                {
                    //await _ApprovalDomainService.CreateSignSummary(list, invoviceResult.data.ElecInvStat, token);
                    //新签核flow
                    Result<string> signFlowResult = await _iApprovalFlowDomainService.ApplyApprovalFlow(list, list.CashDetailList.ToList());

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
                }
            }
            cash.rno = list.rno;
            result.data=cash;
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
            //List<BDInvoiceFolder> invoices = await _BDInvoiceFolderRepository.GetInvInfoListById(ids);
            //foreach (var invoice in invoices)
            //    if (!string.IsNullOrEmpty(invoice.filepath)) await _MinioDomainService.CopyInvoiceAsync(list.rno, invoice.filepath);
            string area = await _minioDomainService.GetMinioArea(list.cuser);
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
                    path = await _MinioDomainService.PutObjectAsync(list.rno, fileName, steam, type, area);
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
            list.SetCash2A(list, user);
            list.AddCash2ADetail(detailData);
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
            if(eFormPapers.Count > 0)
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
    }
}
