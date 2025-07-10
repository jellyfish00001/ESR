using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.Entities;
using ERS.IDomainServices;
using ERS.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Volo.Abp.ObjectMapping;
using ERS.DTO.CashCarryDetail;

namespace ERS.DomainServices
{
    public class CashCarryDetailDomainService : CommonDomainService, ICashCarryDetailDomainService
    {
        private IConfiguration _Configuration;
        private ICashHeadRepository _CashHeadRepository;
        private ICashDetailRepository _CashDetailRepository;
        private IAutoNoRepository _AutoNoRepository;
        private ICashCarrydetailRepository _CashCarrydetailRepository;
        private ICashCarryheadRepository _CashCarryheadRepository;
        private IEmployeeRepository _EmployeeRepository;
        private IComBankRepository _ComBankRepository;
        private ICompanyRepository _CompanyRepository;
        //private IBDExpRepository _BDExpRepository;
        private IBDExpenseSenarioRepository _BDExpenseSenarioRepository;
        private IInvoiceRepository _InvoiceRepository;
        private ICashCarrydetailRepository _cashCarryDetailRepository;
        private IObjectMapper _ObjectMapper;

        public CashCarryDetailDomainService(
            IConfiguration configuration,
            ICashHeadRepository cashHeadRepository,
            ICashDetailRepository cashDetailRepository,
            IAutoNoRepository autoNoRepository,
            ICashCarrydetailRepository cashCarrydetailRepository,
            ICashCarryheadRepository cashCarryheadRepository,
            IEmployeeRepository employeeRepository,
            IComBankRepository comBankRepository,
            ICompanyRepository companyRepository,
            //IBDExpRepository bdExpRepository,
            IBDExpenseSenarioRepository bdExpenseSenarioRepository,
            IInvoiceRepository invoiceRepository,
            IObjectMapper objectMapper)
        {
            _Configuration = configuration;
            _CashHeadRepository = cashHeadRepository;
            _CashDetailRepository = cashDetailRepository;
            _AutoNoRepository = autoNoRepository;
            _CashCarrydetailRepository = cashCarrydetailRepository;
            _CashCarryheadRepository = cashCarryheadRepository;
            _EmployeeRepository = employeeRepository;
            _ComBankRepository = comBankRepository;
            _CompanyRepository = companyRepository;
            _BDExpenseSenarioRepository = bdExpenseSenarioRepository;
            _InvoiceRepository = invoiceRepository;
            _ObjectMapper = objectMapper;
        }

        /// <summary>
        /// 已申请表单修改
        /// </summary>
        public async Task<Result<string>> UpdateCashCarrydetail(IFormCollection formCollection)
        {
            var result = new Result<string>();
            try
            {
                var data = formCollection["data"];
                IList<CashCarryDetailDto> cashCarrydetails = JsonConvert.DeserializeObject<IList<CashCarryDetailDto>>(data);
                List<CashCarrydetail> updateList = new List<CashCarrydetail>();
                foreach (CashCarryDetailDto cashCarrydetail in cashCarrydetails)
                {
                    CashCarrydetail oldData = (await _cashCarryDetailRepository.WithDetailsAsync()).Where(x => x.Id == Guid.Parse(cashCarrydetail.Id)).FirstOrDefault();
                    if(oldData != null)
                    {
                        oldData.basecurr = cashCarrydetail.basecurr;
                        oldData.acctant = cashCarrydetail.acctant;
                        oldData.order = cashCarrydetail.order;
                        oldData.costcenter = cashCarrydetail.costcenter;
                        oldData.linetext = cashCarrydetail.linetext;
                        updateList.Add(oldData);
                    }
                }
                if (updateList.Count > 0)
                {
                    await _cashCarryDetailRepository.UpdateManyAsync(updateList);
                }
            }
            catch (Exception e)
            {
                result.message = e.Message;
                result.status = 0;
            }
            return result;
        }

