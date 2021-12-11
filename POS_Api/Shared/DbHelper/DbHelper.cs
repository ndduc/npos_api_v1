using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS_Api.Shared.DbHelper
{
    public static class DbHelper
    {
        public static readonly string quote = "'";
        public static readonly string comma = ",";
        public static readonly string null_value = "null";
        public static dynamic TryGet(MySqlDataReader reader, string index)
        {
            try
            {
                return reader.GetString(index);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string SetDBValueNull(dynamic value, bool isLast)
        {
            string return_value = null;
            if (value == null)
            {
                return_value = null_value;
            } else
            {
                return_value = quote + value + quote;
            }

            if(isLast)
            {
                return return_value;
            } else
            {
                return return_value + comma;
            }
        }

        public static string SetDBValue(dynamic value, bool isLast)
        {
            string return_value = quote + value + quote;
            if (isLast)
            {
                return return_value;
            }
            else
            {
                return return_value + comma;
            }
        }
    }
}
