using ERS.Entities;
using ERS.Entities.Bank;
using ERS.Entities.Payment;
using System;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace ERS;

public class ERSTestDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private IRepository<AppConfig, Guid> _AppConfigRepository;
    private IRepository<BDCompanyCategory, Guid> _CompanyRepository;
    private IRepository<CashAccount, Guid> _CashAccountRepository;
    private IRepository<Employee, Guid> _EmployeeRepository;

    private IRepository<PmcsPjCode, Guid> _PmcsPjCodeRepository;

    private IRepository<CashCurrency, Guid> _CashCurrencyRepository;
    private IRepository<EmpOrg, Guid> _EmpOrgRepository;
    private IRepository<CustomerNickname, Guid> _NicknameRepository;

     private IRepository<EAutono, Guid> _EAutonoRepository;

     private IRepository<SAPExchRate, Guid> _SAPExchRateRepository;
    private IRepository<BdAccount, Guid> _BdAccountRepository;
    private IRepository<BdExp, Guid> _BdExpRepository;
    private IRepository<CashHead, Guid> _CashHeadRepository;
    private IRepository<CashDetail, Guid> _CashDetailRepository;
    private IRepository<CashAmount, Guid> _CashAmountRepository;
    private IRepository<CashFile, Guid> _CashFileRepository;
    private IRepository<Invoice, Guid> _InvoiceRepository;
    private IRepository<EFormHead, Guid> _EFormHeadRepository;
    private IRepository<ApprovalPaper, Guid> _EFormPaperRepository;
    private IRepository<BDForm, Guid> _BDFormRepository;
    private IRepository<BDPaperSign, Guid> _BDPaperSignRepository;
    private IRepository<BdPayTyp, Guid> _BDPayTypRepository;
    private IRepository<BDCashReturn,Guid> _BDCashRerurnRepository;
    private IRepository<EFormAudit, Guid> _EFormAuditRepository;
    private IRepository<Finreview, Guid> _FinreviewRepository;
    private IRepository<BDCar, Guid> _BDCarRepository;
    private IRepository<CashCarrydetail, Guid> _CashCarrydetailRepository;
    private IRepository<CashCarryhead, Guid> _CashCarryheadRepository;
    private IRepository<EFormSignlog, Guid> _EFormSignlogRepository;
    private IRepository<EFormAlist, Guid> _EFormAlistRepository;
    private IRepository<EFormAuser, Guid> _EFormAuserRepository;
    private IRepository<CashPaymentDetail, Guid> _CashPaylistRepository;
    private IRepository<EmpOrgLv, Guid> _EmpOrgLvRepository;
    private IRepository<SuperCategory, Guid> _SuperCategoryRepository;
    private IRepository<ComBank, Guid> _ComBankRepository;
    private IRepository<CashDetailPst, Guid> _CashDetailPstRepository;
    private IRepository<BDInvoiceFolder, Guid> _BDInvoiceFolderRepository;
    private IRepository<Empchs, Guid> _EmpchsRepository;
    private IRepository<BDExpID, Guid> _BDExpIDRepository;
    private IRepository<BDInvoiceType, Guid> _BDInvoiceTypeRepository;
    public ERSTestDataSeedContributor(
        IRepository<AppConfig, Guid> AppConfigRepository,
        IRepository<BDCompanyCategory, Guid> CompanyRepository,
        IRepository<CashAccount, Guid> CashAccountRepository,
        IRepository<Employee, Guid> EmployeeRepository,
        IRepository<PmcsPjCode, Guid> PmcsPjCodeRepository,
        IRepository<CashCurrency, Guid> CashCurrencyRepository,
        IRepository<EmpOrg, Guid> EmpOrgRepository,
        IRepository<CustomerNickname, Guid> NicknameRepository,
        IRepository<EAutono, Guid> EAutonoRepository,
        IRepository<SAPExchRate, Guid> SAPExchRateRepository,
        IRepository<BdAccount, Guid> BdAccountRepository,
        IRepository<BdExp, Guid> BdExpRepository,
        IRepository<CashHead, Guid> CashHeadRepository,
        IRepository<CashDetail, Guid> CashDetailRepository,
        IRepository<CashAmount, Guid> CashAmountRepository,
        IRepository<CashFile, Guid> CashFileRepository,
        IRepository<Invoice, Guid> InvoiceRepository,
        IRepository<EFormHead, Guid> EFormHeadRepository,
        IRepository<BDForm, Guid> BDFormRepository,
        IRepository<ApprovalPaper, Guid> EFormPaperRepository,
        IRepository<BDPaperSign, Guid> BDPaperSignRepository,
        IRepository<BdPayTyp, Guid> BDPayTypRepository,
        IRepository<BDCashReturn,Guid> BDCashReturnRepository,
        IRepository<EFormAudit, Guid> EFormAuditRepository,
        IRepository<Finreview, Guid> FinreviewRepository,
        IRepository<BDCar, Guid> BDCarRepository,
        IRepository<CashCarrydetail, Guid> CashCarrydetailRepository,
        IRepository<CashCarryhead, Guid> CashCarryheadRepository,
        IRepository<EFormSignlog, Guid> EFormSignlogRepository,
        IRepository<EFormAlist, Guid> EFormAlistRepository,
        IRepository<EFormAuser, Guid> EFormAuserRepository,
        IRepository<CashPaymentDetail,Guid> CashPaylistRepository,
        IRepository<EmpOrgLv,Guid> EmpOrgLvRepository,
        IRepository<SuperCategory,Guid> SuperCategoryRepository,
        IRepository<ComBank, Guid> ComBankRepository,
        IRepository<CashDetailPst, Guid> CashDetailPstRepository,
        IRepository<BDInvoiceFolder, Guid> BDInvoiceFolderRepository,
        IRepository<Empchs, Guid> EmpchsRepository,
        IRepository<BDExpID, Guid> BDExpIDRepository,
        IRepository<BDInvoiceType, Guid> BDInvoiceTypeRepository
        )
    {
        _AppConfigRepository = AppConfigRepository;
        _CompanyRepository = CompanyRepository;
        _CashAccountRepository=CashAccountRepository;
        _EmployeeRepository=EmployeeRepository;
        _PmcsPjCodeRepository=PmcsPjCodeRepository;
        _CashCurrencyRepository=CashCurrencyRepository;
        _EmpOrgRepository=EmpOrgRepository;
        _NicknameRepository=NicknameRepository;
        _EAutonoRepository=EAutonoRepository;
        _SAPExchRateRepository=SAPExchRateRepository;
        _BdAccountRepository = BdAccountRepository;
        _BdExpRepository = BdExpRepository;
        _CashHeadRepository = CashHeadRepository;
        _CashDetailRepository = CashDetailRepository;
        _CashAmountRepository = CashAmountRepository;
        _CashFileRepository = CashFileRepository;
        _InvoiceRepository = InvoiceRepository;
        _EFormHeadRepository = EFormHeadRepository;
        _BDFormRepository = BDFormRepository;
        _EFormPaperRepository = EFormPaperRepository;
        _BDPaperSignRepository=BDPaperSignRepository;
        _BDPayTypRepository = BDPayTypRepository;
        _BDCashRerurnRepository = BDCashReturnRepository;
        _EFormAuditRepository = EFormAuditRepository;
        _FinreviewRepository = FinreviewRepository;
        _BDCarRepository = BDCarRepository;
        _CashCarrydetailRepository = CashCarrydetailRepository;
        _CashCarryheadRepository = CashCarryheadRepository;
        _EFormSignlogRepository = EFormSignlogRepository;
        _EFormAlistRepository = EFormAlistRepository;
        _EFormAuserRepository = EFormAuserRepository;
        _CashPaylistRepository = CashPaylistRepository;
        _EmpOrgLvRepository = EmpOrgLvRepository;
        _SuperCategoryRepository = SuperCategoryRepository;
        _ComBankRepository = ComBankRepository;
        _CashDetailPstRepository = CashDetailPstRepository;
        _BDInvoiceFolderRepository = BDInvoiceFolderRepository;
        _EmpchsRepository = EmpchsRepository;
        _BDExpIDRepository = BDExpIDRepository;
        _BDInvoiceTypeRepository = BDInvoiceTypeRepository;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        /* Seed additional test data... */
        await _AppConfigRepository.InsertManyAsync(JsonHelper.Read<AppConfig>(Path.Combine("Json", "AppConfig.json")));
        await _CompanyRepository.InsertManyAsync(JsonHelper.Read<BDCompanyCategory>(Path.Combine("Json", "Company.json")));
        await _CashAccountRepository.InsertManyAsync(JsonHelper.Read<CashAccount>(Path.Combine("Json", "cash_account.json")));
        await _EmployeeRepository.InsertManyAsync(JsonHelper.Read<Employee>(Path.Combine("Json", "employee.json")));
        await _PmcsPjCodeRepository.InsertManyAsync(JsonHelper.Read<PmcsPjCode>(Path.Combine("Json", "pmcs_pjcode.json")));
        await _CashCurrencyRepository.InsertManyAsync(JsonHelper.Read<CashCurrency>(Path.Combine("Json", "cash_currency.json")));
        await _EmpOrgRepository.InsertManyAsync(JsonHelper.Read<EmpOrg>(Path.Combine("Json", "emp_org.json")));
        await _NicknameRepository.InsertManyAsync(JsonHelper.Read<CustomerNickname>(Path.Combine("Json", "customer_nickname.json")));
        await _EAutonoRepository.InsertManyAsync(JsonHelper.Read<EAutono>(Path.Combine("Json", "e_autono.json")));
        await _SAPExchRateRepository.InsertManyAsync(JsonHelper.Read<SAPExchRate>(Path.Combine("Json", "sap_exch_rate.json")));
        await _BdAccountRepository.InsertManyAsync(JsonHelper.Read<BdAccount>(Path.Combine("Json", "BDAccount.json")));
        await _BdExpRepository.InsertManyAsync(JsonHelper.Read<BdExp>(Path.Combine("Json", "BDExp.json")));
        await _CashHeadRepository.InsertManyAsync(JsonHelper.Read<CashHead>(Path.Combine("Json", "cash_head.json")));
        await _CashDetailRepository.InsertManyAsync(JsonHelper.Read<CashDetail>(Path.Combine("Json", "cash_detail.json")));
        await _CashAmountRepository.InsertManyAsync(JsonHelper.Read<CashAmount>(Path.Combine("Json", "CashAmount.json")));
        await _CashFileRepository.InsertManyAsync(JsonHelper.Read<CashFile>(Path.Combine("Json", "CashFile.json")));
        await _InvoiceRepository.InsertManyAsync(JsonHelper.Read<Invoice>(Path.Combine("Json", "Invoice.json")));
        await _EFormHeadRepository.InsertManyAsync(JsonHelper.Read<EFormHead>(Path.Combine("Json", "EFormHead.json")));
        await _BDFormRepository.InsertManyAsync(JsonHelper.Read<BDForm>(Path.Combine("Json", "e_form.json")));
        await _EFormPaperRepository.InsertManyAsync(JsonHelper.Read<ApprovalPaper>(Path.Combine("Json", "approval_paper.json")));
        await _BDPaperSignRepository.InsertManyAsync(JsonHelper.Read<BDPaperSign>(Path.Combine("Json", "bd_paper_sign.json")));
        await _BDPayTypRepository.InsertManyAsync(JsonHelper.Read<BdPayTyp>(Path.Combine("Json", "bdpaytyp.json")));
        await _BDCashRerurnRepository.InsertManyAsync(JsonHelper.Read<BDCashReturn>(Path.Combine("Json","bd_cash_return.json")));
        await _EFormAuditRepository.InsertManyAsync(JsonHelper.Read<EFormAudit>(Path.Combine("Json", "EFormAudit.json")));
        await _FinreviewRepository.InsertManyAsync(JsonHelper.Read<Finreview>(Path.Combine("Json", "Finreview.json")));
        await _BDCarRepository.InsertManyAsync(JsonHelper.Read<BDCar>(Path.Combine("Json","bd_car.json")));
        await _CashCarrydetailRepository.InsertManyAsync(JsonHelper.Read<CashCarrydetail>(Path.Combine("Json","cash_carrydetail.json")));
        await _CashCarryheadRepository.InsertManyAsync(JsonHelper.Read<CashCarryhead>(Path.Combine("Json","cash_carryhead.json")));
        await _EFormSignlogRepository.InsertManyAsync(JsonHelper.Read<EFormSignlog>(Path.Combine("Json","e_form_signlog.json")));
        await _EFormAlistRepository.InsertManyAsync(JsonHelper.Read<EFormAlist>(Path.Combine("Json","e_form_alist.json")));
        await _EFormAuserRepository.InsertManyAsync(JsonHelper.Read<EFormAuser>(Path.Combine("Json","e_form_auser.json")));
        await _CashPaylistRepository.InsertManyAsync(JsonHelper.Read<CashPaymentDetail>(Path.Combine("Json","cash_paylist.json")));
        await _EmpOrgLvRepository.InsertManyAsync(JsonHelper.Read<EmpOrgLv>(Path.Combine("Json", "emp_org_lv.json")));
        await _SuperCategoryRepository.InsertManyAsync(JsonHelper.Read<SuperCategory>(Path.Combine("Json", "supercategory.json")));
        await _ComBankRepository.InsertManyAsync(JsonHelper.Read<ComBank>(Path.Combine("Json", "combank.json")));
        await _CashDetailPstRepository.InsertManyAsync(JsonHelper.Read<CashDetailPst>(Path.Combine("Json", "cash_detail_pst.json")));
        await _BDInvoiceFolderRepository.InsertManyAsync(JsonHelper.Read<BDInvoiceFolder>(Path.Combine("Json", "bd_invoice_folder.json")));
        await _EmpchsRepository.InsertManyAsync(JsonHelper.Read<Empchs>(Path.Combine("Json", "empchs.json")));
        await _BDExpIDRepository.InsertManyAsync(JsonHelper.Read<BDExpID>(Path.Combine("Json", "bd_exp_identification.json")));
        await _BDInvoiceTypeRepository.InsertManyAsync(JsonHelper.Read<BDInvoiceType>(Path.Combine("Json", "bd_invoice_type.json")));
    }


}
