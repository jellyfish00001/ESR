using Renci.SshNet;
using System.Text;

namespace ERS.Job.Util
{
    public class SFTPUtil
    {
        private readonly SftpClient sftp;
        public string savePath = "";
        public bool isTest = false;
        public string fileContent { get; set; }
        public string fileName { get; set; }
        public string dailyTripsDate { get; set; }
        /// <summary>
        /// UBER 提供的SFTP的目錄
        /// </summary>
        public class UberFolder
        {
            /// <summary>
            /// 員工資料夾-新增(上傳一次後會登記在管理介面後台，但只要不新啟用就會一直發信通知)
            /// </summary>
            public const string toUber_EmployeesAdd = "to_uber/employees/add";
            /// <summary>
            /// 員工資料夾-刪除(上傳後會立即刪除後台名單)
            /// </summary>
            public const string toUber_EmployeesRemove = "to_uber/employees/remove";
            /// <summary>
            /// 費用代碼(也就是我們的部門代碼)
            /// </summary>            
            public const string toUber_EexpenseCodes = "to_uber/expense_codes";
            /// <summary>
            /// 交易記錄檔案-每日
            /// </summary>                
            public const string fromUberTripsDailly = "from_uber/trips";
            /// <summary>
            /// 交易記錄檔案-每月
            /// </summary>                 
            public const string fromUberTripsMonthy = "from_uber/trips/monthly";
            /// <summary>
            /// 交易記錄檔案-報表(目前沒用到)
            /// </summary>              
            public const string fromUberReports = "from_uber/reports";
        }

        public enum FolderAction
        {
            Upload,
            Download,
            Delete

        }

        /// <summary>
        /// SFTP 链接状态
        /// </summary>
        public bool Connected
        {
            get { return sftp.IsConnected; }
        }

        public SFTPUtil(IConfiguration configuration)
        {
            try
            {

                // 從 appsettings.json 中讀取 SFTP 設定
                string hostAddress = configuration["hangfire:SFTP:hostAddress"];
                int port = int.Parse(configuration["hangfire:SFTP:port"]);
                string userAccount = configuration["hangfire:SFTP:userAccount"];
                string privateKeyPath = configuration["hangfire:SFTP:privateKeyPath"];
                savePath = configuration["hangfire:SFTP:savePath"];
                isTest = bool.Parse(configuration["hangfire:SFTP:isTest"]);

                // 使用金鑰檔案初始化 SftpClient 第二個參數passPhrase沒輸入也要加入空字串
                var privateKeyFile = new PrivateKeyFile(privateKeyPath, "");
                var authenticationMethod = new PrivateKeyAuthenticationMethod(userAccount, privateKeyFile);
                var connectionInfo = new Renci.SshNet.ConnectionInfo(hostAddress, port, userAccount, authenticationMethod);
                sftp = new SftpClient(connectionInfo);

                Connect();
                //sftp.ChangeDirectory(_userCwd);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                Console.Error.WriteLine(ex.Message);
            }
        }

        ~SFTPUtil()
        {
            Disconnect();
        }

