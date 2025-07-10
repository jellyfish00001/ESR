using System.Collections.Generic;
using System.IO;
namespace ERS.Model.MobileSign
{
    public class MobileSignFile
    {
        public string fileName { get; set; }
        public string fileSize { get; set; }
    }
    public class SignFile
    {
        public string fileName { get; set; }
        public string filestring { get; set; }
        public string fileSize { get; set; }
    }
    public class MobileSignFileRequset
    {
        public string messageID { get; set; }
        public string File { get; set; }
    }
    public class MobileSignFileRespose
    {
        public MobileSignFileResposeData data { get; set; }
        public int rtnCode { get; set; }
        public string rtnMsg { get; set; }
    }
    public class MobileSignFileResposeData
    {
        public string fileId { get; set; }
        public string messageId { get; set; }
        public string fileName { get; set; }
    }
    public class MobileSignFileForm
    {
        public FileInfo File { get; set; }
        public string MessageId { get; set; }
    }
    public class MobileSendReturn
    {
        public string data { get; set; }
        public int rtnCode { get; set; }
        public string rtnMsg { get; set; }
    }
    public class MobileCancelFormRequset
    {
        public string serviceID { get; set; }
        public string authCode { get; set; }
        public string messageID { get; set; }
    }
    public class MobileCancelFormRespose
    {
        public int rtnCode { get; set; }
        public string rtnMsg { get; set; }
    }
    public class MobileAttInfoRequest
    {
        public string serviceID { get; set; }
        public string authCode { get; set; }
        public string formID { get; set; }
    }
    public class MobileAttInfoRespose
    {
        public List<MobileFile> data { get; set; }
        public string rtnMsg { get; set; }
        public int rtnCode { get; set; }
    }
    public class MobileFile
    {
        public string fileId { get; set; }
        public string fileName { get; set; }
        public string fileSize { get; set; }
        public string procCode { get; set; }
        public string convertFileName { get; set; }
        public object[] details { get; set; }
    }
}
