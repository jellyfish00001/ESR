
using ERS.Job;
using Serilog;

IConfiguration Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();

Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(Configuration)
        .CreateLogger();
var builder = WebApplication.CreateBuilder(args);
builder.Host.UseAutofac().UseSerilog();
await builder.AddApplicationAsync<ERSJobModule>();
var app = builder.Build();
await app.InitializeApplicationAsync();

app.Run();