        /// <summary>
        /// 链接SFTP
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool Connect()
        {
            try
            {
                if (!Connected)
                {
                    sftp.Connect();
                    Console.WriteLine("Connect SFTP");
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                Console.Error.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// 断开SFTP
        /// </summary> 
        public void Disconnect()
        {
            try
            {
                if (sftp != null && Connected)
                {
                    sftp.Disconnect();
                    Console.WriteLine("Disconnect SFTP");
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                throw new Exception(string.Format("断开SFTP失败，原因：{0}", ex.Message));
            }
        }
        /// <summary>
        ///  切换目录
        /// </summary>
        /// <returns></returns>
        public bool ChangeDir(string path)
        {
            try
            {
                if (sftp.IsConnected)
                {
                    sftp.ChangeDirectory(path);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                Console.Error.WriteLine(ex);
                return false;
            }
        }
        public List<string> ListDir(string path)
        {
            List<string> list = new List<string>();
            try
            {
                if (sftp.IsConnected)
                {
                    var files = sftp.ListDirectory(path);
                    foreach (var file in files)
                    {
                        list.Add(file.FullName);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                Console.Error.WriteLine(ex);
            }
            return list;
        }
        /// <summary>
        /// SFTP上传文件
        /// </summary>
        /// <param name="localFile">本地路径</param>
        /// <param name="remoteFile">远程路径</param>
        /// <returns></returns>
        public bool Upload(string localFile, string remoteFile)
        {
            try
            {
                using (var file = File.OpenRead(localFile))
                {
                    sftp.UploadFile(file, remoteFile);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                Console.Error.WriteLine($"上傳失敗：{ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// SFTP上传文件(不儲存檔案)
        /// </summary>
        /// <param name="fileContent">文字內容</param>
        /// <param name="remoteFile">远程路径</param>
        /// <returns></returns>
        public bool UploadByFileContent(string fileContent, string remoteFile)
        {
            try
            {
                // 將字串內容轉成 Stream
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent)))
                {
                    sftp.UploadFile(stream, remoteFile);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                Console.Error.WriteLine($"上傳失敗：{ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// SFTP 下载文件
        /// </summary>
        /// <param name="remoteFile"></param>
        /// <param name="localFile"></param>
        public void Download(string remoteFile, string localFile)
        {
            try
            {
                var byt = sftp.ReadAllBytes(remoteFile);
                fileContent = Encoding.UTF8.GetString(byt);
                File.WriteAllBytes(localFile, byt);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                throw new Exception(string.Format("SFTP文件获取失败，原因：{0}", ex.Message));
            }
        }

        /// <summary>
        /// 删除SFTP文件 
        /// </summary>
        /// <param name="remoteFile">远程路径</param>
        public void Delete(string remoteFile)
        {
            try
            {
                sftp.Delete(remoteFile);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                throw new Exception(string.Format("SFTP文件删除失败，原因：{0}", ex.Message));
            }
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="folderType"></param>
        public bool UberFolderAction(string folderType)
        {
            bool isSuccess = false;
            try
            {
                switch (folderType)
                {
                    case UberFolder.toUber_EmployeesAdd:
                    case UberFolder.toUber_EmployeesRemove:
                    case UberFolder.toUber_EexpenseCodes:
                        UberAction(FolderAction.Upload, folderType);
                        break;
                    case UberFolder.fromUberTripsDailly:
                    case UberFolder.fromUberTripsMonthy:
                    case UberFolder.fromUberReports:
                        // 格式: 2025_05_12
                        //currentDate = (folderType == UberFolder.fromUberTripsDailly) ? DateTime.Now.AddDays(-2).ToString("yyyy_MM_dd") : currentDate;
                        UberAction(FolderAction.Download, folderType);
                        break;
                }
                return isSuccess;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                return isSuccess;
            }
        }
        public void UberUpload(string folderType, string fileContent)
        {
            // 完整的遠端檔案路徑
            string currentDate = DateTime.Now.ToString("yyyy_MM_dd");
            string fileName = $"{currentDate}.csv";
            folderType = (isTest ? "test_" : "") + folderType;
            string remoteRelativeFile = @$"/{folderType}/{fileName}";
            Console.WriteLine($" UberUpload to {remoteRelativeFile} ...");
            UploadByFileContent(fileContent, remoteRelativeFile);
            //Disconnect();
            Console.WriteLine($"UberUpload is successfully.");
        }
        string uberGetLastFile(string folderType, string fileTitle)
        {
            List<String> files = ListDir(folderType);
            var listFiles = files
                .Select(f => new
                {
                    FileName = f,
                    // 取出日期字串並嘗試轉成 DateTime
                    Date = DateTime.TryParseExact(
                        System.IO.Path.GetFileNameWithoutExtension(f).Replace(fileTitle, ""),
                        "yyyy_MM_dd",
                        null,
                        System.Globalization.DateTimeStyles.None,
                        out var dt) ? dt : DateTime.MinValue
                })
                .OrderByDescending(x => x.Date).ToList();
            //.FirstOrDefault()?.FileName;
    
            var objLatestFile = string.IsNullOrEmpty(dailyTripsDate) ? 
                        listFiles.FirstOrDefault() : 
                        listFiles.Where(x => x.Date.ToString("yyyy_MM_dd") == dailyTripsDate).FirstOrDefault();

            // latestFile 就是最近日期的檔案完整路徑字串
            string latestFile = System.IO.Path.GetFileName(objLatestFile?.FileName);
            return latestFile;
        }
        void UberAction(FolderAction action, string destinationPath)
        {
            string actionName = action.ToString();
            string fileTitle = action == FolderAction.Download ? "daily_trips-" : "";
            //判斷路徑是否為fromUberTripsDailly
            //是的話取得最新日期的檔案，若不是直接抓當天日期
            fileName = destinationPath == UberFolder.fromUberTripsDailly ?
                    uberGetLastFile(destinationPath, fileTitle) : // 取得最新的檔案名稱
                    $"{fileTitle}{DateTime.Now.ToString("yyyy_MM_dd")}.csv";  //取該日期的檔案名稱

            //1.判斷是下載的話預設路徑不會有test_
            //2.上傳的話要去判斷isTest的值，有的話路徑會有test_
            destinationPath = (action == FolderAction.Download ? "" : (isTest ? "test_" : "")) + destinationPath;

            // 這裡可以根據 action 的值來決定是上傳還是下載 
            // 本地儲存路徑
            string localFullFile = Path.Combine(@$"{savePath}/{destinationPath}", fileName).Replace("/", "\\");
            // 完整的遠端檔案路徑
            string remoteRelativeFile = @$"/{destinationPath}/{fileName}";

            Console.WriteLine($"{actionName}ing file from {remoteRelativeFile} to {localFullFile}...");

            // 這裡可以根據 action 的值來決定是上傳還是下載
            if (action == FolderAction.Upload)
                Upload(localFullFile, remoteRelativeFile);
            else if (action == FolderAction.Delete)
                Delete(remoteRelativeFile);
            else if (action == FolderAction.Download)
                Download(remoteRelativeFile, localFullFile);

            Disconnect();
            Console.WriteLine($"File {actionName}ed successfully.");
        }
    }
}
