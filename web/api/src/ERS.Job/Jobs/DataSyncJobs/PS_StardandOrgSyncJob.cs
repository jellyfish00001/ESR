using ERS.Job.Util;
using ERS.OracleToPostgreSQL;

namespace ERS.Job.Jobs.DataSyncJobs
{
    public class PS_StardandOrgSyncJob : IJobBase
    {
        private IOracleToPgsqlRepository _oracleToPgsqlRepository;
        public PS_StardandOrgSyncJob(IOracleToPgsqlRepository oracleToPgsqlRepository)
        {
            _oracleToPgsqlRepository = oracleToPgsqlRepository;
        }
        public async Task Run()
        {
             _oracleToPgsqlRepository.OracleToPgsqlDataSync("0_DB_PS_StardandOrg2.xml");
        }
    }
}