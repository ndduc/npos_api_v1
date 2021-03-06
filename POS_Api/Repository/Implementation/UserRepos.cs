using MySql.Data.MySqlClient;
using POS_Api.Database.MySql.Configuration;
using POS_Api.Model;
using POS_Api.Repository.Interface;
using POS_Api.Shared.DbHelper;
using POS_Api.Shared.ExceptionHelper;
using POS_Api.Shared.Security;
using System;
using System.Collections.Generic;
using System.Reflection;


namespace POS_Api.Repository.Implementation
{
    public class UserRepos : BaseHelper, IUserRepos
    {
        public bool VerifyUIdUnique(string uid)
        {
            string id = null;
            string query = "SELECT uid FROM asset_user WHERE uid = " + DbHelper.SetDBValue(uid, true) + ";";
            this.Conn = new DBConnection();
            if (Conn.IsConnect())
            {
                Cmd = new MySqlCommand(query, this.Conn.Connection);
                Reader = Cmd.ExecuteReader();
                while (Reader.Read())
                {
                    id = DbHelper.TryGet(Reader, "uid");
                }
                Conn.Close();
            }
            else
            {
                throw DbConnException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name));
            }
            if (id == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool VerifyUserName(string userName)
        {
            this.Conn = new DBConnection();
            string id = null;
            string query = "SELECT uid FROM asset_user WHERE BINARY UserName = " + DbHelper.SetDBValue(userName, true) + ";";
            if (Conn.IsConnect())
            {
                Cmd = new MySqlCommand(query, this.Conn.Connection);
                Reader = Cmd.ExecuteReader();
                while (Reader.Read())
                {
                    id = DbHelper.TryGet(Reader, "uid");
                }
                Conn.Close();
            }
            else
            {
                throw DbConnException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name));
            }
            if (id == null)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public string VerifyUserAuthority(string uid)
        {
            Conn = new DBConnection();
            string userType = null;
            string query = "SELECT usertype FROM asset_user WHERE BINARY uid = " + DbHelper.SetDBValue(uid, true) + ";";
            if (Conn.IsConnect())
            {
                Cmd = new MySqlCommand(query, this.Conn.Connection);
                Reader = Cmd.ExecuteReader();
                while (Reader.Read())
                {
                    userType = DbHelper.TryGet(Reader, "usertype");
                }
                Conn.Close();
                return userType;
            }
            else
            {
                return null;
            }
        }

