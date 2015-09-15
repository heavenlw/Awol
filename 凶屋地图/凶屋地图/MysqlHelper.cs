using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace 凶屋地图
{
    internal class MysqlHelper
    {

        private static String connStr = Properties.Settings.Default.DBCon_local;
        internal static void Insert(string value, string area)
        {
            string sql = string.Format("insert into area (name,value) value ('{0}','{1}')",area,value);
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
            }
            catch (Exception e)
            {
                conn.Close();
                error = e.Message;
                Console.WriteLine("insert Area error------>{0}", error);
                
            }
        }
    }
}