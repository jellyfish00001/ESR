using ERS.Job.Util;
using ERS.OracleToPostgreSQL;

namespace ERS.Job.Jobs.DataSyncJobs
{
    public class Bank_Account_PRDSyncJob : IJobBase
    {
        private IOracleToPgsqlRepository _oracleToPgsqlRepository;

        public Bank_Account_PRDSyncJob(IOracleToPgsqlRepository oracleToPgsqlRepository)
        {
            _oracleToPgsqlRepository = oracleToPgsqlRepository;
        }

        public async Task Run()
        {
             _oracleToPgsqlRepository.OracleToPgsqlDataSyncForBankAccount("05_DB_Bank_Account_PRD.xml");
        }
    }
}