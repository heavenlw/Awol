using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace 凶屋地图地区Api.Controllers
{
    internal class MysqlHelper
    {
        private static String connStr = System.Configuration.ConfigurationManager.AppSettings["Conntction"];
        private static String connStr_local = System.Configuration.ConfigurationManager.AppSettings["conn"];
        internal string GetDetail(string t)
        {
            string html = "<table border=\"1\">< tr >< td width = \"150\" > Date </ td >< td width = \"180\" > Place </ td >< td width = \"780\">Reason</td></tr>";
            List<string> list_return = new List<string>();
            string sql = string.Format("select * from topic where area_id = {0}", t);
            DataSet testDataSet = null;
            MySqlConnection conn = new MySqlConnection(connStr_local);

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
                    string place = testRow["place"].ToString();
                    string date = testRow["date"].ToString();
                    string name = testRow["reason"].ToString();
                    News news = new News();
                    news.Place = place;
                    news.Value = date;
                    news.Name = name;
                    html += string.Format("<tr>< td >{0}</ td >< td >{1}</ td >< td >{1}</ td ></ tr >", date, place, name);
                }


            }
            html += "</ table >";
            return html;
        }
    }
}