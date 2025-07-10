using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace ERS.Data;

/* This is used if database provider does't define
 * IERSDbSchemaMigrator implementation.
 */
public class NullERSDbSchemaMigrator : IERSDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
