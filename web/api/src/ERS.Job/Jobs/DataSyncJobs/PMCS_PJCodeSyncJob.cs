using ERS.Job.Util;
using ERS.OracleToPostgreSQL;

namespace ERS.Job.Jobs.DataSyncJobs
{
    public class PMCS_PJCodeSyncJob : IJobBase
    {
        private IOracleToPgsqlRepository _oracleToPgsqlRepository;
        public PMCS_PJCodeSyncJob(IOracleToPgsqlRepository oracleToPgsqlRepository)
        {
            _oracleToPgsqlRepository = oracleToPgsqlRepository;
        }
        public async Task Run()
        {
             _oracleToPgsqlRepository.OracleToPgsqlDataSync("6_PMCS_PJCODE.xml","ERSORADB");
        }
    }
}