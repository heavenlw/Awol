using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awol
{
   public  static  class  MathHelper
    { /// <summary>
      /// 返回0.0-1.0之间的随机数，效果等同于Javascript的Math.Random()
      /// </summary>
      /// <returns></returns>
        public static double MathRandom()
        {
            Random rnd = new Random();
            return rnd.NextDouble();
        }

        /// <summary>
        /// 唯一数字序列字符串，19位长
        /// </summary>
        /// <returns></returns>
        public static string GuidToInt64String()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            return BitConverter.ToInt64(buffer, 0).ToString();
        }

        /// <summary>
        /// 唯一序列字符串，32位长
        /// </summary>
        /// <returns></returns>
        public static string GuidToString()
        {
            return System.Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// 组合唯一序列字符串，32位长
        /// </summary>
        /// <returns></returns>
        public static string GuidCombToString(string format = "")
        {
            return GenerateComb().ToString(format);
        }

        /// <summary> /// Generate a new <see cref="Guid"/> using the comb algorithm. 
        /// </summary> 
        public static Guid GenerateComb()
        {
            byte[] guidArray = Guid.NewGuid().ToByteArray();

            DateTime baseDate = new DateTime(1900, 1, 1);
            DateTime now = DateTime.Now;

            // Get the days and milliseconds which will be used to build    
            //the byte string    
            TimeSpan days = new TimeSpan(now.Ticks - baseDate.Ticks);
            TimeSpan msecs = now.TimeOfDay;

            // Convert to a byte array        
            // Note that SQL Server is accurate to 1/300th of a    
            // millisecond so we divide by 3.333333    
            byte[] daysArray = BitConverter.GetBytes(days.Days);
            byte[] msecsArray = BitConverter.GetBytes((long)
              (msecs.TotalMilliseconds / 3.333333));

            // Reverse the bytes to match SQL Servers ordering    
            Array.Reverse(daysArray);
            Array.Reverse(msecsArray);

            // Copy the bytes into the guid    
            Array.Copy(daysArray, daysArray.Length - 2, guidArray,
              guidArray.Length - 6, 2);
            Array.Copy(msecsArray, msecsArray.Length - 4, guidArray,
              guidArray.Length - 4, 4);

            return new Guid(guidArray);
        }

        /// <summary>
        /// 中文转换成阿拉伯数字
        /// </summary>
        /// <param name="strChinese">中文数字，如 一百零八</param>
        /// <returns>阿拉伯数字，如 108</returns>
        public static int ChineseToArab(string strChinese)
        {
            string e = "零一二三四五六七八九";
            string[] ew = new string[] { "十", "百", "千" };
            string[] ej = new string[] { "万", "亿" };
            int a = 0;
            if (strChinese.IndexOf(ew[0]) == 0)
                a = 10;
            strChinese = System.Text.RegularExpressions.Regex.Replace(strChinese, e[0].ToString(), "");
            if (System.Text.RegularExpressions.Regex.IsMatch(strChinese, "([" + e + "])$"))
            {
                a += e.IndexOf(System.Text.RegularExpressions.Regex.Match(strChinese, "([" + e + "])$").Value[0]);
            }
            if (System.Text.RegularExpressions.Regex.IsMatch(strChinese, "([" + e + "])" + ew[0]))
            {
                a += e.IndexOf(System.Text.RegularExpressions.Regex.Match(strChinese, "([" + e + "])" + ew[0]).Value[0]) * 10;
            }
            if (System.Text.RegularExpressions.Regex.IsMatch(strChinese, "([" + e + "])" + ew[1]))
            {
                a += e.IndexOf(System.Text.RegularExpressions.Regex.Match(strChinese, "([" + e + "])" + ew[1]).Value[0]) * 100;
            }
            if (System.Text.RegularExpressions.Regex.IsMatch(strChinese, "([" + e + "])" + ew[2]))
            {
                a += e.IndexOf(System.Text.RegularExpressions.Regex.Match(strChinese, "([" + e + "])" + ew[2]).Value[0]) * 1000;
            }
            return a;
        }
    }
}
