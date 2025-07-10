using ERS.Job.Util;
using ERS.UberToERS;


namespace ERS.Job.Jobs.DataSyncJobs
{
    public class Uber_UploadExpenseInfoJob : IJobBase
    {
        private IConfiguration _configuration;
        private IUberToERSRepository _uberToERSRepository;
        public Uber_UploadExpenseInfoJob(IConfiguration Configuration, IUberToERSRepository uberToERSRepository)
        {
            _configuration = Configuration;
            _uberToERSRepository = uberToERSRepository;
        }
        public async Task Run()
        {
            try
            {
                // Perform the job logic here
                Console.WriteLine("Running Uber_UploadExpenseInfoJob...");
                //取得所有部門並直接轉換成 CSV 格式
                string csv = _uberToERSRepository.GetDeptIdToCsv();

                //上傳公司目前的部門(直接CSV上傳到Uber的SFTP)
                if (!string.IsNullOrEmpty(csv))
                {
                    // 初始化 SFTP 工具
                    SFTPUtil sftpUtil = new SFTPUtil(_configuration);
                    sftpUtil.UberUpload(SFTPUtil.UberFolder.toUber_EexpenseCodes, csv);
                    int csvlineCount = csv.Split('\n', StringSplitOptions.RemoveEmptyEntries).Length;
                    Console.WriteLine("upload Uber expense info Count: " + csvlineCount.ToString()); 
                    sftpUtil.Disconnect();
                    Console.WriteLine("Scuessfully upload Uber expense info");
                }
                else
                {
                    Console.WriteLine("No data to upload for Uber expense info.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Uber_UploadExpenseInfoJob: {ex.Message}");
            }
        }
    }
}