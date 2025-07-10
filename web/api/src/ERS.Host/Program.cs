using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Microsoft.AspNetCore.Hosting;

namespace ERS;

public class Program
{
    static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

    public async static Task<int> Main(string[] args)
    {
        try
        {
            if (Configuration.GetSection("ChangeAppsettingsFile").Value == "true")
            {
                string currpath = Directory.GetCurrentDirectory();
                Console.WriteLine(currpath);
                string parpath = Directory.GetParent(currpath).FullName;
                Console.WriteLine(parpath);
                string w1 = Path.Combine(parpath, "mnt", "secrets-store");
                Console.WriteLine(w1);
                Console.WriteLine(Directory.Exists(w1).ToString());
                w1 = Path.Combine(parpath, "mnt", "secrets-store", "appsettings.json");
                Console.WriteLine(w1);
                Console.WriteLine(Directory.Exists(w1).ToString());
                try
                {
                    File.Copy(w1, Path.Combine(currpath, "appsettings.json"), true);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
                //File.Copy("/app/appsettings.json", "/mnt/secrets-store/appsettings.json", true);
            Console.WriteLine("Starting ERS.HttpApi.Host.");
            //Log.Information("Starting ERS.HttpApi.Host.");
            PrepareLog();
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.UseAutofac().UseSerilog();
            await builder.AddApplicationAsync<ERSHostModule>();
            builder.WebHost.UseKestrel(i => i.Limits.MaxRequestBodySize = null);
            var app = builder.Build();
            var life = app.Services.GetRequiredService<IHostApplicationLifetime>();
            life.ApplicationStopped.Register(() =>
            {
                Log.Information("Application is gracefully shut down");
            });
            await app.InitializeApplicationAsync();
            await app.RunAsync();
            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly!");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static void PrepareLog()
    {
        Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(Configuration)
        .CreateLogger();
    }
}
