using ERS.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using ERS.Entities.Bank;
using System.Linq;
using System;
using ERS.Context;
using Microsoft.Extensions.Configuration;
using ERS.Entities.Uber;
using ERS.Entities.Payment;

namespace ERS.EntityFrameworkCore;

[ConnectionStringName("Default")]
public class ERSDbContext : AbpDbContext<ERSDbContext>
{
    static decimal sysTimezone = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).Hours;
    private IConfiguration _Configuration;

    /* Add DbSet properties for your Aggregate Roots / Entities here.
     * 
     dotnet ef migrations add **
     */

    #region Entities from the modules

    /* Notice: We only implemented IIdentityDbContext and ITenantManagementDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityDbContext and ITenantManagementDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */


    public DbSet<AppConfig> AppConfig { get; set; }
    public DbSet<ApprovalFlow> ApprovalFlow { get; set; }
    public DbSet<ApprovalAssignedApprover> ApprovalAssignedApprover { get; set; }
    public DbSet<AuthRole> AuthRole { get; set; }
    public DbSet<AuthMenu> AuthMenu { get; set; }
    public DbSet<AuthRoleMenu> AuthRoleMenu { get; set; }
    public DbSet<AuthRoleApi> AuthRoleApi { get; set; }
    public DbSet<AuthUserRole> AuthUserRole { get; set; }
    public DbSet<AuthUserCompany> AuthUserCompany { get; set; }
    public DbSet<BDSignlevel> BDSignlevel { get; set; }
    public DbSet<BDTreelevel> BDTreelevel { get; set; }
    public DbSet<BDExpenseDept> BDExpenseDept { get; set; }
    public DbSet<DataDictionary> DataDictionary { get; set; }
    public DbSet<BDMealArea> BDMealArea { get; set; }
    public DbSet<BDCar> BDCar { get; set; }
    public DbSet<BdAccount> BdAccount { get; set; }
    public DbSet<ChargeAgainst> ChargeAgainst { get; set; }
    public DbSet<PmcsPjCode> PmcsPjCode { get; set; }
    public DbSet<BDCashReturn> BDCashReturn { get; set; }
    public DbSet<MealFee> MealFee { get; set; }
    public DbSet<BdExp> BdExp { get; set; }
    public DbSet<BDExpenseSenario> BdSenario { get; set; }
    public DbSet<BdPayTyp> BdPayTyp { get; set; }
    public DbSet<CashAmount> CashAmount { get; set; }
    public DbSet<CashAccount> CashAccount { get; set; }
    public DbSet<CashCarrydetail> CashCarrydetail { get; set; }
    public DbSet<CashCarryhead> CashCarryhead { get; set; }
    public DbSet<CashCurrency> CashCurrency { get; set; }
    public DbSet<CashDetail> CashDetail { get; set; }
    public DbSet<CashFile> CashFile { get; set; }
    public DbSet<CashDetailPst> CashDetailPst { get; set; }
    public DbSet<CashAccountPs> CashAccountPs { get; set; }
    public DbSet<CashHead> CashHead { get; set; }
    public DbSet<ApprovalPaper> EFormPaper { get; set; }
    public DbSet<BDPaperSign> BDPaperSign { get; set; }
    public DbSet<CashPaymentDetail> CashPaylist { get; set; }
    public DbSet<CashPaymentHead> CashPaymentHead { get; set; }
    public DbSet<CompanyAreaInfo> CompanyAreaInfo { get; set; }
    public DbSet<CompanyUser> CompanyUser { get; set; }
    public DbSet<Comtaxcode> Comtaxcode { get; set; }
    public DbSet<Doa> Doa { get; set; }
    public DbSet<DoaType> DoaType { get; set; }
    public DbSet<EAutono> EAutono { get; set; }
    public DbSet<BDForm> BDForm { get; set; }
    public DbSet<EFormAlist> EFormAlist { get; set; }
    public DbSet<EFormAlistHistory> EFormAlistHistory { get; set; }
    public DbSet<EFormAudit> EFormAudit { get; set; }
    public DbSet<EFormAuser> EFormAuser { get; set; }
    public DbSet<EFormContact> EFormContact { get; set; }
    public DbSet<CustomerNickname> CustomerNickname { get; set; }
    public DbSet<EFormFlow> EFormFlow { get; set; }
    public DbSet<EFormHead> EFormHead { get; set; }
    public DbSet<EFormProxy> EFormProxy { get; set; }
    public DbSet<EFormSignlog> EFormSignlog { get; set; }
    public DbSet<Empchs> Empchs { get; set; }
    public DbSet<EmployeeInfo> EmployeeInfo { get; set; }
    public DbSet<Employee> Employee { get; set; }
    public DbSet<EmpOrg> EmpOrg { get; set; }
    public DbSet<EmpOrgCohead> EmpOrgCohead { get; set; }
    public DbSet<EmpOrgCross> EmpOrgCross { get; set; }
    public DbSet<EmpOrgCrossPs> EmpOrgCrossPs { get; set; }
    public DbSet<EmpOrgLv> EmpOrgLv { get; set; }
    public DbSet<EmpOrgStandard> EmpOrgStandard { get; set; }
    public DbSet<Finreview> Finreview { get; set; }
    public DbSet<MailLog> MailLog { get; set; }
    public DbSet<Mailtemplate> Mailtemplate { get; set; }
    public DbSet<ProxyCash> ProxyCash { get; set; }
    public DbSet<SAPExchRate> SAPExchRate { get; set; }
    public DbSet<SuperCategory> SuperCategory { get; set; }
    public DbSet<Invoice> Invoice { get; set; }
    // public DbQuery<Queryposting> Queryposting { get; set; }
    // public DbQuery<Postingdetail> Postingdetail { get; set; }
    public DbSet<ComBank> ComBank { get; set; }
    public DbSet<BDInvoiceFolder> BDInvoiceFolder { get; set; }
    public DbSet<BDInvoiceType> BDInvoiceType { get; set; }
    public DbSet<BDInvoiceRail> BDInvoiceRail { get; set; }
    public DbSet<BDTicketRail> BDTicketRail { get; set; }
    public DbSet<BDTaxRate> BDTaxRate { get; set; }
    public DbSet<HelpManual> HelpManual { get; set; }
    public DbSet<BDExpID> BDExpID { get; set; }
    public DbSet<SendMobileLog> SendMobileLog { get; set; }
    public DbSet<SendMobileFileLog> SendMobileFileLog { get; set; }
    public DbSet<MobileCallBackLog> MobileCallBackLog { get; set; }
    public DbSet<ExpenseSenarioExtraSteps> AdditionalApprovalSteps { get; set; }
    public DbSet<ExtraSteps> ExtraSteps { get; set; }
    public DbSet<CorporateRegistration> CorporateRegistration { get; set; }
    public DbSet<BDVender> Supplier { get; set; }
    public DbSet<CashUberHead> CashUberHead { get; set; }
    public DbSet<CashUberDetail> CashUberDetail { get; set; }
    public DbSet<BDCompanyCategory> BDCompanyCategory { get; set; }

    public DbSet<BDCompanySite> BDCompanySite { get; set; }
    public DbSet<DataDictionaryCriteria> DataDictionaryCriteria { get; set; }

    public DbSet<UberTransactionalDay> UberTransactionalDay { get; set; }

    public DbSet<OCRResults> OcrResults { get; set; }

    public DbSet<BDExpense> BDExpense { get; set; }

    #endregion

    public ERSDbContext(DbContextOptions<ERSDbContext> options) : base(options)
    {
    }
    public ERSDbContext(DbContextOptions<ERSDbContext> options, IConfiguration Configuration) : base(options)
    {
        _Configuration = Configuration;
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
#if DEBUG
        optionsBuilder.UseLoggerFactory(LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        }));
