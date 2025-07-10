namespace ERS.Dapper
{
    public interface IDapperFactory
    {
        DapperHelper CreateConnection(string conn = "Default", DatabaseType databaseType = DatabaseType.PostgreSQL);
    }
}
