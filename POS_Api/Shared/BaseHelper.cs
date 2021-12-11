using MySql.Data.MySqlClient;
using POS_Api.Database.MySql.Configuration;
using POS_Api.Shared.DbHelper;
using POS_Api.Shared.ExceptionHelper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
#pragma warning disable S125 // Sections of code should not be commented out
namespace POS_Api.Shared.ExceptionHelper
{
    public class BaseHelper
    {

        public DBConnection Conn;
        public MySqlCommand Cmd { get; set; }
        public MySqlDataReader Reader { get; set; }


        private readonly string space = "     ";

        // throw DbConnException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name));
        public Exception DbConnException(string e)
        {
            return new Exception("Database Connection Error" + space + e);
        }

        // throw DbInsertException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, e.ToString()));
        // throw DbInsertException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "unauthorized access"));
        public Exception DbInsertException(string e)

        {
            return new Exception("Database Error Occured While Inserting Record" + space + e);
        }

        // throw DbUpdateException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, e.ToString()));
        public Exception DbUpdateException(string e)
        {
            return new Exception("Database Error Occured While Update Record" + space + e);
        }

        public Exception GenericException(string e)
        {
            return new Exception("Generic Error" + space + e);
        }

        public string GenerateExceptionMessage(params string[] values)
        {
            return string.Join("_", values);
        }

        public bool CheckExistingHelper(dynamic response)
        {
            if (response == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool CheckInsertionHelper(int response)
        {
            if (response < 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool CheckUpdateHelper(int response)
        {
            if (response < 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool VerifyNotExist(dynamic response)
        {
            if (response == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

#pragma warning restore S125 // Sections of code should not be commented out



