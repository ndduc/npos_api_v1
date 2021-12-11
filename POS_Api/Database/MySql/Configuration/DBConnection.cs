using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS_Api.Database.MySql.Configuration
{
    public class DBConnection
    {
        public MySqlConnection Connection { get; set; }
        private static DBConnection _instance = null;
        public static DBConnection Instance()
        {
            if (_instance == null)
                _instance = new DBConnection();
            return _instance;
        }

        public bool IsConnect()
        {
            if (Connection == null)
            {
                try
                {
                    string connstring = new DBConfig().GetConfiguration();
                    Connection = new MySqlConnection(connstring);
                    Connection.Open();
                }
                catch (Exception err)
                {
                    Console.WriteLine("ERROR\t\t" + err);
                    return false;
                }

            }

            return true;
        }

        public void Close()
        {
            Connection.Close();
        }

    }
}
