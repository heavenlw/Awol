using System;
using System.Collections.Generic;
using System.Threading;

namespace 凶屋地图爬虫
{
    internal class HandleHelper
    {
      
        internal void Start()
        {
            MysqlHelper mysqlhelper = new MysqlHelper();
              List<Area> area_list =  mysqlhelper.GetList();
            string html = "";
            foreach (Area one_area in area_list)
            {
                html += string.Format("<li>{0}.{1}</li>",one_area.Id,one_area.Name);
                DetailHelper detailhelper = new DetailHelper();
              //  detailhelper.GetInfo(one_area);
               // Thread.Sleep(20000);

            }

        }
    }
}