        public bool VerifyUser(string uid)
        {
            this.Conn = new DBConnection();
            string id = null;
            string query = "SELECT uid FROM asset_user WHERE uid = " + DbHelper.SetDBValue(uid, true) + ";";
            if (Conn.IsConnect())
            {
                Cmd = new MySqlCommand(query, this.Conn.Connection);
                Reader = Cmd.ExecuteReader();
                while (Reader.Read())
                {
                    id = DbHelper.TryGet(Reader, "uid");
                }
                this.Conn.Close();
            }
            else
            {
                this.Conn.Close();
                throw DbConnException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name));
            }
            return CheckExistingHelper(id);
        }

        public bool AddUserExecution(UserModel userModel)
        {
            int res = 0;
            Conn = new DBConnection();
            string query = "INSERT INTO asset_user"
                + " (`uid`, `username`, `password`, `firstname`, `lastname`, `email`, `email2`, `phone`, `address`, `added_by`, `usertype`) "
                + " VALUES ( "
                + DbHelper.SetDBValue(userModel.UId, false)
                + DbHelper.SetDBValue(userModel.UserName, false)
                + DbHelper.SetDBValue(userModel.Password, false)
                + DbHelper.SetDBValue(userModel.FirstName, false)
                + DbHelper.SetDBValue(userModel.LastName, false)
                + DbHelper.SetDBValue(userModel.Email, false)
                + DbHelper.SetDBValueNull(userModel.Email2, false)
                + DbHelper.SetDBValueNull(userModel.Phone, false)
                + DbHelper.SetDBValueNull(userModel.Address, false)
                + DbHelper.SetDBValueNull(userModel.AddedBy, false)
                + DbHelper.SetDBValue(userModel.UserType.ToUpper(), true)
                + " );";
            try
            {
                if (Conn.IsConnect())
                {
                    Cmd = new MySqlCommand(query, this.Conn.Connection);
                    res = Cmd.ExecuteNonQuery();
                }
                Conn.Close();
            }
            catch (Exception e)
            {
                throw DbInsertException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, e.ToString()));
            }
            return CheckInsertionHelper(res);
        }

        public UserModel GetUserByPassAndUserNameExecution(string userName, string pass)
        {
            this.Conn = new DBConnection();
            UserModel model = null;
            string query = "SELECT * FROM asset_user WHERE BINARY username = " + DbHelper.SetDBValue(userName, true) + ";";
            if (Conn.IsConnect())
            {
                Cmd = new MySqlCommand(query, this.Conn.Connection);
                Reader = Cmd.ExecuteReader();
                try
                {
                    while (Reader.Read())
                    {
                        if (Reader.GetString("uid") != null)
                        {
                            string decryptedPass = HashingPassword.DecryptCipherTextToPlainText(Reader.GetString("password"));
                            if (decryptedPass == pass)
                            {
                                model = UserModelHelper.GetUserData(Reader);
                            }
                            else
                            {
                                model = UserModelHelper.GetUserDataHaveError(false, false, null);
                            }

                        }
                        else
                        {
                            model = UserModelHelper.GetUserDataHaveError(false, false, null);
                        }

                    }
                    Conn.Close();
                }
                catch (Exception e)
                {
                    model = UserModelHelper.GetUserDataHaveError(false, true, "Internal Error - " + e.ToString());
                }

            }
            else
            {
                model = UserModelHelper.GetUserDataHaveError(false, true, "Connection Failure - DB");
            }
            return model;
        }

        public bool UpdateUserExecution(UserModel userModel)
        {
            int res = 0;
            this.Conn = new DBConnection();
            string query = "UPDATE asset_user SET "
                + " email = " + DbHelper.SetDBValue(userModel.Email, false)
                + " email2 = " + DbHelper.SetDBValue(userModel.Email2, false)
                + " phone = " + DbHelper.SetDBValue(userModel.Phone, false)
                + " address = " + DbHelper.SetDBValue(userModel.Address, false)
                + " usertype = " + DbHelper.SetDBValue(userModel.UserType.ToUpper(), true)
                + " WHERE uid = " + DbHelper.SetDBValue(userModel.UId, true);
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
                throw DbUpdateException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, e.ToString()));
            }
            return CheckInsertionHelper(res);
        }

        public bool UpdatePasswordExecution(UserModel userModel)
        {
            int res = 0;
            Conn = new DBConnection();
            string query = "UPDATE asset_user SET "
                + " password = " + DbHelper.SetDBValue(HashingPassword.EncryptPlainTextToCipherText(userModel.Password), true)
                + " WHERE uid = " + DbHelper.SetDBValue(userModel.UId, true);
            try
            {
                if (Conn.IsConnect())
                {
                    Cmd = new MySqlCommand(query, this.Conn.Connection);
                    res = Cmd.ExecuteNonQuery();
                }
                Conn.Close();
            }
            catch (Exception e)
            {
                throw DbUpdateException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, e.ToString()));
            }
            return CheckInsertionHelper(res);
        }

        public bool IsRelationLocationUserExist(string userId, string locationId, string reason)
        {
            string id = null;
            try
            {
                this.Conn = new DBConnection();
                string query = "SELECT id FROM REF_LOCATION_USER"
                                + " WHERE user_uid = " + DbHelper.SetDBValue(userId, true) + " AND "
                                + " location_uid = " + DbHelper.SetDBValue(locationId, true) + " AND "
                                + " relation_reason = " + DbHelper.SetDBValue(reason, true) + ";";
                if (Conn.IsConnect())
                {
                    Cmd = new MySqlCommand(query, this.Conn.Connection);
                    Reader = Cmd.ExecuteReader();
                    while (Reader.Read())
                    {
                        id = DbHelper.TryGet(Reader, "id");
                    }
                    Conn.Close();
                }
                else
                {
                    throw DbConnException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name));
                }
            }
            catch (Exception e)
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, e.ToString()));
            }
            return CheckExistingHelper(id);
        }

        public int GetUserPaginationCount(string locationId, string whereClause)
        {
            Conn = new DBConnection();
            string query = " SELECT count(*) as count "
                        + " FROM asset_user as AU "
                        + " INNER JOIN "
                        + " ( "
                        + " SELECT RLU.user_uid, "
                        + " group_concat(RLU.location_uid) as location_uid  "
                        + " FROM ref_location_user as RLU "
                        + " INNER JOIN asset_location as AL "
                        + " ON RLU.location_uid = AL.uid "
                        + " GROUP BY RLU.user_uid "
                        + " ) as LOCATION "
                        + " ON AU.uid = LOCATION.user_uid"
                        + " AND LOCATION.location_uid LIKE '%"
                        + locationId
                        + "%' "
                        + whereClause;
            int count = 0;
            if (Conn.IsConnect())
            {
                Cmd = new MySqlCommand(query, this.Conn.Connection);
                Reader = Cmd.ExecuteReader();
                while (Reader.Read())
                {
                    count = int.Parse(DbHelper.TryGet(Reader, "count"));
                }
                Conn.Close();
            }
            else
            {
                Conn.Close();
            }
            return count;
        }

        public IEnumerable<UserLocationModel> GetUserPagination(string locationId, string whereClause)
        {
            Conn = new DBConnection();
            List<UserLocationModel> userList = new List<UserLocationModel>();
            string query = " SELECT  AU.*, LOCATION.* "
                        + " FROM asset_user as AU "
                        + " INNER JOIN "
                        + " ( "
                        + " SELECT RLU.user_uid, "
                        + " group_concat(RLU.location_uid) as location_uid  "
                        + " FROM ref_location_user as RLU "
                        + " INNER JOIN asset_location as AL "
                        + " ON RLU.location_uid = AL.uid "
                        + " GROUP BY RLU.user_uid "
                        + " ) as LOCATION "
                        + " ON AU.uid = LOCATION.user_uid"
                        + " AND LOCATION.location_uid LIKE '%"
                        + locationId
                        + "%' "
                        + whereClause;
   
            if (Conn.IsConnect())
            {
                Cmd = new MySqlCommand(query, this.Conn.Connection);
                Reader = Cmd.ExecuteReader();
                while (Reader.Read())
                {
                    UserLocationModel model = new UserLocationModel() { 
                        UId = DbHelper.TryGet(Reader, "uid"),
                        UserName = DbHelper.TryGet(Reader, "username"),
                        Password = HashingPassword.DecryptCipherTextToPlainText(DbHelper.TryGet(Reader, "password")), // descript needed
                        FirstName = DbHelper.TryGet(Reader, "firstname"),
                        LastName = DbHelper.TryGet(Reader, "lastname"),
                        Email = DbHelper.TryGet(Reader, "email"),
                        Email2 = DbHelper.TryGet(Reader, "email2"),
                        Phone = DbHelper.TryGet(Reader, "phone"),
                        Address = DbHelper.TryGet(Reader, "address"),
                        UserType = DbHelper.TryGet(Reader, "usertype"),
                        AddedDateTime = DbHelper.TryGet(Reader, "added_datetime"),
                        UpdatedDateTime = DbHelper.TryGet(Reader, "updated_datetime"),
                        AddedBy = DbHelper.TryGet(Reader, "added_by"),
                        UpdatedBy = DbHelper.TryGet(Reader, "updated_by"),
                        LocationIds = Convert.ToString(DbHelper.TryGet(Reader, "location_uid")).Split(',')
                    };
                    userList.Add(model);
                }
                Conn.Close();
                return userList;
            }
            else
            {
                Conn.Close();
                throw DbConnException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name));
            }
        }

        public bool AddRelationLocationUser(string muserId, string userId, string locationId, string reason)
        {
            bool isAuthrozied;
            bool isRelationExist;
            string authority;
            if (muserId == null)
            {
                authority = VerifyUserAuthority(userId);
                isRelationExist = IsRelationLocationUserExist(userId, locationId, reason);
                isAuthrozied = true;
            }
            else
            {
                authority = VerifyUserAuthority(muserId);
                isRelationExist = IsRelationLocationUserExist(userId, locationId, reason);
                isAuthrozied = true;
            }

            if (isAuthrozied && authority != null && !isRelationExist)
            {
                return AddRelationLocationUserExecution(userId, locationId, reason, authority);
            }
            else
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
