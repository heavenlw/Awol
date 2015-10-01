using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
namespace iphone_Booking
{
    internal class HandleHelper
    {
        List<Mail> mail_list = new List<Mail>();
        
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

        internal void Clear()
        {
            iphone_list.Clear();
            have_list.Clear();
        }

        internal void AddMail()
        {
            Mail mail = new Mail();
            mail.Name = "heavenlw@msn.cn";
            mail.DateTime = DateTime.Now;
            mail.Status = 1;
            mail_list.Add(mail);
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
                if (iphone.Color == "玫瑰金")
                {
                    //Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!" + "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!" + "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                    Console.WriteLine(iphone.Size + iphone.Storge + iphone.Color + "---" + iphone.Place);
                    // SendMessage(iphone, "玫瑰金来了~！");
                    Console.Beep(700, 500);
                    //MysqlHelper mysqlhelper = new MysqlHelper();
                    //mysqlhelper.Insert(iphone);
                }
                if (iphone.Color == "金" && iphone.Storge == "64g")
                {
                    //Console.Beep(500, 300);
                    //SendMessage(iphone, "64g 金色来了~！");
                }
                if (iphone.Color == "金" && iphone.Size== "IPhone6s Plus")

                {
                    //SendMessage(iphone, "金色PLus来了~！");
                }
                if (iphone.Size.Contains("Plus"))
                {
                   // Console.WriteLine(iphone.Size + iphone.Storge + iphone.Color + "---" + iphone.Place);
                }
                else
                { }
                //Console.WriteLine(iphone.Size+iphone.Storge+iphone.Color+"---"+iphone.Place);
            }
        }

        private void SendMessage(IPhone iphone,string title)
        {
            try
            {
                foreach (Mail mail in mail_list)
                {
                    if (mail.Status != 1)
                    {
                        if ((DateTime.Now - mail.DateTime).Minutes > 3)
                        {
                            mail.Status = 1;
                            mail.DateTime = DateTime.Now;
                        }
                    }
                    if (mail.Status == 1)
                    {

                        MailMessage emailMessage;
                        MailMessage message = new MailMessage("heavenlw@sina.com",mail.Name, title, iphone.Place + iphone.Storge);
                        string fuwuqi = "smtp.sina.com";

                        SmtpClient smtp = new SmtpClient(fuwuqi);
                        smtp.Credentials = new NetworkCredential("heavenlw@sina.com", "550804648");
                        smtp.Send(message);
                        mail.Status = -1;
                    }
                }
            }
            catch (Exception t)
            {
                Console.WriteLine(t.Message);
            }
        }
    }
}