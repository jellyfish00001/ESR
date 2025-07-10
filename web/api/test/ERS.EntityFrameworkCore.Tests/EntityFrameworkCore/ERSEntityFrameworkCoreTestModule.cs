using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Sqlite;
using Volo.Abp.Modularity;

namespace ERS.EntityFrameworkCore;

[DependsOn(
    typeof(ERSEntityFrameworkCoreModule),
    typeof(ERSTestBaseModule),
    typeof(AbpEntityFrameworkCoreSqliteModule)
    )]
public class ERSEntityFrameworkCoreTestModule : AbpModule
{
    private SqliteConnection _sqliteConnection;

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        //ConfigureInMemorySqlite(context.Services);
        ConfigureInMemory(context.Services);
    }

    private void ConfigureInMemory(IServiceCollection services)
    {
        services.Configure<AbpDbContextOptions>(options =>
        {
            options.Configure(context =>
            {
                context.DbContextOptions.UseInMemoryDatabase("DB", option =>
                {
                    DbContextOptions<ERSDbContext> _option = new DbContextOptionsBuilder<ERSDbContext>().UseInMemoryDatabase("DB").Options;
                    using (var context = new ERSDbContext(_option))
                    {
                        context.Database.EnsureCreated();
                    }
                });
            });
        });
    }

    private void ConfigureInMemorySqlite(IServiceCollection services)
    {
        _sqliteConnection = CreateDatabaseAndGetConnection();

        services.Configure<AbpDbContextOptions>(options =>
        {
            options.Configure(context =>
            {
                context.DbContextOptions.UseSqlite(_sqliteConnection);
                // context.DbContextOptions.UseInMemoryDatabase("DB");
            });
        });
    }

    public override void OnApplicationShutdown(ApplicationShutdownContext context)
    {
        if (_sqliteConnection != null)
            _sqliteConnection.Dispose();
    }

    private static SqliteConnection CreateDatabaseAndGetConnection()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<ERSDbContext>()
            .UseSqlite(connection)
            .Options;

        using (var context = new ERSDbContext(options))
        {
            context.GetService<IRelationalDatabaseCreator>().CreateTables();
        }

        return connection;
    }
}
