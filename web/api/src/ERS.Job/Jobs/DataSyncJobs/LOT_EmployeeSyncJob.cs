using ERS.Job.Util;
using ERS.OracleToPostgreSQL;

namespace ERS.Job.Jobs.DataSyncJobs
{
    public class LOT_EmployeeSyncJob : IJobBase
    {
        private IOracleToPgsqlRepository _oracleToPgsqlRepository;

        public LOT_EmployeeSyncJob(IOracleToPgsqlRepository oracleToPgsqlRepository)
        {
            _oracleToPgsqlRepository = oracleToPgsqlRepository;
        }

        public async Task Run()
        {
             _oracleToPgsqlRepository.OracleToPgsqlDataSync("4_DB_LOT_Employee.xml");
        }
    }
}