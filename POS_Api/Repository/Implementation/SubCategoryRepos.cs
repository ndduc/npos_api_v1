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
    public class SubCategoryRepos : BaseHelper, ISubCategoryRepos
    {

        public bool UpdateSubCategoryExecution(SubCategoryModel model)
        {
            int res = 0;
            Conn = new DBConnection();
            string query = " UPDATE asset_sub_category "
                        + " SET "
                        + " `description` = " + DbHelper.SetDBValue(model.Description, false)
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

        public List<SubCategoryModel> GetSubCategoryByLocationIdExecution(string locationId)
        {
            List<SubCategoryModel> lst = new List<SubCategoryModel>();
            Conn = new DBConnection();
            string query = " SELECT * FROM asset_sub_category "
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
                        SubCategoryModel model = new SubCategoryModel()
                        {
                            UId = DbHelper.TryGet(Reader, "uid"),
                            Description = DbHelper.TryGet(Reader, "description"),
                            LocationUId = DbHelper.TryGet(Reader, "location_uid"),
                            AddedDateTime = DbHelper.TryGet(Reader, "added_datetime"),
                            UpdatedDateTime = DbHelper.TryGet(Reader, "updated_datetime"),
                            AddedBy = DbHelper.TryGet(Reader, "added_by"),
                            UpdatedBy = DbHelper.TryGet(Reader, "updated_by"),
                            ApplyToUI = DbHelper.TryGetBoolean(Reader, "apply_to_ui"),
                            CategoryUId = DbHelper.TryGet(Reader, "category_uid"),
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
                SubCategoryModel model = new SubCategoryModel();
                model.IsError = true;
                model.Error = "No Category Found";
                lst.Add(model);
                return lst;
            }
        }


        public bool AddSubCategoryExecution(SubCategoryModel model)
        {
            int res = 0;
            Conn = new DBConnection();
            string query = "INSERT INTO asset_sub_category "
                            + " (`uid`,`description`, `location_uid`, `added_by`) "
                            + " VALUES ("
                            + DbHelper.SetDBValue(model.UId, false)
                            + DbHelper.SetDBValue(model.Description, false)
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


        public bool UpsertSubCategoryExecutionFromList(List<string> itemIdlist, string productId, string locationId, string userId)
        {
            List<bool> exectutedList = new List<bool>();
            foreach (string item in itemIdlist)
            {
                if (VerifySubCategoryProductRelationExist(productId, locationId))
                {
                    exectutedList.Add(UpdateSubCategoryProductRelationExecution(item, productId, locationId, userId));
                }
                else
                {
                    exectutedList.Add(AddSubCategoryProductRelationExecution(item, productId, locationId, userId));
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

        public bool AddSubCategoryExecutionFromList(List<string> itemIdlist, string productId, string locationId, string userId)
        {
            List<bool> exectutedList = new List<bool>();
            foreach (string item in itemIdlist)
            {
                exectutedList.Add(AddSubCategoryProductRelationExecution(item, productId, locationId, userId));
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

        public bool AddSubCategoryProductRelationExecution(string uid, string productId, string locationId, string userId)
        {
            int res = 0;
            Conn = new DBConnection();
            string query = " INSERT INTO ref_location_product_sub_category "
                            + " (`product_uid`, `location_uid`, `sub_category_uid`, `added_by`) "
                            + " VALUE ( "
                            + DbHelper.SetDBValue(productId, false)
                            + DbHelper.SetDBValue(locationId, false)
                            + DbHelper.SetDBValue(uid, false)
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

        public bool UpdateSubCategoryProductRelationExecution(string uid, string productId, string locationId, string userId)
        {
            int res = 0;
            Conn = new DBConnection();
            string query = " UPDATE  ref_location_product_sub_category "
                            + " SET "
                            + " `sub_category_uid` = " + DbHelper.SetDBValue(uid, false)
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

        public bool VerifySubCategoryProductRelationExist(string productId, string locationId)
        {
            this.Conn = new DBConnection();
            string id = null;
            string query = " SELECT id FROM ref_location_product_sub_category "
                            + " WHERE "
                            + " product_uid = " + DbHelper.SetDBValue(productId, true) + " AND "
                            + " location_uid = " + DbHelper.SetDBValue(locationId, true) + " ; ";
            try
            {
                if (Conn.IsConnect())
                {
                    Cmd = new MySqlCommand(query, this.Conn.Connection);
                    Reader = Cmd.ExecuteReader();
                    while (Reader.Read())
                    {
                        id = DbHelper.TryGet(Reader, "id");
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
            return CheckExistingHelper(id);
        }

        public bool VerifySubCategoryProductRelationExist(string uid, string productId, string locationId)
        {
            this.Conn = new DBConnection();
            string id = null;
            string query = " SELECT id FROM ref_location_product_sub_category "
                            + " WHERE "
                            + " product_uid = " + DbHelper.SetDBValue(productId, true) + " AND "
                            + " location_uid = " + DbHelper.SetDBValue(locationId, true) + " AND "
                            + " sub_category_uid = " + DbHelper.SetDBValue(uid, true) + " ; ";
            try
            {
                if (Conn.IsConnect())
                {
                    Cmd = new MySqlCommand(query, this.Conn.Connection);
                    Reader = Cmd.ExecuteReader();
                    while (Reader.Read())
                    {
                        id = DbHelper.TryGet(Reader, "id");
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
            return CheckExistingHelper(id);
        }

        public bool VerifyUIdUnique(string uid)
        {
            this.Conn = new DBConnection();
            string id = null;
            string query = "SELECT uid FROM asset_sub_category WHERE uid = " + DbHelper.SetDBValue(uid, true) + ";";

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
            string query = "SELECT uid FROM asset_sub_category WHERE uid = " + DbHelper.SetDBValue(uid, true) + ";";
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

        public int GetSubCategoryPaginateCount(string locId)
        {
            this.Conn = new DBConnection();
            string query = "SELECT count(*) as count FROM asset_sub_category AS AL"
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

        public IEnumerable<SubCategoryModel> GetSubCategoryPaginateByDefault(string locId, int startIdx, int endIdx)
        {
            List<SubCategoryModel> itemList = new List<SubCategoryModel>();
            this.Conn = new DBConnection();
            string query = "SELECT AL.* FROM asset_sub_category AS AL"
             + " WHERE location_uid =" + DbHelper.SetDBValue(locId, true)
             + " ORDER BY AL.updated_datetime DESC, AL.added_datetime DESC"
             + "; ";
            if (Conn.IsConnect())
            {
                Cmd = new MySqlCommand(query, this.Conn.Connection);
                Reader = Cmd.ExecuteReader();
                while (Reader.Read())
                {
                    SubCategoryModel model = new SubCategoryModel()
                    {
                        UId = DbHelper.TryGet(Reader, "uid"),
                        Description = DbHelper.TryGet(Reader, "description"),
                        SecondDescription = DbHelper.TryGet(Reader, "second_description"),
                        AddedDateTime = DbHelper.TryGet(Reader, "added_datetime"),
                        UpdatedDateTime = DbHelper.TryGet(Reader, "updated_datetime"),
                        AddedBy = DbHelper.TryGet(Reader, "added_by"),
                        UpdatedBy = DbHelper.TryGet(Reader, "updated_by"),
                        ApplyToUI = DbHelper.TryGetBoolean(Reader, "apply_to_ui"),
                        CategoryUId = DbHelper.TryGet(Reader, "category_uid"),
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

        public SubCategoryModel GetSubCategoryById(string locId, string CategoryId)
        {
            SubCategoryModel item = null;
            this.Conn = new DBConnection();
            string query = "SELECT AL.* FROM asset_sub)category AS AL"
             + " WHERE location_uid =" + DbHelper.SetDBValue(locId, true) + " AND "
             + " uid = " + DbHelper.SetDBValue(CategoryId, true)
             + " ORDER BY AL.updated_datetime DESC, AL.added_datetime DESC"
             + "; ";
            if (Conn.IsConnect())
            {
                Cmd = new MySqlCommand(query, this.Conn.Connection);
                Reader = Cmd.ExecuteReader();
                while (Reader.Read())
                {
                    SubCategoryModel model = new SubCategoryModel()
                    {
                        UId = DbHelper.TryGet(Reader, "uid"),
                        Description = DbHelper.TryGet(Reader, "description"),
                        SecondDescription = DbHelper.TryGet(Reader, "second_description"),
                        AddedDateTime = DbHelper.TryGet(Reader, "added_datetime"),
                        UpdatedDateTime = DbHelper.TryGet(Reader, "updated_datetime"),
                        AddedBy = DbHelper.TryGet(Reader, "added_by"),
                        UpdatedBy = DbHelper.TryGet(Reader, "updated_by"),
                        ApplyToUI = DbHelper.TryGetBoolean(Reader, "apply_to_ui"),
                        CategoryUId = DbHelper.TryGet(Reader, "category_uid"),
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

        public IEnumerable<SubCategoryModel> GetSubCategoryByDescription(string locId, string description)
        {
            List<SubCategoryModel> itemList = new List<SubCategoryModel>();
            this.Conn = new DBConnection();
            string query = "SELECT AL.* FROM asset_sub_category AS AL"
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
                    SubCategoryModel model = new SubCategoryModel()
                    {
                        UId = DbHelper.TryGet(Reader, "uid"),
                        Description = DbHelper.TryGet(Reader, "description"),
                        SecondDescription = DbHelper.TryGet(Reader, "second_description"),
                        AddedDateTime = DbHelper.TryGet(Reader, "added_datetime"),
                        UpdatedDateTime = DbHelper.TryGet(Reader, "updated_datetime"),
                        AddedBy = DbHelper.TryGet(Reader, "added_by"),
                        UpdatedBy = DbHelper.TryGet(Reader, "updated_by"),
                        ApplyToUI = DbHelper.TryGetBoolean(Reader, "apply_to_ui"),
                        CategoryUId = DbHelper.TryGet(Reader, "category_uid"),
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


        public IEnumerable<SubCategoryModel> GetSubCategoryByDepartmentId(string locId, string deptId)
        {
            List<SubCategoryModel> itemList = new List<SubCategoryModel>();
            this.Conn = new DBConnection();
            string query = "SELECT AL.* FROM asset_sub_category AS AL"
             + " WHERE location_uid =" + DbHelper.SetDBValue(locId, true) + " AND "
             + " department_uid =" + DbHelper.SetDBValue(deptId, true) + " "
             + " ORDER BY AL.updated_datetime DESC, AL.added_datetime DESC"
             + "; ";
            if (Conn.IsConnect())
            {
                Cmd = new MySqlCommand(query, this.Conn.Connection);
                Reader = Cmd.ExecuteReader();
                while (Reader.Read())
                {
                    SubCategoryModel model = new SubCategoryModel()
                    {
                        UId = DbHelper.TryGet(Reader, "uid"),
                        Description = DbHelper.TryGet(Reader, "description"),
                        SecondDescription = DbHelper.TryGet(Reader, "second_description"),
                        AddedDateTime = DbHelper.TryGet(Reader, "added_datetime"),
                        UpdatedDateTime = DbHelper.TryGet(Reader, "updated_datetime"),
                        AddedBy = DbHelper.TryGet(Reader, "added_by"),
                        UpdatedBy = DbHelper.TryGet(Reader, "updated_by"),
                        ApplyToUI = DbHelper.TryGetBoolean(Reader, "apply_to_ui"),
                        CategoryUId = DbHelper.TryGet(Reader, "category_uid"),
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
