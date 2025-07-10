using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERS.Domain.IDomainServices;
using Volo.Abp.Domain.Repositories;
using ERS.Entities;
using ERS.DTO.Application;
using ERS.IRepositories;
using Volo.Abp.ObjectMapping;
using ERS.Minio;
using ERS.DTO;
using ERS.IDomainServices;
using ERS.OracleToPostgreSQL;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;

namespace ERS.DomainServices
{
    public class ApplicationDomainService : IApplicationDomainService
    {
        private ICashHeadRepository _cashheadRepository;
        private ICashDetailRepository _cashdetailRepository;
        private ICashAmountRepository _cashamountRepository;
        private ICashFileRepository _cashfileRepository;
        private IInvoiceRepository _invoiceRepository;
        private IMinioRepository _MinioRepository;
        private IEmpOrgRepository _EmpOrgRepository;
        private ICompanyRepository _companyRepository;
        private IRepository<SAPExchRate, Guid> _SAPExchRateRepository;
        private IObjectMapper _objectMapper;
        private IMinioDomainService _minioDomainService;
        private IBDInvoiceFolderRepository _iBDInvoiceFolderRepository;
        private IConfiguration _Configuration;
        private ICorporateRegistrationRepository _corporateRegistrationRepository;
        public ApplicationDomainService(
            ICashHeadRepository cashheadRepository,
            ICashDetailRepository cashdetailRepository,
            ICashAmountRepository cashamountRepository,
            ICashFileRepository cashfileRepository,
            IInvoiceRepository invoiceRepository,
            ICompanyRepository companyRepository,
            IRepository<SAPExchRate, Guid> SAPExchRateRepository,
            IObjectMapper ObjectMapper,
            IEmpOrgRepository EmpOrgRepository,
            IMinioDomainService minioDomainService,
            IMinioRepository MinioRepository,
            IBDInvoiceFolderRepository iBDInvoiceFolderRepository,
            IConfiguration iConfiguration,
            ICorporateRegistrationRepository corporateRegistrationRepository



        )
        {
            _objectMapper = ObjectMapper;
            _cashheadRepository = cashheadRepository;
            _cashdetailRepository = cashdetailRepository;
            _cashamountRepository = cashamountRepository;
            _cashfileRepository = cashfileRepository;
            _invoiceRepository = invoiceRepository;
            _companyRepository = companyRepository;
            _SAPExchRateRepository = SAPExchRateRepository;
            _MinioRepository = MinioRepository;
            _EmpOrgRepository = EmpOrgRepository;
            _minioDomainService = minioDomainService;
            _iBDInvoiceFolderRepository = iBDInvoiceFolderRepository;
            _Configuration = iConfiguration;
            _corporateRegistrationRepository = corporateRegistrationRepository;
        }
        public async Task<Result<ApplicationDto>> QueryApplications(string rno)
        {
            Result<ApplicationDto> results = new();
            ApplicationDto result = new ApplicationDto();
            CashHead headquery = await _cashheadRepository.GetByNo(rno);
            if (headquery == null)
            {
                return results;
            }
            List<CashDetail> detailquery = (await _cashdetailRepository.GetByNo(rno)).OrderBy(i => i.seq).ToList();
            List<CashFile> filequery = (await _cashfileRepository.GetByNo(rno)).ToList();
            List<Invoice> invoicequery = (await _invoiceRepository.GetByNo(rno)).ToList();
            CashAmount amountquery = (await _cashamountRepository.GetByNo(rno));
            List<CashFile> nfilequery = filequery.Where(f => f.ishead != "Y").ToList();
            List<CashFile> yfilequery = filequery.Where(f => f.ishead == "Y").ToList();
            DateTime current = DateTime.Now.AddDays(-Convert.ToInt32(DateTime.Now.Date.Day));
            var basecurr = (await _companyRepository.WithDetailsAsync()).FirstOrDefault(w => w.company == headquery.company)?.BaseCurrency;
            //var exRateList = (await _SAPExchRateRepository.WithDetailsAsync()).Where(w => w.ccurdate > current).AsNoTracking().ToList();
            CashHeadDto headData = _objectMapper.Map<CashHead, CashHeadDto>(headquery);
            List<CashDetailDto> detailsData = _objectMapper.Map<List<CashDetail>, List<CashDetailDto>>(detailquery);
            List<CashFileDto> nfilesData = _objectMapper.Map<List<CashFile>, List<CashFileDto>>(nfilequery);// details file
            List<CashFileDto> yfilesData = _objectMapper.Map<List<CashFile>, List<CashFileDto>>(yfilequery);// head file
            List<InvoiceDto> invoicesData = _objectMapper.Map<List<Invoice>, List<InvoiceDto>>(invoicequery);
            CashAmountDto amountData = _objectMapper.Map<CashAmount, CashAmountDto>(amountquery);
            headData.costdeptid = await _EmpOrgRepository.GetCostDeptid(headData.deptid, headData.company);
            string area=await _minioDomainService.GetMinioArea(headData.cuser);
            // //特殊处理msg类型文件的filetype
            // foreach (var tempFile in nfilesData)
            // {
            //     if (tempFile.filename.EndsWith(".msg"))
            //     {
            //         tempFile.filetype = "application/octet-stream";
            //     }
            // }
            // foreach (var tempFile in yfilesData)
            // {
            //     if (tempFile.filename.EndsWith(".msg"))
            //     {
            //         tempFile.filetype = "application/octet-stream";
            //     }
            // }
            foreach (var item in invoicesData)
            {
                var dto = nfilesData.FirstOrDefault(w => w.seq == item.seq && w.item == item.item && w.status != "F");
                if(headquery.formcode == "CASH_4")
                {
                    dto = yfilesData.FirstOrDefault(w => w.seq == item.seq && w.item == item.item && w.status != "F");
                }
                item.invtype = dto?.category;
                item.fileurl = !string.IsNullOrEmpty(dto?.path) ? await _MinioRepository.PresignedGetObjectAsync(dto.path,area) : "";
            }
            foreach (var item in detailsData)
            {
                item.fileList = nfilesData.Where(i => i.seq == item.seq).OrderBy(i => i.item).ToList();
                if(item.formcode == "CASH_4")
                {
                    item.fileList = yfilesData.Where(i => i.seq == item.seq).OrderBy(i => i.item).ToList();
                }
                // if (item.formcode != "CASH_4")
                // {
                    // 发票信息
                    item.invList = invoicesData.Where(i => i.seq == item.seq).OrderBy(i => i.item).ToList();
                    foreach (var item1 in item.invList)
                    {
                        var dto = item.fileList.Where(w => w.item == item1.item && w.status != "F").FirstOrDefault();
                        item1.invtype = dto?.category;
                        item1.baseamt = item1.oamount;
                        if (basecurr != item1.curr && !String.IsNullOrEmpty(basecurr))
                        {
                            decimal? rate = (await _SAPExchRateRepository.WithDetailsAsync()).Where(w => w.ccurfrom == item1.curr && w.ccurto == basecurr && w.ccurdate > current).OrderBy(s => s.ccurdate).FirstOrDefault()?.ccurrate;
                            if (rate.HasValue)
                            {
                                item1.baseamt = item1.baseamt * rate;
                            }
                        }
                        //item1.fileurl = !String.IsNullOrEmpty(dto?.path) ? await _MinioRepository.PresignedGetObjectAsync(dto.path) : "";
                    }
                //}
                // 附件及发票文件
                foreach (var file in item.fileList)
                {
                    file.url = !String.IsNullOrEmpty(file.path) ? await _MinioRepository.PresignedGetObjectAsync(file.path, area) : "";
                    var invInfo = item.invList.Where(w => w.seq == file.seq && w.item == file.item && file.status != "F").FirstOrDefault();
                    file.invoiceid = invInfo?.invoiceid;
                    file.invoiceno = invInfo?.invno;
                }
                item.cdate = await _cashdetailRepository.GetDateByAdvrno(item.advancerno);
            }
            foreach (var item in yfilesData)
            {
                item.url = !String.IsNullOrEmpty(item.path) ? await _MinioRepository.PresignedGetObjectAsync(item.path, area) : "";
                if(item.status == "I")
                {
                    var invInfo = invoicesData.FirstOrDefault(w => w.seq == item.seq && w.item == item.item);
                    item.invoiceno = invInfo?.invno;
                    item.invoiceid = invInfo?.invoiceid;
                }
            }
            if(headquery.formcode == "CASH_3")
            {
                amountData.actamt = amountData.amount;
            }
            result.head = headData;
            result.detail = detailsData;
            result.amount = amountData;
            result.file = yfilesData.OrderBy(i => i.item).ToList();
            result.invList = invoicesData;
            results.data = result;
            return results;
        }

