using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Awol
{
    public static class DateTimeHelper
    {
        /// <summary>  
        /// 取得某月的第一天  
        /// </summary>  
        /// <param name="datetime">要取得月份第一天的时间</param>  
        /// <returns></returns>  
        public static DateTime GetFirstDayOfMonth(int Year, int Month)
        {
            return Convert.ToDateTime(Year.ToString() + "-" + Month.ToString() + "-1");
        }
        /// <summary>  
        /// 取得某月的最后一天  
        /// </summary>  
        /// <param name="datetime">要取得月份最后一天的时间</param>  
        /// <returns></returns>  
        public static DateTime GetLastDayOfMonth(int Year, int Month)
        {
            //这里的关键就是 DateTime.DaysInMonth 获得一个月中的天数
            int Days = DateTime.DaysInMonth(Year, Month);
            return Convert.ToDateTime(Year.ToString() + "-" + Month.ToString() + "-" + Days.ToString());

        }
        /// <summary>
        /// Unix时间戳转换到普通时间
        /// </summary>
        /// <param name="unixTimeStamp">Unix时间戳</param>
        /// <returns>世界时UTC</returns>
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        /// <summary>
        /// 普通时间转换到Unix时间戳-10位
        /// </summary>
        /// <param name="dateTime">世界时UTC</param>
        /// <returns>Unix时间戳</returns>
        public static long DateTimeToUnixTimeStamp_10(DateTime dateTime)
        {
            //TimeSpan span = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
            //long unixTime = (long)span.TotalSeconds;
            //return unixTime;
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            //10位时间戳
            var timeStamp = (long)(dateTime - startTime).TotalSeconds;
            //13位时间戳
            //var timeStamp = (long)(dateTime - startTime).TotalMilliseconds;
            return timeStamp;

        }
        /// <summary>
        /// 普通时间转换到Unix时间戳-10位
        /// </summary>
        /// <param name="dateTime">世界时UTC</param>
        /// <returns>Unix时间戳</returns>
        public static long DateTimeToUnixTimeStamp_13(DateTime dateTime)
        {
            //TimeSpan span = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
            //long unixTime = (long)span.TotalSeconds;
            //return unixTime;
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            //10位时间戳
            //var timeStamp = (long)(dateTime - startTime).TotalSeconds;
            //13位时间戳
            var timeStamp = (long)(dateTime - startTime).TotalMilliseconds;
            return timeStamp;

        }
        /// <summary>
        /// 从文本中分析时间
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static DateTime? ParseDateTime(string text)
        {
            DateTime? result = null;

            Regex regex;
            Match match;

            regex = new Regex(@"((19|20)[0-9]{2}-\d+-\d+\s+\d+:\d+:\d+)|((19|20)[0-9]{2}-\d+-\d+\s+\d+:\d+)|((19|20)[0-9]{2}-\d+-\d+\s+\d+)|((19|20)[0-9]{2}-\d+-\d+)");
            match = regex.Match(text);
            if (match.Success)
            {
                result = DateTime.Parse(match.Captures[0].Value);
                return result;
            }

            regex = new Regex(@"((19|20)[0-9]{2}/\d+/\d+\s+\d+:\d+:\d+)|((19|20)[0-9]{2}/\d+/\d+\s+\d+:\d+)|((19|20)[0-9]{2}/\d+/\d+\s+\d+)|((19|20)[0-9]{2}/\d+/\d+)");
            match = regex.Match(text);
            if (match.Success)
            {
                result = DateTime.Parse(match.Captures[0].Value);
                return result;
            }

            regex = new Regex(@"((19|20)[0-9]{2}\.\d+\.\d+\s+\d+:\d+:\d+)|((19|20)[0-9]{2}\.\d+\.\d+\s+\d+:\d+)|((19|20)[0-9]{2}\.\d+\.\d+\s+\d+)|((19|20)[0-9]{2}\.\d+\.\d+)");
            match = regex.Match(text);
            if (match.Success)
            {
                result = DateTime.Parse(match.Captures[0].Value);
                return result;
            }

            regex = new Regex(@"(\d+年\d+月\d+日\s+\d+:\d+:\d+)|(\d+年\d+月\d+日\s+\d+:\d+)|(\d+年\d+月\d+日)");
            match = regex.Match(text);
            if (match.Success)
            {
                result = DateTime.Parse(match.Captures[0].Value);
                return result;
            }

            //格式：14-02-18 16:02:00
            regex = new Regex(@"(\d{1,2}-\d{1,2}-\d{1,2}\s+\d{1,2}:\d{1,2}:\d{1,2})");
            match = regex.Match(text);
            if (match.Success)
            {
                result = DateTime.Parse(match.Captures[0].Value);
                return result;
            }

            regex = new Regex(@"(?<value>((19|20)[0-9]{2}\d{2}\d{2}))");
            match = regex.Match(text);
            if (match.Success)
            {
                DateTime date;
                if (DateTime.TryParseExact(match.Captures[0].Value, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date))
                {
                    result = date;
                    return result;
                }
            }

            //格式：2月14日 13:07
            regex = new Regex(@"(\d{1,2}月\d{1,2}日\s+\d{1,2}:\d{1,2}:\d{1,2})|(\d{1,2}月\d{1,2}日\s+\d{1,2}:\d{1,2})");
            match = regex.Match(text);
            if (match.Success)
            {
                result = DateTime.Parse(match.Captures[0].Value);
                return result;
            }

            return result;
        }
    }
}