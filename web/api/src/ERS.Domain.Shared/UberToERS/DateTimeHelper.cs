using System;
using System.Globalization;

namespace ERS.UberToERS
{
    public static class DateTimeHelper
    {

        // 假定 ConvertToDateFormat 和 ConvertTo24HourFormat 已经定义并返回对应格式的字符串

        public static DateTime ConvertToDateTime(string datePart, string timePart)
        {
            // 组合日期和时间字符串
            string dateTimeString = $"{datePart} {timePart}";

            // 定义完整的日期时间格式
            string format = "yyyy/MM/dd HH:mm:ss";

            // 使用 TryParseExact 来解析日期时间字符串
            if (DateTime.TryParseExact(dateTimeString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
            {
                return dateTime;
            }
            else
            {
                throw new FormatException("Date and time string format is incorrect.");
            }
        }
        // 方法1：将日期字符串转换为 "yyyy-MM-dd" 格式
        public static string ConvertToDateFormat(string dateString)
        {
            if (DateTime.TryParse(dateString, out DateTime result))
            {
                return result.ToString("yyyy/MM/dd");
            }
            throw new FormatException("无法将输入字符串解析为日期格式");
        }

        // 方法2：将时间字符串转换为 24 小时制并返回 "HH:mm:ss"
        public static string ConvertTo24HourFormat(string timeString)
        {
            string format = "h:mmtt"; // 定义输入格式
            if (DateTime.TryParseExact(timeString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
            {
                return result.ToString("HH:mm:ss");
            }
            throw new FormatException("无法将输入字符串解析为时间格式");
        }
    }
}
