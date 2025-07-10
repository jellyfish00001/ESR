using System;
using System.IO;
using System.Linq;
using Abp.AspNetCore.Mvc.ExceptionHandling;
using Elastic.Apm.NetCoreAll;
using ERS.EntityFrameworkCore;
using ERS.Filter;
using ERS.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.Swashbuckle;
using Volo.Abp.VirtualFileSystem;

namespace ERS;

[DependsOn(
    typeof(ERSHttpApiModule),
    typeof(AbpAutofacModule),
    typeof(AbpCachingStackExchangeRedisModule),
    typeof(ERSApplicationModule),
    typeof(ERSEntityFrameworkCoreModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpSwashbuckleModule)
)]
public class ERSHostModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        context.Services.Configure<FormOptions>(x =>
            {
                x.ValueCountLimit = 5000;
                x.KeyLengthLimit = int.MaxValue;
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue;
                x.MultipartHeadersLengthLimit = int.MaxValue;
            });
        context.Services.AddHttpClient();
        // 注册 IMemoryCache 服务
        context.Services.AddMemoryCache();
        //ConfigureConventionalControllers();
        ConfigureAuthentication(context, configuration);
        ConfigureLocalization();
        ConfigureCache(configuration);
        //ConfigureVirtualFileSystem(context);
        ConfigureDataProtection(context, configuration, hostingEnvironment);
        ConfigureCors(context, configuration);
        ConfigureSwaggerServices(context, configuration);

        Configure<MvcOptions>(options =>
        {
            var index = options.Filters.ToList().FindIndex(filter => filter is ServiceFilterAttribute attr && attr.ServiceType.Equals(typeof(AbpExceptionFilter)));
            if (index > -1)
                options.Filters.RemoveAt(index);
            options.Filters.Add(typeof(ExceptionFilter));
        });
    }
    private void ConfigureCache(IConfiguration configuration)
    {
        Configure<AbpDistributedCacheOptions>(options => { options.KeyPrefix = "ERS:"; });
    }

    private void ConfigureVirtualFileSystem(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        if (hostingEnvironment.IsDevelopment())
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.ReplaceEmbeddedByPhysical<ERSDomainSharedModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}ERS.Domain.Shared"));
                options.FileSets.ReplaceEmbeddedByPhysical<ERSDomainModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}ERS.Domain"));
                options.FileSets.ReplaceEmbeddedByPhysical<ERSApplicationContractsModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}ERS.Application.Contracts"));
                options.FileSets.ReplaceEmbeddedByPhysical<ERSApplicationModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}ERS.Application"));
            });
        }
    }

    private void ConfigureConventionalControllers()
    {
        Configure<AbpAspNetCoreMvcOptions>(options =>
        {
            options.ConventionalControllers.Create(typeof(ERSApplicationModule).Assembly);
        });
    }

    /// <summary>
    /// 配置 ASP.NET Core 的认证方式，指定使用自定义的 AzureADAuthenticationHandler 处理 JWT Bearer 认证
    /// </summary>
    /// <param name="context"></param>
    /// <param name="configuration"></param>
    private void ConfigureAuthentication(ServiceConfigurationContext context, IConfiguration configuration)
    {
        //context.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        //    .AddJwtBearer(options =>
        //    {
        //        options.Authority = configuration["IDM:AuthorityUrl"];
        //        options.TokenValidationParameters = new TokenValidationParameters
        //        {
        //            ValidateAudience = false
        //        };
        //        options.RequireHttpsMetadata = false;
        //        //options.Audience = "ERS";
        //    });
        context.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddScheme<JwtBearerOptions, AzureADAuthenticationHandler>(JwtBearerDefaults.AuthenticationScheme, options => { });
    }

    private static void ConfigureSwaggerServices(ServiceConfigurationContext context, IConfiguration configuration)
    {
        //context.Services.AddAbpSwaggerGenWithOAuth(
        //    configuration["AuthServer:Authority"],
        //    new Dictionary<string, string>
        //    {
        //            {"ERS", "ERS API"}
        //    },
        //    options =>
        //    {
        //        options.SwaggerDoc("v1", new OpenApiInfo { Title = "ERS API", Version = "v1" });
        //        //options.DocInclusionPredicate((docName, description) => true);
        //        options.CustomSchemaIds(type => type.FullName);
        //    });

        context.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "ers", Version = "v1" });
            //c.IncludeXmlComments(xmlPath, true);
            var security = new OpenApiSecurityRequirement()
                {
                    { new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference()
                        {
                            Id = "Bearer",
                            Type = ReferenceType.SecurityScheme
                        }
                    }, Array.Empty<string>() }
                };
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"ERS.HttpApi.xml"), true);
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"ERS.Application.Contracts.xml"), true);
            c.AddSecurityRequirement(security);//添加一个必须的全局安全信息，和AddSecurityDefinition方法指定的方案名称要一致，这里是Bearer。
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT授权(数据将在请求头中进行传输) 参数结构: \"Authorization: Bearer {token}\"",
                Name = "Authorization",//jwt默认的参数名称
                In = ParameterLocation.Header,//jwt默认存放Authorization信息的位置(请求头中)
                Type = SecuritySchemeType.ApiKey
            });
            c.OrderActionsBy(o => o.RelativePath);
        });
    }

    /// <summary>
    /// 配置语言资源
    /// </summary>
    private void ConfigureLocalization()
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            //注册支持的语言
            //第一个参数要等同于Accept-Language；要和本地JSON文件（如 zh.json）中的 "culture" 字段一致
            options.Languages.Add(new LanguageInfo("zh-TW", "zh-TW", "繁體中文(台灣)"));
            options.Languages.Add(new LanguageInfo("zh-CN", "zh-CN", "简体中文"));
            options.Languages.Add(new LanguageInfo("en", "en", "English"));
        });
    }

    private void ConfigureDataProtection(
        ServiceConfigurationContext context,
        IConfiguration configuration,
        IWebHostEnvironment hostingEnvironment)
    {
        var dataProtectionBuilder = context.Services.AddDataProtection().SetApplicationName("ERS");
        if (!hostingEnvironment.IsDevelopment())
        {
            var redis = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]);
            dataProtectionBuilder.PersistKeysToStackExchangeRedis(redis, "ERS-Protection-Keys");
        }
    }

    private void ConfigureCors(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder
                    .WithOrigins(
                        configuration["App:CorsOrigins"]
                            .Split(",", StringSplitOptions.RemoveEmptyEntries)
                            .Select(o => o.RemovePostFix("/"))
                            .ToArray()
                    )
                    .WithAbpExposedHeaders()
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        if (env.IsDevelopment())
        {

            //app.UseDeveloperExceptionPage();
        }
        if (context.GetConfiguration()["ShowSwagger"]?.ToString() == "true")
        {
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ers v1"));
        }
        var configuration = context.GetConfiguration();
        if (configuration.GetSection("ElasticApm:Open").Value == "true")
            app.UseAllElasticApm(configuration);

        /**
         * ABP多语言支持中间件
         * 根据请求头中的语言 (比如 Accept-Language) 动态解析并应用适当的语言设置
         */
        app.UseAbpRequestLocalization();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseCors();
        app.UseAuthentication();
        app.UseHttpsRedirection();
        //if (MultiTenancyConsts.IsEnabled)
        //{
        //    app.UseMultiTenancy();
        //}

        app.UseAuthorization();

        app.UseSwagger();

        app.UseMiddleware<RequestContextMiddleware>();
        //app.UseAuditing();
        app.UseAbpSerilogEnrichers();
        app.UseUnitOfWork();
        app.UseConfiguredEndpoints();
    }
}
