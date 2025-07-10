using Autofac;
using Elastic.Apm.NetCoreAll;
using ERS.Domain.DomainServices;
using ERS.DTO.UberFare;
using ERS.EntityFrameworkCore;
using ERS.IDomainServices;
using ERS.Job.Jobs;
using ERS.Job.Jobs.DataSyncJobs;
using ERS.Job.Model;
using ERS.Job.Util;
using Hangfire;
using Hangfire.Dashboard.BasicAuthorization;
using Hangfire.MemoryStorage;
using Volo.Abp;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.Hangfire;
using Volo.Abp.Modularity;

namespace ERS.Job;
[DependsOn(
    typeof(ERSDomainModule),
    typeof(ERSEntityFrameworkCoreModule),
    typeof(ERSApplicationContractsModule),
    typeof(AbpHangfireModule),
    typeof(AbpAutofacModule),
    typeof(AbpAspNetCoreSerilogModule)
)]
public class ERSJobModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        //context.Services.AddHttpClient();
        context.Services.AddHangfire(options =>
        {
            options.UseSimpleAssemblyNameTypeSerializer().UseRecommendedSerializerSettings().UseStorage(new MemoryStorage());
        });
        context.Services.Configure<List<JobInfo>>(configuration.GetSection("hangfire:jobs"));
        context.Services.AddScoped(typeof(JobRegister));
        context.Services.AddHttpClient();
        //RegisterAutofac(context.Services);
        RegisterJob(context.Services);

    }
    IServiceProvider RegisterJob(IServiceCollection services)
    {
        services.AddScoped<IJobBase, DemoJob>();
        services.AddScoped<IJobBase,PS_WHQEmployeeSyncJob>();
        services.AddScoped<IJobBase,LOT_EmployeeSyncJob>();
        services.AddScoped<IJobBase,PS_StardandOrgSyncJob>();
        services.AddScoped<IJobBase, PS_CorssOrgSyncJob>();
        services.AddScoped<IJobBase,PS_StardandOrg2SyncJob>();
        services.AddScoped<IJobBase,Bank_Account_PRDSyncJob>();
        services.AddScoped<IJobBase,PMCS_PJCodeSyncJob>();
        services.AddScoped<IJobBase,SAP_EXCH_RATESyncJob>();
        services.AddScoped<IJobBase,CashAccountSyncJob>();
        services.AddScoped<IJobBase, CorporateRegistrationSyncJob>();

        //uber
        services.AddScoped<IJobBase, Uber_ERSSignJob>();
        services.AddScoped<IJobBase,Uber_UploadEmployeesJob>();
        services.AddScoped<IJobBase,Uber_UploadExpenseInfoJob>();
        services.AddScoped<IJobBase,Uber_DownloadTransactionByDaillyJob>();
        services.AddScoped<IJobBase,Uber_DownloadTransactionByMonthyJob>();
        return services.BuildServiceProvider();
    }

    IServiceProvider RegisterAutofac(IServiceCollection services)
    {
        var builder = services.GetContainerBuilder();
        builder.RegisterModule<AutofacModuleRegister>();
        return services.BuildServiceProvider();
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var configuration = context.GetConfiguration();
        var env = context.GetEnvironment();
        var serviceProvider = context.ServiceProvider;
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        //app.UseMiddleware<ExceptionHandleMiddleware>();
        //app.UseAbpRequestLocalization();
        //app.UseCorrelationId();
        //app.UseStaticFiles();
        if (configuration.GetSection("ElasticApm:Open").Value == "true")
            app.UseAllElasticApm(configuration);
        app.UseRouting();
        //app.UseCors();
        app.UseHttpsRedirection();
        //if (MultiTenancyConsts.IsEnabled)
        //{
        //    app.UseMultiTenancy();
        //}

        GlobalConfiguration.Configuration.UseActivator(new HangfireJobActivator(serviceProvider.GetService<IServiceScopeFactory>()));
        app.UseHangfireDashboard("/hangfire", new DashboardOptions
        {
            Authorization = new[]
                {
                    new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
                        {
                            SslRedirect = false,          // 是否将所有非SSL请求重定向到SSL URL
                            RequireSsl = false,           
                            LoginCaseSensitive = false,   //登录检查是否区分大小写
                            Users = new[]                 
                            {
                                new BasicAuthAuthorizationUser
                                {
                                    // Login ="admin",//用户名
                                    Login =configuration.GetSection("hangfire:login").Value,//用户名
                                    PasswordClear=configuration.GetSection("hangfire:password").Value
                                    // PasswordClear="admin"
                                }
                            }
                        })
                }
        });
        var service = (JobRegister)app.ApplicationServices.GetService(typeof(JobRegister));
        service.RegisterJobsFromConfig();

        //app.UseAuditing();
        //app.UseAbpSerilogEnrichers();
        //app.UseUnitOfWork();
        //app.UseConfiguredEndpoints();
    }
}
