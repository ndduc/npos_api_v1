using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS_Api.Database.MySql.Configuration
{
    public class DBConfig
    {
        private readonly string db = "nail_pos_dev";
        private readonly string uName = "c#api";
        private readonly string pass = "1856";
        private readonly string host = "192.168.1.2";
        private readonly string port = "3306";

        public string GetConfiguration()
        {
            return "Data Source=" + host + "," 
                + port + ";Initial Catalog=" + db 
                + ";User ID=" + uName + ";Password=" + pass+ ";";
        }
    }
}
