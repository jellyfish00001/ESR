using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.Application;
using ERS.DTO.BDInvoiceFolder;
using ERS.Entities;
using ERS.IDomainServices;
using ERS.IRepositories;
using ERS.Minio;
using Microsoft.EntityFrameworkCore;
using Microsoft.International.Converters.TraditionalChineseToSimplifiedConverter;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;
namespace ERS.DomainServices
{
    public class BDInvoiceFolderDomainService : CommonDomainService, IBDInvoiceFolderDomainService
    {
        private IBDInvoiceFolderRepository _BDInvoiceFolderRepository;
        private IBDInvoiceTypeRepository _BDInvoiceTypeRepository;
        private IMinioRepository _MinioRepository;
        private IObjectMapper _ObjectMapper;
        private IInvoiceRepository _InvoiceRepository;
        private ICompanyRepository _companyRepository;
        private IRepository<SAPExchRate, Guid> _SAPExchRateRepository;
        private IMinioDomainService _minioDomainService;
        public BDInvoiceFolderDomainService(
            IBDInvoiceFolderRepository iBDInvoiceFolderRepository,
            IMinioRepository iMinioRepository,
            IObjectMapper iObjectMapper,
            IInvoiceRepository iInvoiceRepository,
            ICompanyRepository iCompanyRepository,
            IRepository<SAPExchRate, Guid> iSAPExchRateRepository,
            IBDInvoiceTypeRepository iBDInvoiceTypeRepository,
            IMinioDomainService minioDomainService,
            IOcrResultRepository iOcrResultRepository
        )
        {
            _BDInvoiceFolderRepository = iBDInvoiceFolderRepository;
            _MinioRepository = iMinioRepository;
            _ObjectMapper = iObjectMapper;
            _InvoiceRepository = iInvoiceRepository;
            _companyRepository = iCompanyRepository;
            _SAPExchRateRepository = iSAPExchRateRepository;
            _BDInvoiceTypeRepository = iBDInvoiceTypeRepository;
            _minioDomainService = minioDomainService;
        }
        public async Task<Result<List<BDInvoiceFolderDto>>> GetPageInvInfo(Request<QueryBDInvoiceFolderDto> request, string userId)
        {
            Result<List<BDInvoiceFolderDto>> result = new Result<List<BDInvoiceFolderDto>>()
            {
                data = new List<BDInvoiceFolderDto>()
            };

            List<BDInvoiceFolder> invFolderList = (await _BDInvoiceFolderRepository.WithDetailsAsync())
                                .Where(w => w.emplid == userId)
                                .WhereIf(request.data.startdate.HasValue, q => q.cdate.Value.Date >= Convert.ToDateTime(request.data.startdate).ToLocalTime().Date)
                                .WhereIf(request.data.enddate.HasValue, q => q.cdate.Value.Date <= Convert.ToDateTime(request.data.enddate).ToLocalTime().Date)
                                .WhereIf(!String.IsNullOrEmpty(request.data.invno), q => q.invno == request.data.invno)
                                .WhereIf(!String.IsNullOrEmpty(request.data.invtype), q => q.invtype == request.data.invtype)
                                .WhereIf(!String.IsNullOrEmpty(request.data.verifytype), q => q.verifytype == request.data.verifytype)
                                .WhereIf(!String.IsNullOrEmpty(request.data.paytype), q => q.paytype == request.data.paytype)
                                .OrderBy(dd => dd.invdate)
                                .ToList();
            // Get all related invoice types
            var invoiceTypes = await _BDInvoiceTypeRepository.GetBDInvoiceTypeList();

            //发票查询需要带上发票类型确定哪些栏位可供查看或编辑
            List<BDInvoiceFolderDto> bDInvoiceFolderDtos = _ObjectMapper.Map<List<BDInvoiceFolder>, List<BDInvoiceFolderDto>>(invFolderList);

            foreach (var item in bDInvoiceFolderDtos)
            {
                // Get matching invoice type based on invtype
                var matchingInvoiceType = invoiceTypes.FirstOrDefault(o => o.InvType == item.invtype);
                // Assign category if found
                if (matchingInvoiceType != null)
                {
                    item.invoicecategory = matchingInvoiceType.Category;
                }

                var originalFolder = invFolderList.FirstOrDefault(f => f.Id == item.Id);
                if (originalFolder != null)
                {
                    item.salestaxno = originalFolder.sellertaxid;
                    item.buyertaxno = originalFolder.buyertaxid;
                    item.oamount = originalFolder.amount;
                    item.amount = originalFolder.untaxamount;
                    item.taxamount = originalFolder.taxamount;
                }

                // Get presigned URL if filepath exists
                if (!String.IsNullOrEmpty(item.filepath))
                {
                    string area = await _minioDomainService.GetMinioArea(userId);
                    item.url = await _MinioRepository.PresignedGetObjectAsync(item.filepath, area);
                }
            }

            int pageIndex = request.pageIndex;
            int pageSize = request.pageSize;
            if (pageIndex < 1 || pageSize < 0)
            {
                pageIndex = 1;
                pageSize = 10;
            }
            if (request.data.isphone)
            {
                result.data = bDInvoiceFolderDtos;
            }
            else
            {
                result.data = bDInvoiceFolderDtos.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
            result.total = bDInvoiceFolderDtos.Count;
            return result;
        }
        public async Task<Result<List<string>>> GetInvPayTypes()
        {
            Result<List<string>> result = new();
            result.data = await _BDInvoiceFolderRepository.GetPayTypeList();
            return result;
        }
        public async Task<Result<string>> DeleteInvInfo(Guid id,string userId)
        {
            Result<string> result = new Result<string>();
            string area = await _minioDomainService.GetMinioArea(userId);
            var query = await _BDInvoiceFolderRepository.GetInvInfoById(id);
            if (query == null)
            {
                result.status = 2;
                result.message = L["DeleteFail"] + "：" + L["BDInvoiceFolder-NotFound"];
                return result;
            }
            if ((query.paytype == "已請款" || query.paytype == "已请款") && (query.verifytype == "已校驗" || query.verifytype == "已校验"))
            {
                result.status = 2;
                result.message = L["DeleteFail"] + "：" + L["BDInvoiceFolder-Delete"];
                return result;
            }
            await _BDInvoiceFolderRepository.DeleteAsync(query);

            if (!String.IsNullOrEmpty(query.filepath))
            {
                await _MinioRepository.RemoveObjectAsync(query.filepath,area);
            }
            result.message = L["DeleteSuccess"];
            return result;
        }
        public async Task<Result<string>> EditInvInfo(InvoiceDto request, string userId)
        {
            Result<string> result = new Result<string>();
            BDInvoiceFolder bDInvoiceFolder = await _BDInvoiceFolderRepository.GetInvInfoById(request.Id);
            var repeatInvList = (await _BDInvoiceFolderRepository.WithDetailsAsync()).Where(w => w.Id != request.Id).ToList();
            //信息為空
            if (bDInvoiceFolder == null)
            {
                result.message = L["SaveFailMsg"] + L["BDInvoiceFolder-NotFound"];
                result.status = 2;
                return result;
            }
            //if (bDInvoiceFolder.isfill == false)
            //{
            //    result.message = L["SaveFailMsg"] + L["BDInvoiceFolder-Edit"];
            //    result.status = 2;
            //    return result;
            //}
            //检查编辑后的发票信息是否与已存在的发票一致
            if (!String.IsNullOrEmpty(request.invcode) || !String.IsNullOrEmpty(request.invno))
            {
                if (repeatInvList.Where(w => w.invcode == request.invcode && w.invno == request.invno).Count() > 0)
                {
                    result.message = L["SaveFailMsg"] + L["BDInvoiceFolder-Edit-Repeat"];
                    result.status = 2;
                    return result;
                }
            }
            bDInvoiceFolder.invcode = request.invcode;
            bDInvoiceFolder.invno = request.invno;
            bDInvoiceFolder.invdate = request.invdate;
            bDInvoiceFolder.invtype = request.invtype;
            bDInvoiceFolder.currency = request.curr;
            bDInvoiceFolder.amount = request.oamount;
            bDInvoiceFolder.untaxamount = request.amount;
            bDInvoiceFolder.taxamount = request.taxamount;
            bDInvoiceFolder.startstation = request.startstation;
            bDInvoiceFolder.endstation = request.endstation;
            bDInvoiceFolder.buyertaxid = request.buyertaxno;
            bDInvoiceFolder.sellertaxid = request.salestaxno;
            bDInvoiceFolder.invoicetitle = request.invoicetitle;
            bDInvoiceFolder.taxbase = (decimal)request.taxbase;
            bDInvoiceFolder.importtaxamount = (decimal)request.importtaxamount;
            bDInvoiceFolder.servicefee = (decimal)request.servicefee;
            bDInvoiceFolder.shippingfee = (decimal)request.shippingfee;
            bDInvoiceFolder.transactionfee = (decimal)request.transactionfee;
            bDInvoiceFolder.quantity = (decimal)request.quantity;
            bDInvoiceFolder.productinfo = request.productinfo;
            bDInvoiceFolder.remarks = request.remarks;
            bDInvoiceFolder.abnormalreason = request.abnormalreason;
            bDInvoiceFolder.responsibleparty = request.responsibleparty;
            bDInvoiceFolder.taxtype = request.taxtype;

            bDInvoiceFolder.muser = userId;
            bDInvoiceFolder.isedit = await _InvoiceRepository.CheckIsExistFolder(request.Id);
            bDInvoiceFolder.mdate = System.DateTime.Now;
            result.message = L["SaveSuccessMsg"];
            await _BDInvoiceFolderRepository.UpdateAsync(bDInvoiceFolder);
            return result;
        }
        //獲取個人未請款發票信息
        public async Task<Result<List<UnpaidInvInfoDto>>> GetUnpaidInvInfo(string userId, string company)
        {
            Result<List<UnpaidInvInfoDto>> results = new();
            string area = await _minioDomainService.GetMinioArea(userId);
            var query = (await _BDInvoiceFolderRepository.WithDetailsAsync()).Where(w => w.emplid == userId && w.paytype == "unrequested").AsNoTracking().ToList();
            //原本由买方公司名称与申请者公司别的公司名称比较，现改成发票上买方税号与公司别里设定的识别号比较
            //string companydesc = await (await _companyRepository.WithDetailsAsync()).Where(i => i.CompanyCategory == company).Select(i => i.CompanyDesc).FirstOrDefaultAsync();
            //if (!String.IsNullOrEmpty(companydesc))
            //{
            //    companydesc = ChineseConverter.Convert(companydesc, ChineseConversionDirection.TraditionalToSimplified);
            //}
            string buyerTaxid = await (await _companyRepository.WithDetailsAsync()).Where(i => i.CompanyCategory == company).Select(i => i.IdentificationNo).FirstOrDefaultAsync();

            var result = _ObjectMapper.Map<List<BDInvoiceFolder>, List<UnpaidInvInfoDto>>(query);
            string basecurr = (await _companyRepository.WithDetailsAsync()).Where(w => w.company == company).FirstOrDefault()?.BaseCurrency;
            DateTime current = DateTime.Now.AddDays(-Convert.ToInt32(DateTime.Now.Date.Day));
            var exRateList = (await _SAPExchRateRepository.WithDetailsAsync()).Where(w => result.Select(s => s.curr).Contains(w.ccurfrom) && w.ccurdate > current).ToList();
            //result = result.Where(i => (String.IsNullOrEmpty(i.paymentName?.Replace("(", "（").Replace(")", "）")) ? i.paymentName?.Replace("(", "（").Replace(")", "）") : ChineseConverter.Convert(i.paymentName?.Replace("(", "（").Replace(")", "）"), ChineseConversionDirection.TraditionalToSimplified)) == companydesc || string.IsNullOrEmpty(i.paymentName) || (!String.IsNullOrEmpty(i.paymentName) ? ChineseConverter.Convert(i.paymentName, ChineseConversionDirection.TraditionalToSimplified) : i.paymentName) == companydesc.Replace("（", "").Replace("）", "")).ToList();
            result = result.Where(x => string.IsNullOrEmpty(x.paymentNo) || (!string.IsNullOrEmpty(x.paymentNo) && x.paymentNo == buyerTaxid)).ToList();
            foreach (var item in result)
            {
                if (!String.IsNullOrEmpty(item.expdesc))
                {
                    item.taxloss = Math.Round(item.oamount * Convert.ToDecimal(0.25), 2, MidpointRounding.AwayFromZero);
                    item.expcode = (L["001"] + item.invno + L["002"] + item.expdesc + L["003"]);
                }
                if (!String.IsNullOrEmpty(item.filepath))
                {
                    item.fileurl = await _MinioRepository.PresignedGetObjectAsync(item.filepath, area);
                }
                item.baseamt = item.oamount;
                if (basecurr != item.curr && !String.IsNullOrEmpty(basecurr))
                {
                    decimal? rate = exRateList.Where(w => w.ccurfrom == item.curr && w.ccurto == basecurr).OrderByDescending(s => s.date_fm).FirstOrDefault()?.ccurrate;
                    if (rate.HasValue)
                    {
                        item.baseamt = item.oamount * rate;
                    }
                }
            }
            results.data = result;
            results.total = result.Count;
            return results;
        }
    }
}