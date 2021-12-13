﻿using MySql.Data.MySqlClient;
using POS_Api.Core.Interface;
using POS_Api.Database.MySql.Configuration;
using POS_Api.Model;
using POS_Api.Repository.Interface;
using POS_Api.Shared.DbHelper;
using POS_Api.Shared.ExceptionHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;


namespace POS_Api.Repository.Implementation
{
    public class LocationRepos : BaseHelper, ILocationRepos
    {
        // Return true indicate the id is unique
        public bool VerifyUIdUnique(string uid)
        {
            Conn = new DBConnection();
            string id = null;
            string query = "SELECT uid FROM asset_location WHERE uid = " + DbHelper.SetDBValue(uid, true) + ";";
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
            return VerifyNotExist(id);
        }


        public bool VerifyUIdExist(string uid)
        {
            Conn = new DBConnection();
            string id = null;
            string query = "SELECT uid FROM asset_location WHERE uid = " + DbHelper.SetDBValue(uid, true) + ";";
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
            return CheckExistingHelper(id);
        }

        public bool AddLocationExecution(LocationModel model)
        {
            int res = 0;
            Conn = new DBConnection();

            string query = "INSERT INTO asset_location"
                + " (`uid`,`name`,`address`, `zipCode`, `state`, `phoneNumber`)"
                + " VALUES ( "
                + DbHelper.SetDBValue(model.UId, false)
                + DbHelper.SetDBValue(model.Name, false)
                + DbHelper.SetDBValue(model.Address, false)
                + DbHelper.SetDBValueNull(model.ZipCode.ToString(), false)
                + DbHelper.SetDBValueNull(model.State, false)
                + DbHelper.SetDBValueNull(model.PhoneNumber, true)
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

        public IEnumerable<LocationModel> GetLocationByUserIdExecution(string userId)
        {
            List<LocationModel> productList = new List<LocationModel>();
            this.Conn = new DBConnection();
            string query = "SELECT L.*, RLU.relation_reason FROM asset_location AS L "
                        + " INNER JOIN ref_location_user AS RLU"
                        + " ON L.uid = RLU.location_uid and RLU.user_uid = "
                        + DbHelper.SetDBValue(userId, true)
                        + " LIMIT 1";
            if (Conn.IsConnect())
            {
                Cmd = new MySqlCommand(query, this.Conn.Connection);
                Reader = Cmd.ExecuteReader();
                while (Reader.Read())
                {
                    LocationModel model = new LocationModel()
                    {
                        UId = DbHelper.TryGet(Reader, "uid"),
                        Name = DbHelper.TryGet(Reader, "name"),
                        Address = DbHelper.TryGet(Reader, "address"),
                        ZipCode = int.Parse(DbHelper.TryGet(Reader, "zipcode")),
                        State = DbHelper.TryGet(Reader, "state"),
                        PhoneNumber = DbHelper.TryGet(Reader, "phonenumber"),
                        AddedDateTime = DbHelper.TryGet(Reader, "added_datetime"),
                        UpdatedDateTime = DbHelper.TryGet(Reader, "updated_datetime"),
                        RelationReason = DbHelper.TryGet(Reader, "relation_reason")
                    };
                    productList.Add(model);

                }
                this.Conn.Close();
            }
            else
            {
                throw DbConnException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name));
            }

            if (productList.Count > 0)
            {
                return productList;
            }
            else
            {
                LocationModel model = new LocationModel();
                model.IsError = true;
                model.Error = "No Location Found";
                productList.Add(model);
                return productList;
            }
        }

    }
}
