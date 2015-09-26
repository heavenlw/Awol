using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
namespace iphone_Booking
{
    internal class HandleHelper
    {
        List<IPhone> iphone_list = new List<IPhone>();
        string phone_size = Settings.Default.Iphone;
        List<IPhone> have_list = new List<IPhone>();
        internal void GetDetail(string html)
        {
            string place = Regex.Match(html, "(R.*?)\"", RegexOptions.Singleline).Groups[1].ToString();
            MatchCollection matches = Regex.Matches(html, "\"(MK.*?)\".*?\"(.*?)\"", RegexOptions.Singleline);
            
            foreach (Match mac in matches)
            {
                string iphone_name = mac.Groups[1].ToString().Trim();
                string have = mac.Groups[2] .ToString().Trim();
                IPhone iphone = new IPhone();
                iphone.Name = iphone_name;
                iphone.Have = have;
                iphone.Place = place;
                iphone_list.Add(iphone);
            }

        }

        internal void Handle()
        {
            foreach (IPhone iphone in iphone_list)
            {
                if (iphone.Name.Contains("MKQ"))
                {
                    iphone.Size = "IPhone6s";

                }
                else
                    iphone.Size = "IPhone6s Plus";
                Match mac = Regex.Match(phone_size, string.Format("{0}:(.*?):(.*?);", iphone.Name.Trim()), RegexOptions.Singleline);
                if (mac != null)
                {
                    iphone.Storge = mac.Groups[1].ToString();
                    iphone.Color = mac.Groups[2].ToString();  
                }
                if (iphone.Place == "R499")
                    iphone.Place = "Canton Road";
                if (iphone.Place == "R409")
                    iphone.Place = "Causeway Bay";
                if (iphone.Place == "R485")
                    iphone.Place = "Festival Walk";
                if (iphone.Place == "R428")
                    iphone.Place = "ifc mall";
                if (iphone.Have == "ALL")
                {
                    have_list.Add(iphone);
                }
            }
        }

        internal void Result()
        {
            foreach (IPhone iphone in have_list)
            {
                Console.WriteLine(iphone.Size+iphone.Storge+iphone.Color+"---"+iphone.Place);
            }
        }
    }
}