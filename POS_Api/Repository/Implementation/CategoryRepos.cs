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
    public class CategoryRepos : BaseHelper, ICategoryRepos
    {

        public bool UpdateCategoryExecution(CategoryModel model)
        {
            int res = 0;
            Conn = new DBConnection();
            string query = " UPDATE asset_category "
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

        public List<CategoryModel> GetCategoryByLocationIdExecution(string locationId)
        {
            List<CategoryModel> lst = new List<CategoryModel>();
            Conn = new DBConnection();
            string query = " SELECT * FROM asset_category "
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
                        CategoryModel model = new CategoryModel()
                        {
                            UId = DbHelper.TryGet(Reader, "uid"),
                            Description = DbHelper.TryGet(Reader, "description"),
                            LocationUId = DbHelper.TryGet(Reader, "location_uid"),
                            AddedDateTime = DbHelper.TryGet(Reader, "added_datetime"),
                            UpdatedDateTime = DbHelper.TryGet(Reader, "updated_datetime"),
                            AddedBy = DbHelper.TryGet(Reader, "added_by"),
                            UpdatedBy = DbHelper.TryGet(Reader, "updated_by"),
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
                CategoryModel model = new CategoryModel();
                model.IsError = true;
                model.Error = "No Category Found";
                lst.Add(model);
                return lst;
            }
        }


        public bool AddCategoryExecution(CategoryModel model)
        {
            int res = 0;
            Conn = new DBConnection();
            string query = "INSERT INTO asset_category "
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


        public bool UpsertCategoryExecutionFromList(List<string> itemIdlist, string productId, string locationId, string userId)
        {
            List<bool> exectutedList = new List<bool>();
            foreach (string item in itemIdlist)
            {
                if (VerifyCategoryProductRelationExist(productId, locationId))
                {
                    exectutedList.Add(UpdateCategoryProductRelationExecution(item, productId, locationId, userId));
                }
                else
                {
                    exectutedList.Add(AddCategoryProductRelationExecution(item, productId, locationId, userId));
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

        public bool AddCategoryExecutionFromList(List<string> itemIdlist, string productId, string locationId, string userId)
        {
            List<bool> exectutedList = new List<bool>();
            foreach (string item in itemIdlist)
            {
                exectutedList.Add(AddCategoryProductRelationExecution(item, productId, locationId, userId));
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

        public bool AddCategoryProductRelationExecution(string uid, string productId, string locationId, string userId)
        {
            int res = 0;
            Conn = new DBConnection();
            string query = " INSERT INTO ref_location_product_category "
                            + " (`product_uid`, `location_uid`, `category_uid`, `added_by`) "
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

        public bool UpdateCategoryProductRelationExecution(string uid, string productId, string locationId, string userId)
        {
            int res = 0;
            Conn = new DBConnection();
            string query = " UPDATE  ref_location_product_category "
                            + " SET "
                            + " `category_uid` = " + DbHelper.SetDBValue(uid, false)
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

        public bool VerifyCategoryProductRelationExist(string productId, string locationId)
        {
            this.Conn = new DBConnection();
            string id = null;
            string query = " SELECT id FROM ref_location_product_category "
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

        public bool VerifyCategoryProductRelationExist(string uid, string productId, string locationId)
        {
            this.Conn = new DBConnection();
            string id = null;
            string query = " SELECT id FROM ref_location_product_category "
                            + " WHERE "
                            + " product_uid = " + DbHelper.SetDBValue(productId, true) + " AND "
                            + " location_uid = " + DbHelper.SetDBValue(locationId, true) + " AND "
                            + " category_uid = " + DbHelper.SetDBValue(uid, true) + " ; ";
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
            string query = "SELECT uid FROM asset_category WHERE uid = " + DbHelper.SetDBValue(uid, true) + ";";

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
            string query = "SELECT uid FROM asset_category WHERE uid = " + DbHelper.SetDBValue(uid, true) + ";";
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

        public int GetCategoryPaginateCount(string locId)
        {
            this.Conn = new DBConnection();
            string query = "SELECT count(*) as count FROM asset_category AS AL"
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

        public IEnumerable<CategoryModel> GetCategoryPaginateByDefault(string locId, int startIdx, int endIdx)
        {
            List<CategoryModel> itemList = new List<CategoryModel>();
            this.Conn = new DBConnection();
            string query = "SELECT AL.* FROM asset_category AS AL"
             + " WHERE location_uid =" + DbHelper.SetDBValue(locId, true)
             + " ORDER BY AL.updated_datetime DESC, AL.added_datetime DESC"
             + "; ";
            if (Conn.IsConnect())
            {
                Cmd = new MySqlCommand(query, this.Conn.Connection);
                Reader = Cmd.ExecuteReader();
                while (Reader.Read())
                {
                    CategoryModel model = new CategoryModel()
                    {
                        UId = DbHelper.TryGet(Reader, "uid"),
                        Description = DbHelper.TryGet(Reader, "description"),
                        SecondDescription = DbHelper.TryGet(Reader, "second_description"),
                        AddedDateTime = DbHelper.TryGet(Reader, "added_datetime"),
                        UpdatedDateTime = DbHelper.TryGet(Reader, "updated_datetime"),
                        AddedBy = DbHelper.TryGet(Reader, "added_by"),
                        UpdatedBy = DbHelper.TryGet(Reader, "updated_by")
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

        public CategoryModel GetCategoryById(string locId, string CategoryId)
        {
            CategoryModel item = null;
            this.Conn = new DBConnection();
            string query = "SELECT AL.* FROM asset_category AS AL"
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
                    CategoryModel model = new CategoryModel()
                    {
                        UId = DbHelper.TryGet(Reader, "uid"),
                        Description = DbHelper.TryGet(Reader, "description"),
                        SecondDescription = DbHelper.TryGet(Reader, "second_description"),
                        AddedDateTime = DbHelper.TryGet(Reader, "added_datetime"),
                        UpdatedDateTime = DbHelper.TryGet(Reader, "updated_datetime"),
                        AddedBy = DbHelper.TryGet(Reader, "added_by"),
                        UpdatedBy = DbHelper.TryGet(Reader, "updated_by")
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

        public IEnumerable<CategoryModel> GetCategoryByDescription(string locId, string description)
        {
            List<CategoryModel> itemList = new List<CategoryModel>();
            this.Conn = new DBConnection();
            string query = "SELECT AL.* FROM asset_category AS AL"
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
                    CategoryModel model = new CategoryModel()
                    {
                        UId = DbHelper.TryGet(Reader, "uid"),
                        Description = DbHelper.TryGet(Reader, "description"),
                        SecondDescription = DbHelper.TryGet(Reader, "second_description"),
                        AddedDateTime = DbHelper.TryGet(Reader, "added_datetime"),
                        UpdatedDateTime = DbHelper.TryGet(Reader, "updated_datetime"),
                        AddedBy = DbHelper.TryGet(Reader, "added_by"),
                        UpdatedBy = DbHelper.TryGet(Reader, "updated_by")
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