        /// <summary>
        /// SAP入賬清單生成
        /// </summary>
        /// <param name="rno"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Result<string>> SaveCashCarryDetail(string rno, string userId)
        {
            Result<string> result = new Result<string>();

            // Check if already exists in CashCarrydetail
            var existingDetail = await (await _CashCarrydetailRepository.WithDetailsAsync())
                .FirstOrDefaultAsync(w => w.rno == rno && w.rnostatus == "N");

            if (existingDetail != null)
            {
                result.status = 2;
                result.message = string.Format(L["ExistsAccountList"], existingDetail.rno);
                return result;
            }

            // 1. Query cashhead and cashdetails
            var cashHead = await _CashHeadRepository.ReadCashHeadsByNo(rno);
            var cashDetails = await _CashDetailRepository.ReadCashDetailsByNo(rno);
            var invoicelist = await _InvoiceRepository.ReadDetailsByNo(rno);

            if (cashHead == null || cashDetails.Count == 0)
            {
                result.message = L["RnoHaveNoData"];
                result.status = 2;
                return result;
            }

            // 2. Generate carry number
            string carryNo = await _AutoNoRepository.CreateAccountNo();

            // 3. Create CashCarryhead
            CashCarryhead cashCarryhead = new CashCarryhead
            {
                carryno = carryNo,
                postdate = DateTime.Now,
                acctant = userId,
                company = cashHead.company,
                companycode = cashHead.company,
                bank = cashHead.bank,
                cuser = userId,
                cdate = DateTime.Now,
                stat = "Y"
            };

            // 4. Create CashCarrydetail records
            List<CashCarrydetail> cashCarrydetails = new List<CashCarrydetail>();
            string category = (cashHead.formcode == ERSConsts.FormCode.CASH_3.ToString()) ? ERSConsts.SenarioCategoryEnum.Advance.ToValue() : ERSConsts.SenarioCategoryEnum.Reimbursement.ToValue();

            // Get company info
            var companySap = await (await _CompanyRepository.WithDetailsAsync())
                .Where(c => c.company == cashHead.company)
                .Select(c => c.CompanySap)
                .FirstOrDefaultAsync();

            var bdExpenseSenarios = await (await _BDExpenseSenarioRepository.WithDetailsAsync())
                .Where(b => b.category == cashHead.company && b.category == category)
                .ToListAsync();

            // Get bank vendor code
            string vendorCode = await _ComBankRepository.GetVenderCode(cashHead.company, cashHead.bank);

            int seqNo = 1;

            // Group invoices with non-empty invno

            var invoicesWithNumbers = invoicelist.Where(i => !string.IsNullOrEmpty(i.invno)).ToList();
            var invoicesWithoutNumbers = invoicelist.Where(i => string.IsNullOrEmpty(i.invno)).ToList();

            var matchCashDetailWithNumbers = cashDetails.Where(cd => invoicesWithNumbers.Any(inv => inv.seq == cd.seq)).ToList();
            var matchCashDetailWithoutNumbers = cashDetails.Where(cd => invoicesWithoutNumbers.Any(inv => inv.seq == cd.seq)).ToList();

            //若費用發票有發票號碼，則屬於可抵扣費用，可抵扣費單獨做成一張傳票；其他則合併成一張傳票
            foreach (var detail in matchCashDetailWithNumbers)
            {
                //Create expense line (postkey 40)
                var bdExpSenario = bdExpenseSenarios.FirstOrDefault(e => e.expensecode == detail.expcode);

                //税前金额和税金各有一笔Posting Key:40;然后总费用对应有一笔31出现
                CashCarrydetail expenseLine40_amount = new CashCarrydetail
                {
                    carryno = carryNo,
                    seq = seqNo,
                    docdate = DateTime.Now,
                    postdate = DateTime.Now,
                    companysap = companySap,
                    basecurr = detail.basecurr,
                    rate = 1,
                    @ref = rno,
                    doctyp = ERSConsts.SAPDocumentTypeEnum.KR.ToString(),
                    postkey = ERSConsts.SAPPostingKeyEnum.Debit.ToValue(),
                    acctcode = detail.acctcode,
                    actamt1 = detail.baseamt ?? 0,
                    actamt2 = detail.baseamt ?? 0,
                    txtcode = "M0", // Default tax code
                    costcenter = detail.deptid,
                    linetext = $"{DateTime.Now:yy/MM} {cashHead.cname} 報銷 {detail.expname} 費用",
                    asinmnt = bdExpSenario?.assignment ?? "",
                    ref1 = detail.payeeid ?? cashHead.payeeId,
                    order = bdExpSenario?.projectcode ?? cashHead.projectcode,
                    rno = rno,
                    company = cashHead.company,
                    cuser = userId,
                    cdate = DateTime.Now,
                    carritem = detail.seq,
                    formcode = cashHead.formcode,
                    rnostatus = ERSConsts.Value_N,
                };

                CashCarrydetail expenseLine40_tax = new CashCarrydetail
                {
                    carryno = carryNo,
                    seq = seqNo,
                    docdate = DateTime.Now,
                    postdate = DateTime.Now,
                    companysap = companySap,
                    basecurr = detail.basecurr,
                    rate = 1,
                    @ref = rno,
                    doctyp = ERSConsts.SAPDocumentTypeEnum.KR.ToString(),
                    postkey = ERSConsts.SAPPostingKeyEnum.Debit.ToValue(),
                    acctcode = detail.acctcode,
                    actamt1 = detail.baseamt ?? 0,
                    actamt2 = detail.baseamt ?? 0,
                    txtcode = "M0", // Default tax code
                    costcenter = detail.deptid,
                    linetext = $"{DateTime.Now:yy/MM} {cashHead.cname} 報銷 {detail.expname} 費用",
                    asinmnt = bdExpSenario?.assignment ?? "",
                    ref1 = detail.payeeid ?? cashHead.payeeId,
                    order = bdExpSenario?.projectcode ?? cashHead.projectcode,
                    rno = rno,
                    company = cashHead.company,
                    cuser = userId,
                    cdate = DateTime.Now,
                    carritem = detail.seq,
                    formcode = cashHead.formcode,
                    rnostatus = ERSConsts.Value_N,
                };

                CashCarrydetail expenseLine31 = new CashCarrydetail
                {
                    carryno = carryNo,
                    seq = seqNo,
                    docdate = DateTime.Now,
                    postdate = DateTime.Now,
                    companysap = companySap,
                    basecurr = detail.basecurr,
                    rate = 1,
                    @ref = rno,
                    doctyp = ERSConsts.SAPDocumentTypeEnum.KR.ToString(),
                    postkey = ERSConsts.SAPPostingKeyEnum.Credit.ToValue(),
                    acctcode = detail.acctcode,
                    actamt1 = detail.baseamt ?? 0,
                    actamt2 = detail.baseamt ?? 0,
                    txtcode = "M0", // Default tax code
                    costcenter = detail.deptid,
                    linetext = $"{DateTime.Now:yy/MM} {cashHead.cname} 报销 {detail.expname} 费用",
                    asinmnt = bdExpSenario?.assignment ?? "",
                    ref1 = detail.payeeid ?? cashHead.payeeId,
                    order = bdExpSenario?.projectcode ?? cashHead.projectcode,
                    rno = rno,
                    company = cashHead.company,
                    cuser = userId,
                    cdate = DateTime.Now,
                    carritem = detail.seq,
                    formcode = cashHead.formcode,
                    rnostatus = "N",
                };

                cashCarrydetails.Add(expenseLine40_amount);
                cashCarrydetails.Add(expenseLine40_tax);
                cashCarrydetails.Add(expenseLine31);

                seqNo++;
            }

            if (matchCashDetailWithoutNumbers.Any())
            {
                var detail = matchCashDetailWithoutNumbers.FirstOrDefault();

                // Add vendor payment line (postkey 31) for each expense
                CashCarrydetail paymentLine_40 = new CashCarrydetail
                {
                    carryno = carryNo,
                    seq = seqNo,
                    docdate = DateTime.Now,
                    postdate = DateTime.Now,
                    companysap = companySap,
                    basecurr = detail.basecurr,
                    rate = 1,
                    @ref = rno,
                    doctyp = ERSConsts.SAPDocumentTypeEnum.KR.ToString(),
                    postkey = ERSConsts.SAPPostingKeyEnum.Debit.ToValue(),
                    acctcode = !string.IsNullOrEmpty(vendorCode) ? vendorCode : "Z800003",
                    // actamt1 = detail.baseamt ?? 0,
                    // actamt2 = detail.baseamt ?? 0,
                    actamt1 = invoicesWithoutNumbers.Sum(i => (decimal?)i.amount) ?? 0,      
                    actamt2 = invoicesWithoutNumbers.Sum(i => (decimal?)i.amount) ?? 0,
                    payterm = "YTTP",
                    paytyp = "T",
                    baslindate = DateTime.Now,
                    linetext = $"{DateTime.Now:yy/MM} {cashHead.cname} 报销 {detail.expname} 费用",
                    ref1 = detail.payeeid ?? cashHead.payeeId,
                    rno = rno,
                    company = cashHead.company,
                    cuser = userId,
                    cdate = DateTime.Now,
                    carritem = detail.seq,
                    formcode = cashHead.formcode,
                    rnostatus = ERSConsts.CashCarryDetailStatusEnum.N.ToString(),
                };

                CashCarrydetail paymentLine_31 = new CashCarrydetail
                {
                    carryno = carryNo,
                    seq = seqNo,
                    docdate = DateTime.Now,
                    postdate = DateTime.Now,
                    companysap = companySap,
                    basecurr = detail.basecurr,
                    rate = 1,
                    @ref = rno,
                    doctyp = ERSConsts.SAPDocumentTypeEnum.KR.ToString(),
                    postkey = ERSConsts.SAPPostingKeyEnum.Debit.ToValue(),
                    acctcode = !string.IsNullOrEmpty(vendorCode) ? vendorCode : "Z800003",
                    // actamt1 = detail.baseamt ?? 0,
                    // actamt2 = detail.baseamt ?? 0,
                    actamt1 = invoicesWithoutNumbers.Sum(i => (decimal?)i.amount) ?? 0,
                    actamt2 = invoicesWithoutNumbers.Sum(i => (decimal?)i.amount) ?? 0,
                    payterm = "YTTP",
                    paytyp = "T",
                    baslindate = DateTime.Now,
                    linetext = $"{DateTime.Now:yy/MM} {cashHead.cname} 报销 {detail.expname} 费用",
                    ref1 = detail.payeeid ?? cashHead.payeeId,
                    rno = rno,
                    company = cashHead.company,
                    cuser = userId,
                    cdate = DateTime.Now,
                    carritem = detail.seq,
                    formcode = cashHead.formcode,
                    rnostatus = ERSConsts.CashCarryDetailStatusEnum.N.ToString(),
                };

                cashCarrydetails.Add(paymentLine_40);
                cashCarrydetails.Add(paymentLine_31);

            }

            // 5. Save to database
            await _CashCarryheadRepository.InsertAsync(cashCarryhead);
            await _CashCarrydetailRepository.InsertManyAsync(cashCarrydetails);

            result.status = 1;
            result.data = carryNo;
            result.message = "Cash carry detail saved successfully";

            return result;
        }

        /// <summary>
        /// by 单号获取入账明细内容
        /// </summary>
        /// <param name="rno"></param>
        /// <returns></returns>
        public async Task<Result<List<CashCarryDetailDto>>> GetCarryDetailByRno(string rno)
        {
            Result<List<CashCarryDetailDto>> result = new Result<List<CashCarryDetailDto>>();
            List<CashCarrydetail> data = (await _cashCarryDetailRepository.WithDetailsAsync()).Where(x => x.rno == rno).ToList();
            List<CashCarryDetailDto> res = _ObjectMapper.Map<List<CashCarrydetail>, List<CashCarryDetailDto>>(data);

            result.data = res;
            return result;
        }
    }
}