        //给rq104使用，只更新部分栏位
        public async Task<Result<string>> UpdateCashDetailRemark(CashDetailDto cashDetailDto)
        {

            var result = new Result<string>();
            CashDetail cashDetail = (await _cashdetailRepository.WithDetailsAsync()).Where(x => x.Id == cashDetailDto.Id).FirstOrDefault();
            if(cashDetail != null)
            {
                cashDetail.remarks = cashDetailDto.remarks;
                await _cashdetailRepository.UpdateAsync(cashDetail);
                result.status = 1;
            }
            else
            {
                result.status = 0;
                result.data = "";
            }
            return result;
        }

        public async Task<Result<string>> UpdateCashDetailRequestAmount(IFormCollection formCollection)
        {
            var data = formCollection["data"];
            IList<CashDetailDto> cashDetailDtos = JsonConvert.DeserializeObject<IList<CashDetailDto>>(data);
            var result = new Result<string>();
            foreach (var cashDetailDto in cashDetailDtos)
            {
                CashDetail cashDetail = (await _cashdetailRepository.WithDetailsAsync()).Where(x => x.Id == cashDetailDto.Id).FirstOrDefault();
                List<Invoice> invoices = (await _invoiceRepository.WithDetailsAsync()).Where(i => i.rno == cashDetailDto.rno).ToList();
                decimal totalAmount = 0;
                foreach (var invoice in invoices)
                {
                    totalAmount += invoice.oamount;
                }
                if (cashDetail != null)
                {
                    //修改不能比原来的值大
                    if(cashDetail.amount1 < cashDetailDto.amount1)
                    {
                        result.message = "The reimbursement amount can only be reduced, not increased.";
                    }
                    //报销金额不得大于发票金额
                    if (totalAmount < cashDetail.amount1)
                    {
                        result.message = "The reimbursement amount must be less than or equal to the invoice amount.";
                        
                    }
                    if (!string.IsNullOrEmpty(result.message))
                    {
                        result.status = 0;
                        return result;
                    }

                    cashDetail.amount1 = cashDetailDto.amount1;
                    await _cashdetailRepository.UpdateAsync(cashDetail);
                    result.status = 1;
                }
                else
                {
                    result.status = 0;
                    result.data = "";
                }
            }
            
            return result;
        }

