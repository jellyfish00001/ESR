namespace ERS.OracleToPostgreSQL
{
    public interface IOracleToPgsqlRepository
    {
        void OracleToPgsqlDataSync(string xmlFileName,string db = "PSDB");
        // DataSet GetXMLSet(string xmlStr);
        void OracleToPgsqlDataSyncForBankAccount(string xmlFileName, string db = "PSDB");
    }
}