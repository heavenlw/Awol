using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace iphone_Booking
{
    internal class MysqlHelper
    {
        private String connStr = Settings.Default.DBCon_local;
         

        internal void Insert(IPhone iphone)
        {
            string sql = string.Format("insert into iphone (place,size,storage,datetime) values ('{0}','{1}','{2}','{3}')",iphone.Place,iphone.Size,iphone.Storge,DateTime.Now);
            string error = null;
            MySqlConnection conn = null;
            try
            {
                conn = new MySqlConnection(connStr);
                conn.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn);
                DataSet testDataSet = new DataSet();
                adapter.Fill(testDataSet, "result_data");
                conn.Close();
            }
            catch (Exception e)
            {
                conn.Close();
                error = e.Message;
                Console.WriteLine("insert error");
            }
        }
    }
}