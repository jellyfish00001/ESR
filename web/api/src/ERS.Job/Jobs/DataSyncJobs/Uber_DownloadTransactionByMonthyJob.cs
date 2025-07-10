using ERS.Job.Util;
using ERS.UberToERS;

namespace ERS.Job.Jobs.DataSyncJobs
{
    public class Uber_DownloadTransactionByMonthyJob : IJobBase
    {
        private IConfiguration _configuration;
        private IUberToERSRepository _uberToERSRepository;
        public Uber_DownloadTransactionByMonthyJob(IConfiguration Configuration, IUberToERSRepository uberToERSRepository)
        {
            _configuration = Configuration;
            _uberToERSRepository = uberToERSRepository;
        }
        public async Task Run()
        {
               try
            {
                // Perform the job logic here
                Console.WriteLine("Running Uber_DownloadTransactionByMonthyJob...");
                // 初始化 SFTP 工具
                SFTPUtil sftpUtil = new SFTPUtil(_configuration);
                sftpUtil.UberFolderAction(SFTPUtil.UberFolder.fromUberTripsMonthy);
                Console.WriteLine("Scuessfully download Uber transaction by monthy");

                _uberToERSRepository.CsvInsDB(sftpUtil.fileName, sftpUtil.fileContent ,false);
                Console.WriteLine("Scuessfully CsvInsDB By Monthy");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Uber_DownloadTransactionByMonthyJob: {ex.Message}");
            }
        }
    }
}