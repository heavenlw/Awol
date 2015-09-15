using System;
using System.Text.RegularExpressions;

namespace 凶屋地图
{
    internal class HandleHelper
    {
        public HandleHelper()
        {
        }

        internal void Start()
        {
            MysqlHelper mysqlhelper = new MysqlHelper();
            string area_string = Properties.Settings.Default.Area;
            MatchCollection matches = Regex.Matches(area_string,"value=\"(.*?)\".*?>(.*?)</option>",RegexOptions.Singleline);
            foreach (Match mac in matches)
            {
                if (mac.Groups[1].Length < 2)
                    continue;
                else
                {
                    string value = mac.Groups[1].ToString().Trim();
                    string area = mac.Groups[2].ToString().Trim();
                    MysqlHelper.Insert(value, area);
                }
            }

        }
    }
}