#endif
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        //添加複合主鍵：注意isdeleted也需要加入，否則刪除的數據會被認爲已經存在，導致複合主鍵衝突
        //例子 builder.Entity<AuthMenu>().HasKey(x => new { x.menukey, x.menuname,x.isdeleted });
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
            {
                entityType.AddSoftDeleteQueryFilter();
            }
        }
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }
    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
    {
        OnBeforeSaving();
        OnBeforeSavingByDate();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
    private void OnBeforeSaving()
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (typeof(ISoftDelete).IsAssignableFrom(entry.Entity.GetType()))
            {
                switch (entry.State)
                {
                    case EntityState.Deleted:
                        if (entry.Entity.GetType() == typeof(CorporateRegistration))
                        {
                            break;
                        }
                        else if (entry.Entity.GetType() == typeof(BDExpenseDept))
                        {
                            break;
                        }
                        //加簽人員資料變動做物理刪除
                        else if (entry.Entity.GetType() == typeof(ExpenseSenarioExtraSteps))
                        {
                            break;
                        }
                        else
                        {
                            entry.State = EntityState.Modified;
                            entry.CurrentValues["isdeleted"] = true;
                            break;
                        }

                }
            }
        }
    }
    private void OnBeforeSavingByDate()
    {
        if (_Configuration.GetSection("UnitTest").Value == "true") return;
        if (_Configuration.GetSection("IsJob").Value == "true") return;

        decimal diff = RequestContext.Current?.timezone == null ? 8 : RequestContext.Current.timezone - sysTimezone;

        ChangeTracker.Entries().Where(i => i.State == EntityState.Added && i.Entity is SuperBaseEntity).ToList().ForEach(i => { 
            ((SuperBaseEntity)i.Entity).cdate = DateTime.Now.AddDays((double)diff);
            if (RequestContext.Current.userid != null)
            {
                ((SuperBaseEntity)i.Entity).cuser = RequestContext.Current.userid;
            }
        });

        ChangeTracker.Entries().Where(i => i.State == EntityState.Modified && i.Entity is SuperBaseEntity).ToList().ForEach(i => { 
            ((SuperBaseEntity)i.Entity).mdate = DateTime.Now.AddDays((double)diff);
            if (RequestContext.Current.userid != null)
            {
                ((SuperBaseEntity)i.Entity).muser = RequestContext.Current.userid;
            }
        });
    }
}
