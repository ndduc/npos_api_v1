using MySql.Data.MySqlClient;
using POS_Api.Core.Interface;
using POS_Api.Database.MySql.Configuration;
using POS_Api.Model;
using POS_Api.Model.EnumData;
using POS_Api.Shared.DbHelper;
using POS_Api.Shared.ExceptionHelper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace POS_Api.Core.Implementation
{
    public class LocationUserRelationLogic : BaseHelper, ILocationUserRelationLogic
    {
        
        private readonly IUserLogic _userLogic;

        public LocationUserRelationLogic(IUserLogic userLogic)
        {
            _userLogic = userLogic;
        }

        public bool IsRelationLocationUserExist(string userId, string locationId, string reason)
        {
            string id = null;
            try
            {
                this.Conn = new DBConnection();
                string query = "SELECT id FROM REF_LOCATION_USER"
                                + " WHERE user_uid = " + DbHelper.SetDBValue(userId, true)  + " AND "
                                + " location_uid = " + DbHelper.SetDBValue(locationId, true) + " AND "
                                + " relation_reason = " + DbHelper.SetDBValue(reason, true) + ";";
                if(Conn.IsConnect())
                {
                    Cmd = new MySqlCommand(query, this.Conn.Connection);
                    Reader = Cmd.ExecuteReader();
                    while (Reader.Read())
                    {
                        id = DbHelper.TryGet(Reader, "id");
                    }
                    Conn.Close();
                } else
                {
                    throw DbConnException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name));
                }
            } catch (Exception e)
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, e.ToString()));
            }
            return CheckExistingHelper(id);
        }

        public bool AddRelationLocationUser(string muserId, string userId, string locationId, string reason)
        {
            bool isAuthrozied;
            bool isRelationExist;
            string authority;
            if (muserId == null)
            {
                authority = _userLogic.VerifyUserAuthority(userId);
                isRelationExist = IsRelationLocationUserExist(userId, locationId, reason);
                isAuthrozied = true;
            }
            else
            {
                authority = _userLogic.VerifyUserAuthority(muserId);
                isRelationExist = IsRelationLocationUserExist(userId, locationId, reason);
                isAuthrozied = true;
            }

            if (isAuthrozied && authority != null && !isRelationExist)
            {
                return AddRelationLocationUserExecution(userId, locationId, reason, authority);
            } else
            {
                throw DbInsertException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "unauthorized access"));
            }

            
        }
        private bool AddRelationLocationUserExecution(string userId, string locationId, string reason, string authority)
        {
            bool isAuthrozied = false;
            string createdById = userId;
            // Use to verify user authority
            int res = 0;
            Conn = new DBConnection();
            string query = "INSERT INTO REF_LOCATION_USER "
                + " (`user_uid`,`location_uid`,`relation_reason`, `added_by`)"
                + " VALUES ( "
                + DbHelper.SetDBValue(userId, false)
                + DbHelper.SetDBValue(locationId, false)
                + DbHelper.SetDBValue(reason, false)
                + DbHelper.SetDBValue(createdById, true)
                + " );";
            try
            {
                if (Conn.IsConnect())
                {
                    Cmd = new MySqlCommand(query, this.Conn.Connection);
                    res = Cmd.ExecuteNonQuery();
                }
                this.Conn.Close();
            }
            catch (Exception e)
            {
                throw new Exception("Error While Inserting Data\t\t" + e.ToString());
            }
            return CheckInsertionHelper(res);



        }

    }
}
