using ERS.Job.Util;
using ERS.OracleToPostgreSQL;

namespace ERS.Job.Jobs.DataSyncJobs
{
    public class SAP_EXCH_RATESyncJob : IJobBase
    {
        private IOracleToPgsqlRepository _oracleToPgsqlRepository;
        public SAP_EXCH_RATESyncJob(IOracleToPgsqlRepository oracleToPgsqlRepository)
        {
            _oracleToPgsqlRepository = oracleToPgsqlRepository;
        }
        public async Task Run()
        {
             _oracleToPgsqlRepository.OracleToPgsqlDataSync("7_SAP_EXCH_RATE.xml","ERSORADB");
        }
    }
}