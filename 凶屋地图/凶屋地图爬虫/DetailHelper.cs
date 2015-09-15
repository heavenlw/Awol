using System;
using System.Net;
using System.Text.RegularExpressions;
namespace 凶屋地图爬虫
{
    internal class DetailHelper
    {
      

        internal void GetInfo(Area one_area)
        {
            for (int page= 1;page <= 100;page++)
            {
                string url = string.Format("http://www.property.hk/unlucky/{0}/{1}/0/",page,one_area.Value);
                {
                    HttpWebRequest requestScore = (HttpWebRequest)WebRequest.Create(url);
                    string html = Awol.WebHelper.GetResponseStr(requestScore,"utf-8",null,null);
                    if (html != null && html.Length > 100)
                    {
                        MatchCollection matches = Regex.Matches(html, "<TR bgColor.*?</TR>",RegexOptions.Singleline);
                        if (matches.Count < 3)
                        {
                            break;
                        }
                        foreach (Match mac in matches)
                        {
                            MatchCollection details = Regex.Matches(mac.ToString(), "<TD.*?>(.*?)</TD>", RegexOptions.Singleline);
                            if (details.Count < 3)
                            {
                                continue;
                            }
                            string date = details[0].Groups[1].ToString().Trim();
                            string place = details[1].Groups[1].ToString().Trim();
                            string level = details[2].Groups[1].ToString().Trim();
                            string reason = details[3].Groups[1].ToString().Trim();
                            string detail_url = details[4].ToString().Trim();
                            detail_url = HandleUrl(detail_url, url);
                            string only_code = Regex.Match(detail_url, "www.property.hk/unlucky_detail/.*?/(.*?.html)", RegexOptions.Singleline).Groups[1].ToString(); ;
                            if (level.Length == 0)
                                level = "-1";

                            News news = new News();
                            
                            news.Date = GetDate(date);
                            news.Place = place;
                            news.Level = level;
                            news.Reason = reason;
                            news.Detail_Url = detail_url;
                            news.Only = only_code;
                            news.Area_Id = one_area.Id;
                            MysqlHelper mysqlhelper = new MysqlHelper();
                            mysqlhelper.InertNews(news);
                         
                        }
                    }
                }
            }

        }

        private DateTime? GetDate(string date)
        {
            if (date != null && date.Length > 1)
            {
                try
                {
                  return   Convert.ToDateTime(date);
                }
                catch (Exception t)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private string HandleUrl(string detail_url, string url)
        {
            detail_url = Regex.Match(detail_url, "href='(.*?)'").Groups[1].ToString() ;
            Uri uri = new Uri(new Uri(url), detail_url);
            return uri.AbsoluteUri;
        }
    }
}