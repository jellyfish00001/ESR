namespace ERS.Job.Model
{
    public class JobInfo
    {
        public string JobName { get; set; }
        public bool IsEnable { get; set; }
        public string JobClass { get; set; }
        public string Cron { get; set; }
    }
}
