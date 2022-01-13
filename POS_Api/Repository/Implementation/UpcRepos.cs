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
    public class UpcRepos : BaseHelper, IUpcRepos
    {
        public bool RemoveUpcExecution(string productUid, string locationUid, string upc)
        {
            int res = 0;
            Conn = new DBConnection();
            string query = " DELETE FROM ref_location_product_upc WHERE"
                    + " `product_uid` = " + DbHelper.SetDBValue(productUid, true) 
                    + " AND `location_uid` = " + DbHelper.SetDBValue(locationUid, true) 
                    + " AND `upc` = " + DbHelper.SetDBValue(upc, true) + "; ";
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

        public bool AddUpcExecution(string productUid, string locationUid, string upc, string userId)
        {
            int res = 0;
            Conn = new DBConnection();
            string query = "INSERT INTO ref_location_product_upc "
                    + " (`product_uid`, `location_uid`, `upc`, `added_by`) "
                    + " VALUE ("
                    + DbHelper.SetDBValue(productUid, false)
                    + DbHelper.SetDBValue(locationUid, false)
                    + DbHelper.SetDBValue(upc, false)
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

        public bool VerifyUpcExist(string productUid, string locationUid, string upc)
        {
            Conn = new DBConnection();
            string id = null;
            string query = "SELECT id FROM ref_location_product_upc WHERE "
                    + " `product_uid` = " + DbHelper.SetDBValue(productUid, true)
                    + " AND `location_uid` = " + DbHelper.SetDBValue(locationUid, true)
                    + " AND `upc` = "  + DbHelper.SetDBValue(upc, true)
                    + "; ";
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


        public UpcModel GetUpcById (string productUid, string locationUid, string upc)
        {
            UpcModel item = null;
            this.Conn = new DBConnection();
            string query = "SELECT * FROM ref_location_product_upc WHERE "
                    + " `product_uid` = " + DbHelper.SetDBValue(productUid, true)
                    + " AND `location_uid` = " + DbHelper.SetDBValue(locationUid, true)
                    + " AND `upc` = " + DbHelper.SetDBValue(upc, true)
                    + "; ";
            if (Conn.IsConnect())
            {
                Cmd = new MySqlCommand(query, this.Conn.Connection);
                Reader = Cmd.ExecuteReader();
                while (Reader.Read())
                {
                    UpcModel model = new UpcModel()
                    {
                        ProductUid = DbHelper.TryGet(Reader, "product_uid"),
                        LocationUid = DbHelper.TryGet(Reader, "location_uid"),
                        Upc = DbHelper.TryGet(Reader, "upc"),
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

        public IEnumerable<UpcModel> GetAllUpcByLocationAndProduct(string productUid, string locationUid)
        {
            List<UpcModel> itemList = new List<UpcModel>();
            this.Conn = new DBConnection();
            string query = "SELECT * FROM ref_location_product_upc WHERE "
                    + " `product_uid` = " + DbHelper.SetDBValue(productUid, true)
                    + " AND `location_uid` = " + DbHelper.SetDBValue(locationUid, true)
                    + "; ";
            if (Conn.IsConnect())
            {
                Cmd = new MySqlCommand(query, this.Conn.Connection);
                Reader = Cmd.ExecuteReader();
                while (Reader.Read())
                {
                    UpcModel model = new UpcModel()
                    {
                        ProductUid = DbHelper.TryGet(Reader, "product_uid"),
                        LocationUid = DbHelper.TryGet(Reader, "location_uid"),
                        Upc = DbHelper.TryGet(Reader, "upc"),
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
