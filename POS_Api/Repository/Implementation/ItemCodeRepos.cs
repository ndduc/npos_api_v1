using MySql.Data.MySqlClient;
using POS_Api.Database.MySql.Configuration;
using POS_Api.Model;
using POS_Api.Model.ReponseViewModel;
using POS_Api.Repository.Interface;
using POS_Api.Shared.DbHelper;
using POS_Api.Shared.ExceptionHelper;
using System;
using System.Collections.Generic;
using System.Reflection;


namespace POS_Api.Repository.Implementation
{
    public class ItemCodeRepos : BaseHelper, IItemCodeRepos
    {
        public bool RemoveItemCodeExecution(string productUid, string locationUid, string itemCode)
        {
            int res = 0;
            Conn = new DBConnection();
            string query = " DELETE FROM ref_location_product_itemcode WHERE"
                    + " `product_uid` = " + DbHelper.SetDBValue(productUid, true) 
                    + " AND `location_uid` = " + DbHelper.SetDBValue(locationUid, true) 
                    + " AND `item_code` = " + DbHelper.SetDBValue(itemCode, true) + "; ";
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

        public bool AddItemCodeExecution(string productUid, string locationUid, string itemCode, string userId)
        {
            int res = 0;
            Conn = new DBConnection();
            string query = "INSERT INTO ref_location_product_itemcode "
                    + " (`product_uid`, `location_uid`, `item_code`, `added_by`) "
                    + " VALUE ("
                    + DbHelper.SetDBValue(productUid, false)
                    + DbHelper.SetDBValue(locationUid, false)
                    + DbHelper.SetDBValue(itemCode, false)
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

        public bool VerifyItemCodeExist(string productUid, string locationUid, string itemCode)
        {
            Conn = new DBConnection();
            string id = null;
            string query = "SELECT id FROM ref_location_product_itemcode WHERE "
                    + " `location_uid` = " + DbHelper.SetDBValue(locationUid, true)
                    + " AND `item_code` = "  + DbHelper.SetDBValue(itemCode, true)
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


        public ItemCodeModel GetItemCodeById (string productUid, string locationUid, string itemCode)
        {
            ItemCodeModel item = null;
            this.Conn = new DBConnection();
            string query = "SELECT * FROM ref_location_product_itemcode WHERE "
                    + " `product_uid` = " + DbHelper.SetDBValue(productUid, true)
                    + " AND `location_uid` = " + DbHelper.SetDBValue(locationUid, true)
                    + " AND `item_code` = " + DbHelper.SetDBValue(itemCode, true)
                    + "; ";
            if (Conn.IsConnect())
            {
                Cmd = new MySqlCommand(query, this.Conn.Connection);
                Reader = Cmd.ExecuteReader();
                while (Reader.Read())
                {
                    ItemCodeModel model = new ItemCodeModel()
                    {
                        ProductUid = DbHelper.TryGet(Reader, "product_uid"),
                        LocationUid = DbHelper.TryGet(Reader, "location_uid"),
                        ItemCode = DbHelper.TryGet(Reader, "item_code"),
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

        public IEnumerable<ItemCodeModel> GetAllItemCodeByLocationAndProduct(string productUid, string locationUid)
        {
            List<ItemCodeModel> itemList = new List<ItemCodeModel>();
            this.Conn = new DBConnection();
            string query = "SELECT * FROM ref_location_product_itemcode WHERE "
                    + " `product_uid` = " + DbHelper.SetDBValue(productUid, true)
                    + " AND `location_uid` = " + DbHelper.SetDBValue(locationUid, true)
                    + "; ";
            if (Conn.IsConnect())
            {
                Cmd = new MySqlCommand(query, this.Conn.Connection);
                Reader = Cmd.ExecuteReader();
                while (Reader.Read())
                {
                    ItemCodeModel model = new ItemCodeModel()
                    {
                        ProductUid = DbHelper.TryGet(Reader, "product_uid"),
                        LocationUid = DbHelper.TryGet(Reader, "location_uid"),
                        ItemCode = DbHelper.TryGet(Reader, "item_code"),
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


        public ItemCodePaginationModelVm GetItemCodePagination(string productUid, string locationUid, int limit, int offset, string order)
        {
            int count = GetItemCodePaginationCountExecution(productUid, locationUid);
            List<ItemCodeModel> itemCodes = GetItemCodePaginationExecution(productUid, locationUid, limit, offset);
            PaginationModel pag = new PaginationModel()
            {
                Limit = limit,
                OffSet = offset,
                Order = order,
                Count = count
            };
            ItemCodePaginationModelVm model = new ItemCodePaginationModelVm() {
                ItemCodeList = itemCodes,
                PaginationObject = pag
            };
            return model;
        }

        private int GetItemCodePaginationCountExecution(string productUid, string locationUid)
        {
            int count = 0;
            this.Conn = new DBConnection();
            string query = "SELECT count(*) as count "
                            + " FROM ref_location_product_itemcode as ic "
                            + " WHERE "
                            + " ic.product_uid = " + DbHelper.SetDBValue(productUid, true)
                            + " AND ic.location_uid = " + DbHelper.SetDBValue(locationUid, true)
                            + " ; ";
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

        private List<ItemCodeModel> GetItemCodePaginationExecution(string productUid, string locationUid, int limit, int offset)
        {
            List<ItemCodeModel> itemList = new List<ItemCodeModel>();
            this.Conn = new DBConnection();
            string query = "SELECT ic.id, ic.product_uid, ic.item_code, ic.added_datetime, ic.updated_datetime, "
                        + " (select concat(firstname, ', ', lastname) from asset_user where uid = ic.added_by) as add_by, "
                        + " (select concat(firstname, ', ', lastname) from asset_user where uid = ic.updated_by) as updated_by "
                        + " FROM ref_location_product_itemcode as ic "
                        + " WHERE "
                        + " ic.product_uid = " + DbHelper.SetDBValue(productUid, true)
                        + " AND ic.location_uid = " + DbHelper.SetDBValue(locationUid, true)
                        + " LIMIT "  + limit
                        + " OFFSET " + offset
                        + " ; ";
            if (Conn.IsConnect())
            {
                Cmd = new MySqlCommand(query, this.Conn.Connection);
                Reader = Cmd.ExecuteReader();
                while (Reader.Read())
                {
                    ItemCodeModel model = new ItemCodeModel()
                    {
                        Id = DbHelper.TryGet(Reader, "id"),
                        ProductUid = DbHelper.TryGet(Reader, "product_uid"),
                        LocationUid = DbHelper.TryGet(Reader, "location_uid"),
                        ItemCode = DbHelper.TryGet(Reader, "item_code"),
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
                    return itemList;
                }
            }
            else
            {
                throw DbConnException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name));
            }
        }
    }
}
