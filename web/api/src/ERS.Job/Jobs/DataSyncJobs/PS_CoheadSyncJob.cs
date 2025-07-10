using ERS.Job.Util;
using ERS.OracleToPostgreSQL;

namespace ERS.Job.Jobs.DataSyncJobs
{
    public class PS_CoheadSyncJob : IJobBase
    {
        private IOracleToPgsqlRepository _oracleToPgsqlRepository;

        public PS_CoheadSyncJob(IOracleToPgsqlRepository oracleToPgsqlRepository)
        {
            _oracleToPgsqlRepository = oracleToPgsqlRepository;
        }

        public async Task Run()
        {
             _oracleToPgsqlRepository.OracleToPgsqlDataSync("5_DB_PS_Cohead.xml");
        }
    }
}