using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Threading;

namespace iphone_Booking
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("start" + DateTime.Now);
                string url = "https://reserve.cdn-apple.com/HK/zh_HK/reserve/iPhone/availability.json";
                HttpWebRequest requestScore = (HttpWebRequest)WebRequest.Create(url);
                string html = Awol.WebHelper.GetResponseStr(requestScore, "utf-8", null, null);
                MatchCollection matches = Regex.Matches(html, "\"R.*?\"MKUD2ZP/A\".*?}", RegexOptions.Singleline);
                HandleHelper handlehelper = new HandleHelper();
                handlehelper.AddMail();
                foreach (Match mac in matches)
                {

                    handlehelper.GetDetail(mac.ToString());
                }
                handlehelper.Handle();
                handlehelper.Result();
                Thread.Sleep(5000);

            }
        }
    }
}
