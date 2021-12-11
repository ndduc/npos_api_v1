using MySql.Data.MySqlClient;
using POS_Api.Core.Interface;
using POS_Api.Database.MySql.Configuration;
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
    public class LocationProductRelationLogic : BaseHelper, ILocationProductRelationLogic
    {

        public bool IsRelationLocationProductExist(string locationId, string productId)
        {
            Conn = new DBConnection();
            string id = null;
            string query = "SELECT id FROM ref_location_product"
                            + " WHERE PRODUCT_UID = " + DbHelper.SetDBValue(productId, true)  + " AND"
                            + " LOCATION_UID = " + DbHelper.SetDBValue(locationId, true) + "; ";
            try
            {
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

        public bool AddRelationLocationProduct(string locationId, string productId, string userId)
        {
            int res = 0;
            Conn = new DBConnection();
            string query = "INSERT INTO REF_LOCATION_PRODUCT (`product_uid`, `location_uid`, `added_by`) VALUES ( "
                + DbHelper.SetDBValue(productId, false)
                + DbHelper.SetDBValue(locationId, false)
                + DbHelper.SetDBValue(userId, true)
                + " );";
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

        public bool IsRelationItemCodeExist(string locationId, string productId, string itemCode)
        {
            Conn = new DBConnection();
            string id = null;
            string query = "SELECT id FROM ref_location_product_itemcode"
                            + " WHERE PRODUCT_UID = " + DbHelper.SetDBValue(productId, true) + " AND"
                            + " LOCATION_UID = " + DbHelper.SetDBValue(locationId, true) + " AND "
                            + " ITEM_CODE = " + DbHelper.SetDBValue(itemCode, true) + " ; ";
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

        

        public bool AddRelationItemCode(string locationId, string productId, string userId, string itemCode)
        {
            bool isRelationExistItemCode = IsRelationItemCodeExist(locationId, productId, itemCode);
            bool isRelationLocationExist = IsRelationLocationProductExist(locationId, productId);
            int res;
            if (!isRelationExistItemCode && isRelationLocationExist)
            {

                Conn = new DBConnection();
                string query = "INSERT INTO ref_location_product_itemcode (`product_uid`, `location_uid`, `item_code`, `added_by`) VALUES ( "
                    + DbHelper.SetDBValue(productId, false)
                    + DbHelper.SetDBValue(locationId, false)
                    + DbHelper.SetDBValue(itemCode, false)
                    + DbHelper.SetDBValue(userId, true)
                    + " );";
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

                
            }
            else
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Itemcode Already Existed"));
            }

            return CheckInsertionHelper(res);

        }

        public bool UpdateRelationItemCode(string locationId, string productId, string userId, string newItemCode, string oldItemCode)
        {
            bool isRelationExistOldItemCode = IsRelationItemCodeExist(locationId, productId, oldItemCode);
            bool isRelationExistNewItemCode = IsRelationItemCodeExist(locationId, productId, newItemCode);
            bool isRelationLocationExist = IsRelationLocationProductExist(locationId, productId);
            int res;
            if (isRelationExistOldItemCode && isRelationLocationExist && !isRelationExistNewItemCode)
            {
                Conn = new DBConnection();
                string query = "UPDATE ref_location_product_itemcode SET "
                    + " `item_code` = " + DbHelper.SetDBValue(newItemCode, true)
                    + " `updated_by` = " + DbHelper.SetDBValue(userId, true)
                    + " WHERE "
                    + " `location_uid` = " + DbHelper.SetDBValue(locationId, true)
                    + " AND `product_uid` = " + DbHelper.SetDBValue(productId, true)
                    + " AND `item_code` = " + DbHelper.SetDBValue(oldItemCode, true) + "; ";
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
            }
            else
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Itemcode Already Existed"));
            }

            return CheckUpdateHelper(res);
        }

        /*Going to need audit, to record whoever delete the iem*/
        public bool DeleteRelationItemCode(string locationId, string productId, string itemCode)
        {
            bool isRelationExistItemCode = IsRelationItemCodeExist(locationId, productId, itemCode);
            bool isRelationLocationExist = IsRelationLocationProductExist(locationId, productId);
            int res;
            if (isRelationExistItemCode && isRelationLocationExist)
            {
                Conn = new DBConnection();
                string query = "DELTE FROM ref_location_product_itemcode "
                    + " WHERE "
                    + " `location_uid` = " + DbHelper.SetDBValue(locationId, true)
                    + " AND `product_uid` = " + DbHelper.SetDBValue(productId, true)
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
            }
            else
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Itemcode Already Existed"));
            }

            return CheckUpdateHelper(res);

        }
    }
}
