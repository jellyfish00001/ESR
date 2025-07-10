using ERS.Job.Util;
using ERS.UberToERS;

namespace ERS.Job.Jobs.DataSyncJobs
{
    public class Uber_DownloadTransactionByDaillyJob : IJobBase
    {
        private IConfiguration _configuration;
        private IUberToERSRepository _uberToERSRepository;
        public Uber_DownloadTransactionByDaillyJob(IConfiguration Configuration, IUberToERSRepository uberToERSRepository)
        {
            _configuration = Configuration;
            _uberToERSRepository = uberToERSRepository;
        }
        public async Task Run()
        {
            try
            {
                // Perform the job logic here
                Console.WriteLine("Running Uber_DownloadTransactionByDaillyJob...");
                // 初始化 SFTP 工具
                SFTPUtil sftpUtil = new SFTPUtil(_configuration);
                //指定日期下載交易檔
                //sftpUtil.dailyTripsDate = "2025_05_15";
                sftpUtil.UberFolderAction(SFTPUtil.UberFolder.fromUberTripsDailly);
                Console.WriteLine("Scuessfully download Uber transaction by dailly");

                _uberToERSRepository.CsvInsDB(sftpUtil.fileName, sftpUtil.fileContent);
                Console.WriteLine("Scuessfully CsvInsDB By dailly");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Uber_DownloadTransactionByDaillyJob: {ex.Message}");
            }
        }

    }
}