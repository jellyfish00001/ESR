using ERS.Job.Util;
using ERS.UberToERS;

namespace ERS.Job.Jobs.DataSyncJobs
{
    public class Uber_UploadEmployeesJob : IJobBase
    {
        private IConfiguration _configuration;
        private IUberToERSRepository _uberToERSRepository;
        public Uber_UploadEmployeesJob(IConfiguration Configuration, IUberToERSRepository uberToERSRepository)
        {
            _configuration = Configuration;
            _uberToERSRepository = uberToERSRepository;
        }
        public async Task Run()
        {
            try
            {
                // Perform the job logic here
                Console.WriteLine("Running Uber_UploadEmployeesJob...");
                //取得所有員工資料
                IList<ERS.UberToERS.Model.uber_employees> emps = _uberToERSRepository.GetAllEmployees();
                //將在職員工轉換成 CSV 格式
                string addCsv = _uberToERSRepository.GetEmployeesToCsv(emps);
                //將離職員工轉換成 CSV 格式
                string reMoveCsv = _uberToERSRepository.GetEmployeesToCsv(emps, true);

                //
                if (string.IsNullOrEmpty(addCsv) && string.IsNullOrEmpty(reMoveCsv))
                {
                    Console.WriteLine("No data to upload for Uber employees.");
                }
                else
                {
                    // 初始化 SFTP 工具
                    SFTPUtil sftpUtil = new SFTPUtil(_configuration);
                    //上傳此次新增的在職員工
                    if (!string.IsNullOrEmpty(addCsv))
                    {
                        sftpUtil.UberUpload(SFTPUtil.UberFolder.toUber_EmployeesAdd, addCsv);
                        int addCsvlineCount = addCsv.Split('\n', StringSplitOptions.RemoveEmptyEntries).Length;
                        Console.WriteLine("upload Uber employees add Count: " + addCsvlineCount.ToString());
                    }

                    //上傳此次移除的離職員工
                    if (!string.IsNullOrEmpty(reMoveCsv))
                    {
                        sftpUtil.UberUpload(SFTPUtil.UberFolder.toUber_EmployeesRemove, reMoveCsv);
                        int reMoveCsvlineCount = reMoveCsv.Split('\n', StringSplitOptions.RemoveEmptyEntries).Length;
                        Console.WriteLine("upload Uber employees remove Count: " + reMoveCsvlineCount.ToString());
                    }
                    sftpUtil.Disconnect();

                    Console.WriteLine("Scuessfully upload Uber employees");
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Uber_UploadEmployeesJob: {ex.Message}");
            }
        }
    }
}