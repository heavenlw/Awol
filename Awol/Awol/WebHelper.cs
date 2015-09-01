using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Awol
{
   public static class WebHelper
    {
        /// <summary>
        /// 获取HTML源码中设定的网页编码
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string GetEncoding(string html)
        {
            string strEncoding = null;

            if (html != null)
            {
                Match charsetMatch = Regex.Match(html, "(?<=charset\\s*=\"*)[^\"]+(?=\")", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                if (charsetMatch.Success)
                {
                    strEncoding = charsetMatch.Value.Trim();
                }
            }

            return strEncoding;
        }
        /// <summary>
        /// 将相对路径转换为绝对路径
        /// </summary>
        /// <param name="node"></param>
        public static void RelativeUriToAbsoluteUri(Uri baseUri, HtmlNode node)
        {
            var nodes = node.SelectNodes(".//@href");
            if (nodes != null)
            {
                foreach (var item in nodes)
                {
                    var url = item.GetAttributeValue("href", null);
                    if (url != null)
                    {
                        try
                        {
                            item.SetAttributeValue("href", new Uri(baseUri, url).AbsoluteUri);
                        }
                        catch (Exception)
                        {

                        }

                    }
                }
            }

            nodes = node.SelectNodes(".//@src");
            if (nodes != null)
            {
                foreach (var item in nodes)
                {
                    var url = item.GetAttributeValue("src", null);
                    if (url != null)
                    {
                        item.SetAttributeValue("src", new Uri(baseUri, url).AbsoluteUri);
                    }
                }
            }
        }
        /// <summary>
        /// Get方法获取Html
        /// </summary>
        /// <param name="request">HttpWebRequest类，包含header</param>
        /// <param name="encoding">网页的编码</param>
        /// <param name="proxy">代理</param>
        /// <param name="timeout">默认设置为2000，可随意</param>
        /// <returns></returns>
        public static string GetResponseStr(HttpWebRequest request, string encoding, WebProxy proxy,int? timeout)
        {
            string html = null;
            try
            {
                if (timeout == null || timeout < 2000)
                {
                    timeout = 2000;
                }

              
                request.Timeout = timeout.Value;
                request.AllowAutoRedirect = true;
                if (proxy != null)
                    request.Proxy = proxy;
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
                if (response.ContentEncoding == "gzip")
                {
                    //StreamReader sr1 = new StreamReader(response1.GetResponseStream(), Encoding.UTF8);
                    //是否要解压缩stream？
                    html = GetResponseByZipStream(response);
                    //Console.WriteLine(res);
                }
                else
                {
                    html = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                html = e.Message;
            }

            return html;
        }

        private static string GetResponseByZipStream(HttpWebResponse response)
        {

            GZipStream g = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress);
            StreamReader test = new StreamReader(g, Encoding.UTF8);
            return test.ReadToEnd();
        }
        /// <summary>
        ///  Post方法获取html
        /// </summary>
        /// <param name="postData">post数据</param>
        /// <param name="uriStr">目标url</param>
        /// <param name="proxy">代理</param>
        /// <returns></returns>
        internal static string requestPost(string postData, string uriStr,WebProxy proxy)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            //postData = Uri.EscapeDataString(postData);
            string post_str = postData;
            byte[] bytes;
            bytes = encoding.GetBytes(post_str);


            HttpWebRequest requestScore = (HttpWebRequest)WebRequest.Create(uriStr);
            requestScore.Method = "POST";
            //requestScore.Proxy = proxy;
            byte[] data = Encoding.GetEncoding("utf-8").GetBytes(postData);
            requestScore.ContentType = "application/x-www-form-urlencoded";
            requestScore.ContentLength = data.Length;
            //使用cookies
            //requestScore.CookieContainer = ...;
            Stream stream = requestScore.GetRequestStream();
            stream.Write(bytes, 0, bytes.Length);
            stream.Close();
            HttpWebResponse response = (HttpWebResponse)requestScore.GetResponse();
            string res = null;
            StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            res = sr.ReadToEnd();
            return res;
        }
    }
}
