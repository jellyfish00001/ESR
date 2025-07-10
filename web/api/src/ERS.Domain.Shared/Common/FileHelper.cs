using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;

namespace ERS
{
    public class FileHelper
    {
        public static bool IsImage(string name)
        {
            bool result = true;
            string[] temp = name.Split(".");
            result = "jpg,jpeg,png,gif,bmp,tiff,svg".Split(",").Any(i => temp[temp.Length - 1].Contains(i, StringComparison.OrdinalIgnoreCase));
            return result;
        }
        public static bool IsPdf(string name)
        {
            bool result = true;
            string[] temp = name.Split(".");
            result = "pdf".Split(",").Any(i => temp[temp.Length - 1].Contains(i, StringComparison.OrdinalIgnoreCase));
            return result;
        }
        public static bool IsHtml(string name)
        {
            bool result = true;
            string[] temp = name.Split(".");
            result = "htm,html".Split(",").Any(i => temp[temp.Length - 1].Contains(i, StringComparison.OrdinalIgnoreCase));
            return result;
        }
        public static bool IsExcel(string name)
        {
            bool result = true;
            string[] temp = name.Split(".");
            result = "xlsx,xls".Split(",").Any(i => temp[temp.Length - 1].Contains(i, StringComparison.OrdinalIgnoreCase));
            return result;
        }

        public static byte[] StreamToBytes(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }

        public static Stream BytesToStream(byte[] bytes)
        {
            using (Stream stream = new MemoryStream(bytes))
            {
                return stream;
            }
        }
        public static IFormFile ReadFile(string path)
        {
            var fileName = "模板.xlsx";
            FileStream excelFS = new FileStream(path, FileMode.Open);
            IFormFile r = new FormFile(excelFS, 0, excelFS.Length, "模板", fileName);
            return r;
        }
        public static IFormFile ReadFile(Stream stream)
        {
            IFormFile r = new FormFile(stream, 0, stream.Length, "模板", "模板.xlsx");
            return r;
        }
        public static String GetContentType(String FilenameExtension)
        {
            if (FilenameExtension.Equals(".BMP") || FilenameExtension.Equals(".bmp")
                    || FilenameExtension.ToUpper().Equals(".BMP"))
            {
                return "image/bmp";
            }
            if (FilenameExtension.Equals(".GIF") || FilenameExtension.Equals(".gif")
                    || FilenameExtension.ToUpper().Equals(".GIF"))
            {
                return "image/gif";
            }
            if (FilenameExtension.Equals(".JPEG") || FilenameExtension.Equals(".jpeg") || FilenameExtension.Equals(".JPG")
                    || FilenameExtension.Equals(".jpg") || FilenameExtension.Equals(".PNG")
                    || FilenameExtension.Equals(".png") || FilenameExtension.ToUpper().Equals(".JPEG")
                    || FilenameExtension.ToUpper().Equals(".JPG") || FilenameExtension.ToUpper().Equals(".PNG"))
            {
                return "image/jpeg";
            }
            if (FilenameExtension.Equals(".HTML") || FilenameExtension.Equals(".html"))
            {
                return "text/html";
            }
            if (FilenameExtension.Equals(".TXT") || FilenameExtension.Equals(".txt")
                    || FilenameExtension.ToUpper().Equals(".TXT"))
            {
                return "text/plain";
            }
            if (FilenameExtension.Equals(".VSD") || FilenameExtension.Equals(".vsd")
                    || FilenameExtension.ToUpper().Equals(".VSD"))
            {
                return "application/vnd.visio";
            }
            if (FilenameExtension.Equals(".PPTX") || FilenameExtension.Equals(".pptx") || FilenameExtension.Equals(".PPT")
                    || FilenameExtension.Equals(".ppt") || FilenameExtension.ToUpper().Equals(".PPTX")
                    || FilenameExtension.ToUpper().Equals(".PPT"))
            {
                return "application/vnd.ms-powerpoint";
            }
            if (FilenameExtension.Equals(".DOCX") || FilenameExtension.Equals(".docx") || FilenameExtension.Equals(".DOC")
                    || FilenameExtension.Equals(".doc") || FilenameExtension.ToUpper().Equals(".DOCX")
                    || FilenameExtension.ToUpper().Equals(".DOC"))
            {
                return "application/msword";
            }
            if (FilenameExtension.Equals(".XML") || FilenameExtension.Equals(".xml")
                    || FilenameExtension.ToUpper().Equals(".XML"))
            {
                return "text/xml";
            }
            if (FilenameExtension.Equals(".pdf") || FilenameExtension.Equals(".PDF")
                    || FilenameExtension.ToUpper().Equals(".PDF"))
            {
                return "application/pdf";
            }
            return null;
        }
    }
}
