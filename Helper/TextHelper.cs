using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App.Utilities
{
    public static class TextHelper
    {
        /// <summary>
        /// UTF8编码的字符转换为GBK编码
        /// </summary>
        /// <param name="text">源字符</param>
        /// <returns>GBK编码字符</returns>
        public static string UTF8ToGBK(string text)
        {
            byte[] utfBytes = Encoding.UTF8.GetBytes(text);
            byte[] gbkBytes = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding("GBK"), utfBytes);
            return Encoding.GetEncoding("GBK").GetString(gbkBytes);
        }

        /// <summary>
        /// 根据源与目标编码转换字符串
        /// </summary>
        /// <param name="text">源字符</param>
        /// <param name="sourceCode">源编码</param>
        /// <param name="targetCode">目标编码</param>
        /// <returns>转换编码后的字符</returns>
        public static string ConvertEncodingTo(string text, string sourceCode, string targetCode)
        {
            byte[] sourceBytes = Encoding.GetEncoding(sourceCode).GetBytes(text);
            byte[] targetBytes = Encoding.Convert(Encoding.GetEncoding(sourceCode), Encoding.GetEncoding(targetCode), sourceBytes);
            return Encoding.GetEncoding(targetCode).GetString(targetBytes);
        }

        /// <summary>
        /// GBK编码的字符转换为UTF8编码
        /// </summary>
        /// <param name="text"></param>
        /// <returns>UTF8编码字符</returns>
        public static string GBKToUTF8(string text)
        {
            byte[] gbkBytes = Encoding.GetEncoding("GBK").GetBytes(text);
            byte[] utfBytes = Encoding.Convert(Encoding.GetEncoding("GBK"), Encoding.UTF8, gbkBytes);
            return Encoding.UTF8.GetString(utfBytes);
        }

        /// <summary>
        /// 清除HTML代码
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string CleanHTML(string text)
        {
            var regex = new System.Text.RegularExpressions.Regex(@"(<[a-zA-Z].*?>)|(<[\/][a-zA-Z].*?>)");
            var content = regex.Replace(text, string.Empty);
            content = content.Replace("&nbsp;", " ");
            return content;
        }

        /// <summary>
        /// 清除UTF16的字符，范围 \uD800-\uDFFF
        /// </summary>
        /// <returns></returns>
        public static string RemoveUTF16Char(string text)
        {
            var regex = new System.Text.RegularExpressions.Regex(@"[^\u0000-\uD7FF\uE000-\uFFFF]");
            var content = regex.Replace(text, string.Empty);
            return content;
        }

        /// <summary>  
        /// 编辑距离（Levenshtein Distance）  
        /// </summary>  
        /// <param name="source">源串</param>  
        /// <param name="target">目标串</param>  
        /// <param name="similarity">输出：相似度，值在0～１</param>  
        /// <param name="isCaseSensitive">是否大小写敏感</param>  
        /// <returns>源串和目标串之间的编辑距离</returns>  
        public static Int32 LevenshteinDistance(String source, String target, out Double similarity, Boolean isCaseSensitive = false)
        {
            if (String.IsNullOrEmpty(source))
            {
                if (String.IsNullOrEmpty(target))
                {
                    similarity = 1;
                    return 0;
                }
                else
                {
                    similarity = 0;
                    return target.Length;
                }
            }
            else if (String.IsNullOrEmpty(target))
            {
                similarity = 0;
                return source.Length;
            }

            String From, To;
            if (isCaseSensitive)
            {   // 大小写敏感  
                From = source;
                To = target;
            }
            else
            {   // 大小写无关  
                From = source.ToLower();
                To = target.ToLower();
            }

            // 初始化  
            Int32 m = From.Length;
            Int32 n = To.Length;
            Int32[,] H = new Int32[m + 1, n + 1];
            for (Int32 i = 0; i <= m; i++) H[i, 0] = i;  // 注意：初始化[0,0]  
            for (Int32 j = 1; j <= n; j++) H[0, j] = j;

            // 迭代  
            for (Int32 i = 1; i <= m; i++)
            {
                Char SI = From[i - 1];
                for (Int32 j = 1; j <= n; j++)
                {   // 删除（deletion） 插入（insertion） 替换（substitution）  
                    if (SI == To[j - 1])
                        H[i, j] = H[i - 1, j - 1];
                    else
                        H[i, j] = Math.Min(H[i - 1, j - 1], Math.Min(H[i - 1, j], H[i, j - 1])) + 1;
                }
            }

            // 计算相似度  
            Int32 MaxLength = Math.Max(m, n);   // 两字符串的最大长度  
            similarity = ((Double)(MaxLength - H[m, n])) / MaxLength;

            return H[m, n];    // 编辑距离  
        }
    }
}
