using System;

namespace 凶屋地图爬虫
{
    internal class News
    {
        public News()
        {
        }

        public string Area_Id { get; internal set; }
        public DateTime? Date { get; internal set; }
        public string Detail_Url { get; internal set; }
        public string Level { get; internal set; }
        public string Only { get; internal set; }
        public string Place { get; internal set; }
        public string Reason { get; internal set; }
    }
}