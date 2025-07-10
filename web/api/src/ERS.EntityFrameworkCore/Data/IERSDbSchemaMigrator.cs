using System.Threading.Tasks;

namespace ERS.Data;

public interface IERSDbSchemaMigrator
{
    Task MigrateAsync();
}
