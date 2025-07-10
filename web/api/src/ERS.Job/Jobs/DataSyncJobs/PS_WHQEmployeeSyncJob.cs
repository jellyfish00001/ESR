using ERS.Job.Util;
using ERS.OracleToPostgreSQL;

namespace ERS.Job.Jobs.DataSyncJobs
{
    public class PS_WHQEmployeeSyncJob : IJobBase
    {
        private IOracleToPgsqlRepository _oracleToPgsqlRepository;

        public PS_WHQEmployeeSyncJob(IOracleToPgsqlRepository oracleToPgsqlRepository)
        {
            _oracleToPgsqlRepository = oracleToPgsqlRepository;
        }

        public async Task Run()
        {
             _oracleToPgsqlRepository.OracleToPgsqlDataSync("3_DB_PS_WHQEmployee.xml");
        }
    }
}