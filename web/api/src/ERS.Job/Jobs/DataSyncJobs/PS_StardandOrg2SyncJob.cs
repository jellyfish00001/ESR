using ERS.Job.Util;
using ERS.OracleToPostgreSQL;

namespace ERS.Job.Jobs.DataSyncJobs
{
    public class PS_StardandOrg2SyncJob : IJobBase
    {
        private IOracleToPgsqlRepository _oracleToPgsqlRepository;

        public PS_StardandOrg2SyncJob(IOracleToPgsqlRepository oracleToPgsqlRepository)
        {
            _oracleToPgsqlRepository = oracleToPgsqlRepository;
        }

        public async Task Run()
        {
             _oracleToPgsqlRepository.OracleToPgsqlDataSync("2_DB_PS_StardandOrg2.xml");
        }
    }
}