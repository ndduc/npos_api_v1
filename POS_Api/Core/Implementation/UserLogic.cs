﻿using MySql.Data.MySqlClient;
using POS_Api.Core.Interface;
using POS_Api.Database.MySql.Configuration;
using POS_Api.Model;
using POS_Api.Shared.DbHelper;
using POS_Api.Shared.ExceptionHelper;
using POS_Api.Shared.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace POS_Api.Core.Implementation
{
    public class UserLogic : BaseHelper, IUserLogic
    {

        private readonly ILocationUserRelationLogic _locationRelationLogic;

        public UserLogic(ILocationUserRelationLogic locationRelationLogic)
        {
            _locationRelationLogic = locationRelationLogic;
        }
        public UserLogic() { }

        public bool AddUser(UserModel userModel)
        {
            string id = null;
            bool isIdUnqiue = false;
            bool isSucess = false;
            // Verify whether this id is unique
      
            while(!isIdUnqiue)
            {
                id = Guid.NewGuid().ToString();
                isIdUnqiue = VerifyUIdUnique(id);
            }

            bool isUserNameUnique = VerifyUserName(userModel.UserName);

            if(isIdUnqiue && isUserNameUnique)
            {
                userModel.UId = id;
                //Hash Password
                userModel.Password = HashingPassword.EncryptPlainTextToCipherText(userModel.Password);
                // Insert Record, return false if the insertion false
                isSucess = AddUserExecution(userModel);
                return isSucess;
            } else
            {
                return isSucess;
            }
        }

        public bool AddUserWithParent(UserModel userModel, string parentId, string locationId, string type)
        {
            string id = null;
            bool isIdUnqiue = false;
            bool isSucess = false;
            // Verify whether this id is unique

            while (!isIdUnqiue)
            {
                id = Guid.NewGuid().ToString();
                isIdUnqiue = VerifyUIdUnique(id);
            }

            bool isUserNameUnique = VerifyUserName(userModel.UserName);

            if (isIdUnqiue && isUserNameUnique)
            {
                userModel.UId = id;
                //Hash Password
                userModel.Password = HashingPassword.EncryptPlainTextToCipherText(userModel.Password);
                // Insert Record, return false if the insertion false
                isSucess = AddUserExecution(userModel);
                if(isSucess)
                {
                    return _locationRelationLogic.AddRelationLocationUser(parentId, id, locationId, type);
                } else
                {
                    return isSucess;
                }
                
            }
            else
            {
                return isSucess;
            }
        }

        // Return true indicate the id is unique
        public bool VerifyUIdUnique(string uid)
        {
            string id = null;
            string query = "SELECT uid FROM asset_user WHERE uid = " + DbHelper.SetDBValue(uid, true) + ";";
            this.Conn = new DBConnection();
            if (Conn.IsConnect())
            {
                Cmd = new MySqlCommand(query, this.Conn.Connection);
                Reader = Cmd.ExecuteReader();
                while (Reader.Read()) {
                    id = DbHelper.TryGet(Reader, "uid");
                }
                Conn.Close();
            } else
            {
                throw DbConnException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name));
            }
            if (id == null)
            {
                return true;
            } else
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
                throw DbConnException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name));
            }

            return CheckExistingHelper(id);
        }



        private bool AddUserExecution(UserModel userModel)
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
            } catch (Exception e)
            {
                throw DbInsertException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, e.ToString()));
            }
            return CheckInsertionHelper(res);
        }

        public UserModel GetUserByPassAndUserName(string userName, string pass)
        {
            this.Conn = new DBConnection();
            UserModel model = null;
            string query = "SELECT * FROM asset_user WHERE BINARY username = " + DbHelper.SetDBValue(userName, true) + ";";
            if(Conn.IsConnect())
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
                            if(decryptedPass == pass)
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
                } catch (Exception e)
                {
                    model = UserModelHelper.GetUserDataHaveError(false, true, "Internal Error - " + e.ToString());
                }
                
            } else
            {
                model = UserModelHelper.GetUserDataHaveError(false, true, "Connection Failure - DB");
            }
            return model;
        }

        public bool UpdateUser(UserModel userModel)
        {
            return UpdateUserExecution(userModel);
        }

        private bool UpdateUserExecution(UserModel userModel)
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

        public bool UpdatePassword(UserModel userModel)
        {
            return UpdatePasswordExecution(userModel);
        }

        private bool UpdatePasswordExecution(UserModel userModel)
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
    }
}