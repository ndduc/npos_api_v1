using MySql.Data.MySqlClient;
using POS_Api.Database.MySql.Configuration;
using POS_Api.Model;
using POS_Api.Repository.Interface;
using POS_Api.Shared.DbHelper;
using POS_Api.Shared.ExceptionHelper;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace POS_Api.Repository.Implementation
{
    public class DiscountRepos : BaseHelper, IDiscountRepos
    {
        public bool UpdateDiscountExecution(DiscountModel model)
        {
            int res = 0;
            Conn = new DBConnection();
            string query = " UPDATE asset_discount "
                        + " SET "
                        + " `description` = " + DbHelper.SetDBValue(model.Description, false)
                        + " `rate` = " + DbHelper.SetDBValue(model.Rate, false)
                        + " `updated_by` = " + DbHelper.SetDBValue(model.UpdatedBy, true)
                        + " WHERE "
                        + " uid = " + DbHelper.SetDBValue(model.UId, true) + " AND "
                        + " location_uid = " + DbHelper.SetDBValue(model.LocationUId, true) + "";
            try
            {
                if (Conn.IsConnect())
                {
                    Cmd = new MySqlCommand(query, this.Conn.Connection);
                    res = Cmd.ExecuteNonQuery();
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

            return CheckUpdateHelper(res);
        }

        public List<DiscountModel> GetDiscountByLocationIdExecution(string locationId)
        {
            List<DiscountModel> lst = new List<DiscountModel>();
            Conn = new DBConnection();
            string query = " SELECT * FROM asset_discount "
                                + " WHERE `location_uid` = "
                                + DbHelper.SetDBValue(locationId, true)
                                + " ORDER BY `description` ASC; ";
            try
            {
                if (Conn.IsConnect())
                {
                    Cmd = new MySqlCommand(query, this.Conn.Connection);
                    Reader = Cmd.ExecuteReader();
                    while (Reader.Read())
                    {
                        DiscountModel model = new DiscountModel()
                        {
                            UId = DbHelper.TryGet(Reader, "uid"),
                            Description = DbHelper.TryGet(Reader, "description"),
                            LocationUId = DbHelper.TryGet(Reader, "location_uid"),
                            Rate = double.Parse(DbHelper.TryGet(Reader, "rate")),
                            AddedDateTime = DbHelper.TryGet(Reader, "added_datetime"),
                            UpdatedDateTime = DbHelper.TryGet(Reader, "updated_datetime"),
                            AddedBy = DbHelper.TryGet(Reader, "added_by"),
                            UpdatedBy = DbHelper.TryGet(Reader, "updated_by"),
                            ApplyToUI = DbHelper.TryGetBoolean(Reader, "apply_to_ui"),
                        };

                        lst.Add(model);
                    }
                    this.Conn.Close();
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

            if (lst.Count > 0)
            {
                return lst;
            }
            else
            {
                DiscountModel model = new DiscountModel();
                model.IsError = true;
                model.Error = "No Discount Found";
                lst.Add(model);
                return lst;
            }
        }


        public bool AddDiscountExecution(DiscountModel model)
        {
            int res = 0;
            Conn = new DBConnection();
            string query = "INSERT INTO asset_discount "
                            + " (`uid`,`description`, `rate`, `location_uid`,`added_by`) "
                            + " VALUES ("
                            + DbHelper.SetDBValue(model.UId, false)
                            + DbHelper.SetDBValue(model.Description, false)
                            + DbHelper.SetDBValue(model.Rate, false)
                            + DbHelper.SetDBValue(model.LocationUId, false)
                            + DbHelper.SetDBValue(model.AddedBy, true)
                            + " ); ";
            try
            {
                if (Conn.IsConnect())
                {
                    Cmd = new MySqlCommand(query, this.Conn.Connection);
                    res = Cmd.ExecuteNonQuery();
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

            return CheckInsertionHelper(res);
        }

        public bool VerifyUIdUnique(string uid)
        {
            this.Conn = new DBConnection();
            string id = null;
            string query = "SELECT uid FROM asset_discount WHERE uid = " + DbHelper.SetDBValue(uid, true) + ";";

            try
            {
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

            }
            catch (Exception e)
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, e.ToString()));
            }
            return VerifyNotExist(id);
        }

        public bool VerifyUIdExist(string uid)
        {
            Conn = new DBConnection();
            string id = null;
            string query = "SELECT uid FROM asset_discount WHERE uid = " + DbHelper.SetDBValue(uid, true) + ";";
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

        public bool VerifyDiscountProductRelation(string productId, string locationId)
        {
            Conn = new DBConnection();
            string id = null;
            string query = " SELECT id FROM ref_product_discount WHERE "
                        + " `product_uid` = " + DbHelper.SetDBValue(productId, true) + " AND "
                        + " `location_uid` = " + DbHelper.SetDBValue(locationId, true) + "; ";
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
            return CheckExistingHelper(id);
        }

        public bool VerifyDiscountProductRelation(string discountId, string productId, string locationId)
        {
            Conn = new DBConnection();
            string id = null;
            string query = " SELECT id FROM ref_product_discount WHERE "
                        + " `product_uid` = " + DbHelper.SetDBValue(productId, true) + " AND "
                        + " `discount_uid` = " + DbHelper.SetDBValue(discountId, true) + " AND "
                        + " `location_uid` = " + DbHelper.SetDBValue(locationId, true) + "; ";
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
            return CheckExistingHelper(id);
        }

        public bool AddDiscountProductRelationExecution(string productId, string locationId, string discountId, string userId)
        {
            int res = 0;
            Conn = new DBConnection();
            string query = " INSERT INTO ref_product_discount "
                            + " (`product_uid`, `location_uid`, `discount_uid`, `added_by`) "
                            + " VALUE( "
                            + DbHelper.SetDBValue(productId, false)
                            + DbHelper.SetDBValue(locationId, false)
                            + DbHelper.SetDBValue(discountId, false)
                            + DbHelper.SetDBValue(userId, true)
                            + " ); ";
            try
            {
                if (Conn.IsConnect())
                {
                    Cmd = new MySqlCommand(query, this.Conn.Connection);
                    res = Cmd.ExecuteNonQuery();
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

            return CheckInsertionHelper(res);
        }

        public bool UpdateDiscountProductRelationExecution(string productId, string locationId, string discountId, string userId)
        {
            int res = 0;
            Conn = new DBConnection();
            string query = " UPDATE  ref_product_discount"
                            + " SET "
                            + " `discount_uid` = " + DbHelper.SetDBValue(discountId, false)
                            + " `updated_by` = " + DbHelper.SetDBValue(userId, true)
                            + " WHERE "
                            + " `product_uid` = " + DbHelper.SetDBValue(productId, true)
                            + " AND "
                            + " `location_uid` = " + DbHelper.SetDBValue(locationId, true) + ";";
            try
            {
                if (Conn.IsConnect())
                {
                    Cmd = new MySqlCommand(query, this.Conn.Connection);
                    res = Cmd.ExecuteNonQuery();
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

            return CheckInsertionHelper(res);
        }

        public bool AddDiscountExecutionFromList(List<string> itemIdlist, string productId, string locationId, string userId)
        {
            List<bool> exectutedList = new List<bool>();
            foreach (string item in itemIdlist)
            {
                exectutedList.Add(AddDiscountProductRelationExecution(productId, locationId, item, userId));
            }

            if (exectutedList.Contains(false))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool UpsertDiscountExecutionFromList(List<string> itemIdlist, string productId, string locationId, string userId)
        {
            List<bool> exectutedList = new List<bool>();
            foreach (string item in itemIdlist)
            {
                if(VerifyDiscountProductRelation(item, productId, locationId))
                {
                    exectutedList.Add(UpdateDiscountProductRelationExecution(productId, locationId, item, userId));
                }
                else
                {
                    exectutedList.Add(AddDiscountProductRelationExecution(productId, locationId, item, userId));
                }
            }

            if (exectutedList.Contains(false))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public int GetDiscountPaginateCount(string locId)
        {
            this.Conn = new DBConnection();
            string query = "SELECT count(*) as count FROM asset_discount AS AL"
              + " WHERE location_uid =" + DbHelper.SetDBValue(locId, true)
              + " ;";
            int count = 0;
            if (Conn.IsConnect())
            {
                Cmd = new MySqlCommand(query, this.Conn.Connection);
                Reader = Cmd.ExecuteReader();
                while (Reader.Read())
                {
                    count = int.Parse(DbHelper.TryGet(Reader, "count"));
                }
                this.Conn.Close();
            }
            else
            {
                throw DbConnException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name));
            }

            return count;
        }

        public IEnumerable<DiscountModel> GetDiscountPaginateByDefault(string locId, int startIdx, int endIdx)
        {
            List<DiscountModel> itemList = new List<DiscountModel>();
            this.Conn = new DBConnection();
            string query = "SELECT AL.* FROM asset_discount AS AL"
             + " WHERE location_uid =" + DbHelper.SetDBValue(locId, true)
             + " ORDER BY AL.updated_datetime DESC, AL.added_datetime DESC"
             + "; ";
            if (Conn.IsConnect())
            {
                Cmd = new MySqlCommand(query, this.Conn.Connection);
                Reader = Cmd.ExecuteReader();
                while (Reader.Read())
                {
                    DiscountModel model = new DiscountModel()
                    {
                        UId = DbHelper.TryGet(Reader, "uid"),
                        Description = DbHelper.TryGet(Reader, "description"),
                        SecondDescription = DbHelper.TryGet(Reader, "second_description"),
                        AddedDateTime = DbHelper.TryGet(Reader, "added_datetime"),
                        UpdatedDateTime = DbHelper.TryGet(Reader, "updated_datetime"),
                        Rate = double.Parse(DbHelper.TryGet(Reader, "rate")),
                        AddedBy = DbHelper.TryGet(Reader, "added_by"),
                        UpdatedBy = DbHelper.TryGet(Reader, "updated_by"),
                        ApplyToUI = DbHelper.TryGetBoolean(Reader, "apply_to_ui"),
                    };
                    itemList.Add(model);
                }
                this.Conn.Close();
                if (itemList.Count > 0)
                {
                    return itemList;
                }
                else
                {
                    throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "No Record Found"));
                }
            }
            else
            {
                throw DbConnException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name));
            }
        }

        public DiscountModel GetDiscountById(string locId, string DiscountId)
        {
            DiscountModel item = null;
            this.Conn = new DBConnection();
            string query = "SELECT AL.* FROM asset_discount AS AL"
             + " WHERE location_uid =" + DbHelper.SetDBValue(locId, true) + " AND "
             + " uid = " + DbHelper.SetDBValue(DiscountId, true)
             + " ORDER BY AL.updated_datetime DESC, AL.added_datetime DESC"
             + "; ";
            if (Conn.IsConnect())
            {
                Cmd = new MySqlCommand(query, this.Conn.Connection);
                Reader = Cmd.ExecuteReader();
                while (Reader.Read())
                {
                    DiscountModel model = new DiscountModel()
                    {
                        UId = DbHelper.TryGet(Reader, "uid"),
                        Description = DbHelper.TryGet(Reader, "description"),
                        SecondDescription = DbHelper.TryGet(Reader, "second_description"),
                        Rate = double.Parse(DbHelper.TryGet(Reader, "rate")),
                        AddedDateTime = DbHelper.TryGet(Reader, "added_datetime"),
                        UpdatedDateTime = DbHelper.TryGet(Reader, "updated_datetime"),
                        AddedBy = DbHelper.TryGet(Reader, "added_by"),
                        UpdatedBy = DbHelper.TryGet(Reader, "updated_by"),
                        ApplyToUI = DbHelper.TryGetBoolean(Reader, "apply_to_ui"),
                    };
                    item = model;
                }
                this.Conn.Close();
                if (CheckExistingHelper(item))
                {
                    return item;
                }
                else
                {
                    throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "No Record Found"));
                }
            }
            else
            {
                throw DbConnException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name));
            }
        }


        public IEnumerable<DiscountModel> GetDiscountByDescription(string locId, string description)
        {
            List<DiscountModel> itemList = new List<DiscountModel>();
            this.Conn = new DBConnection();
            string query = "SELECT AL.* FROM asset_discount AS AL"
             + " WHERE location_uid =" + DbHelper.SetDBValue(locId, true) + " AND "
             + " description like '%" + description + "%' "
             + " ORDER BY AL.updated_datetime DESC, AL.added_datetime DESC"
             + "; ";
            if (Conn.IsConnect())
            {
                Cmd = new MySqlCommand(query, this.Conn.Connection);
                Reader = Cmd.ExecuteReader();
                while (Reader.Read())
                {
                    DiscountModel model = new DiscountModel()
                    {
                        UId = DbHelper.TryGet(Reader, "uid"),
                        Description = DbHelper.TryGet(Reader, "description"),
                        SecondDescription = DbHelper.TryGet(Reader, "second_description"),
                        Rate = double.Parse(DbHelper.TryGet(Reader, "rate")),
                        AddedDateTime = DbHelper.TryGet(Reader, "added_datetime"),
                        UpdatedDateTime = DbHelper.TryGet(Reader, "updated_datetime"),
                        AddedBy = DbHelper.TryGet(Reader, "added_by"),
                        UpdatedBy = DbHelper.TryGet(Reader, "updated_by"),
                        ApplyToUI = DbHelper.TryGetBoolean(Reader, "apply_to_ui"),
                    };
                    itemList.Add(model);
                }
                this.Conn.Close();
                if (itemList.Count > 0)
                {
                    return itemList;
                }
                else
                {
                    throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "No Record Found"));
                }
            }
            else
            {
                throw DbConnException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name));
            }
        }

    }
}