        public async Task<bool> CheckInvNoDuplicate(string invNo, string id)
        {
            bool duplicated =  (await _iBDInvoiceFolderRepository.CheckInvNoDuplicate(invNo, id));
            return duplicated;
        }

        public async Task<Result<string>> UpdateInvoice(IFormCollection formCollection)
        {
            var data = formCollection["data"];
            IList<InvoiceDto> invoiceDtos = JsonConvert.DeserializeObject<IList<InvoiceDto>>(data);
            var result = new Result<string>();
            List<Invoice> updateList = new List<Invoice>();
            foreach (var invoiceDto in invoiceDtos)
            {
                Invoice inv_old = (await _invoiceRepository.WithDetailsAsync()).Where(x => x.Id == invoiceDto.Id).FirstOrDefault();
                if (inv_old != null)
                {
                    if(inv_old.oamount < invoiceDto.oamount)
                    {
                        result.message += "The oamount can only be reduced, not increased."; 
                    }
                    //重新计算不含税金额
                    if (!inv_old.taxamount.Equals(invoiceDto.taxamount) || !inv_old.oamount.Equals(invoiceDto.oamount))
                    {
                        invoiceDto.amount = invoiceDto.oamount - invoiceDto.taxamount;
                    }
                }
                //检查税号是否合法
                if(invoiceDto.invtype == "INV0020" && !string.IsNullOrEmpty(invoiceDto.sellerTaxId))
                {
                    CorporateRegistration corporateRegistration = (await _corporateRegistrationRepository.WithDetailsAsync()).Where(x => x.unifiedNo == invoiceDto.sellerTaxId && x.usesInvoice == "F").FirstOrDefault();
                    if(corporateRegistration == null)
                    {
                        result.message += "The seller's tax number does not exist.";
                    }
                }
                //检查发票号码
                if (!string.IsNullOrEmpty(invoiceDto.invno))
                {
                    bool duplicate =  await CheckInvNoDuplicate(invoiceDto.invno, invoiceDto.Id.ToString());
                    if (duplicate)
                    {
                        result.message += "The invoice number already existed.";
                    }
                }
                if (!string.IsNullOrEmpty(result.message))
                {
                    result.status = 0;
                    return result;
                }
                Invoice oldData = (await _invoiceRepository.WithDetailsAsync()).Where(x => x.Id == invoiceDto.Id).FirstOrDefault();
                if(oldData != null)
                {
                    oldData.amount = invoiceDto.amount;
                    oldData.taxamount = invoiceDto.taxamount;
                    oldData.invno = invoiceDto.invno;
                    oldData.oamount = invoiceDto.oamount;
                    oldData.sellerTaxId = invoiceDto.sellerTaxId;
                    updateList.Add(oldData);
                }
                //检查无误后更新DB
                if(updateList.Count > 0)
                {
                    await _invoiceRepository.UpdateManyAsync(updateList);
                }
            }

            return result;
        }
    }
}
