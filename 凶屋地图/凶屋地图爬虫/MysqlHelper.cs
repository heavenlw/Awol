using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace 凶屋地图爬虫
{
    internal class MysqlHelper
    {
        private static String connStr = Properties.Settings.Default.DBCon_local;
        internal List<Area> GetList()
        {
            List<Area> list_return = new List<Area>();
            string sql = string.Format("select * from area");
            DataSet testDataSet = null;
            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                conn.Open();
                // 创建一个适配器
                MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn);
                // 创建DataSet，用于存储数据.
                testDataSet = new DataSet();
                // 执行查询，并将数据导入DataSet.
                adapter.Fill(testDataSet, "result_data");
            }
            // 关闭数据库连接.
            catch (Exception e)
            {
                //log4net.ILog log = log4net.LogManager.GetLogger("MyLogger");
                //log.Debug(e.Message);
                Console.WriteLine(e.Message);
                //Console.ReadLine();

            }
            finally
            {
                conn.Close();
            }
            if (testDataSet != null && testDataSet.Tables["result_data"] != null && testDataSet.Tables["result_data"].Rows != null)
            {
                foreach (DataRow testRow in testDataSet.Tables["result_data"].Rows)
                {
                    string id = testRow["id"].ToString();
                    string value = testRow["value"].ToString();
                    string name = testRow["name"].ToString();
                    Area area = new Area();
                    area.Value = value;
                    area.Name = name;
                    area.Id = id;
                    list_return.Add(area);
                }
            }
            return list_return;
        }

        internal void InertNews(News news)
        {
            string sql = string.Format("insert into topic set date='{0}',url='{1}',place='{2}',level='{3}',reason='{4}',code='{5}',area_id='{6}'", news.Date, news.Detail_Url, news.Place, news.Level, news.Reason,news.Only,news.Area_Id);
            string error = null;
            MySqlConnection conn = null;
            try
            {
                conn = new MySqlConnection(connStr);
                conn.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn);
                // 创建DataSet，用于存储数据.
                DataSet testDataSet = new DataSet();
                // 执行查询，并将数据导入DataSet.
                adapter.Fill(testDataSet, "result_data");
                conn.Close();
                Console.WriteLine(news.Place+"--"+news.Reason);
            }
            catch (Exception e)
            {
                conn.Close();
                error = e.Message;
                Console.WriteLine("insert Topic error------>{0}", error);

            }
        }
    }
}