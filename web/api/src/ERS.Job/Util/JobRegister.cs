using ERS.Job.Model;
using Hangfire;
using Microsoft.Extensions.Options;

namespace ERS.Job.Util
{
    public class JobRegister
    {
        private readonly IConfiguration _configuration;

        private readonly IOptions<List<JobInfo>> _jobInfos;

        private readonly IServiceProvider _app;

        private readonly IEnumerable<IJobBase> _jobBase;

        public JobRegister(IConfiguration configuration, IOptions<List<JobInfo>> jobInfos,
         IServiceProvider app, IEnumerable<IJobBase> jobBase)
        {
            _configuration = configuration;
            _jobInfos = jobInfos;
            _app = app;
            _jobBase = jobBase;
        }

        public void RegisterJobsFromConfig()
        {
            foreach (var item in _jobInfos.Value)
            {
                if (!item.IsEnable) continue;
                var service = _jobBase.Where(o => o.GetType().Name == item.JobClass).FirstOrDefault();
                if (service == null) continue;
                RecurringJob.AddOrUpdate(() => service.Run(), item.Cron, TimeZoneInfo.Local);
            }
        }
    }
}
