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
    public class ProductLogic : BaseHelper, IProductLogic
    {

        private readonly IUserLogic _userLogic;
        private readonly ILocationProductRelationLogic _locationProductRelationLogic;
        public ProductLogic(IUserLogic userLogic)
        {
            _userLogic = userLogic;
            _locationProductRelationLogic = new LocationProductRelationLogic();
        }

        public bool AddProduct(ProductModel model, string userId, string locationId)
        {
            string id = null;
            bool isUnqiue = false;
            bool isUserValid = false;
            bool isSucess = false;
            bool isRelationSucess = false;
            while (!isUnqiue)
            {
                id = Guid.NewGuid().ToString();
                isUnqiue = VerifyUIdUnique(id);
            }

            isUserValid = _userLogic.VerifyUser(userId);

            if(isUserValid)
            {
                model.UId = id;
                model.AddedBy = userId;

                isSucess = AddProductExecution(model);

                if (isSucess)
                {
                    return AddProductExecutionRelation(model, userId, locationId);
                } else
                {
                    return isSucess;
                }
            } else
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "unauthorized access"));
            }

        }

        public bool UpdateProduct(ProductModel model, string userId, string locationId)
        {
            bool isUnqiue = false;
            bool isUserValid = false;
            bool isSucess = false;
            bool isRelationSucess = false;
            isUserValid = _userLogic.VerifyUser(userId);

            if (isUserValid)
            {
                model.UpdatedBy = userId;

                isSucess = UpdateProductExecution(model);

                if (isSucess)
                {
                    int retry = 0;
                    while (!isRelationSucess && retry > 5)
                    {
                        retry++;
                        isRelationSucess = UpdateProductExecutionRelation(model, locationId);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "unauthorized access"));
            }
        }

        public IEnumerable<ProductModel> GetProductByLocation(string userId, string locationId)
        {
            if (_userLogic.VerifyUser(userId))
            {
                return GetProductByLocationExecution(locationId);
            }
            else
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "unauthorized access"));
            }
        }

        private IEnumerable<ProductModel> GetProductByLocationExecution(string locId)
        {
            List<ProductModel> productList = new List<ProductModel>();
            this.Conn = new DBConnection();
            string query = "SELECT AL.* FROM asset_product AS AL"
                        + " INNER JOIN ref_location_product AS RLP"
                        + " ON AL.uid = RLP.product_uid"
                        + " AND RLP.location_uid = "
                        + DbHelper.SetDBValue(locId, true)
                        + " ORDER BY AL.updated_datetime DESC, AL.added_datetime DESC"
                        + " LIMIT 20; ";
            if (Conn.IsConnect())
            {
                Cmd = new MySqlCommand(query, this.Conn.Connection);
                Reader = Cmd.ExecuteReader();
                while (Reader.Read())
                {
                    ProductModel model = new ProductModel(
                        DbHelper.TryGet(Reader, "uid"),
                        DbHelper.TryGet(Reader, "description"),
                        DbHelper.TryGet(Reader, "second_description"),
                        DbHelper.TryGet(Reader, "third_description"),
                        DbHelper.TryGet(Reader, "upc"),
                        double.Parse(DbHelper.TryGet(Reader, "cost")),
                        double.Parse(DbHelper.TryGet(Reader, "price")),
                        DbHelper.TryGet(Reader, "added_datetime"),
                        DbHelper.TryGet(Reader, "updated_datetime"),
                        DbHelper.TryGet(Reader, "added_by"),
                        DbHelper.TryGet(Reader, "updated_by")
                    );
                    productList.Add(model);
                }
                this.Conn.Close();
                if(productList.Count > 0)
                {
                    return productList;
                } else
                {
                    throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "No Record Found"));
                }
            } else
            {
                throw DbConnException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name));
            }

        }

        public ProductModel GetProductById(string userId, string locationId, Dictionary<string, string> param)
        {
            param.TryGetValue("uid", out string uid);
            param.TryGetValue("itemCode", out string itemCode);
            param.TryGetValue("upc", out string upc);
            string whereClause;
            if(uid != null)
            {
                whereClause = " WHERE AP.uid = " + DbHelper.SetDBValue(uid, true) + " ";
            } else if (itemCode != null)
            {
                whereClause = " INNER JOIN REF_LOCATION_PRODUCT_ITEMCODE AS RLPI "
                                + " ON RLP.location_uid = RLPI.location_uid AND AP.uid = RLPI.product_uid AND RLPI.item_code = " + DbHelper.SetDBValue(itemCode, true) + " ";
            } else if (upc != null)
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Not Implemented"));
            } else
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Invalid Argument"));
            }

            if(_userLogic.VerifyUser(userId))
            {
                return GetProductByIdExecution(locationId, whereClause);
            } else
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Authorized User"));
            }
        }

        private ProductModel GetProductByIdExecution(string locationId, string where)
        {
            ProductModel model = null;
            Conn = new DBConnection();
            string query = "SELECT AP.* FROM ASSET_PRODUCT AS AP "
                            + " INNER JOIN REF_LOCATION_PRODUCT AS RLP "
                            + " ON AP.uid = RLP.product_uid AND RLP.location_uid = " + DbHelper.SetDBValue(locationId, true)
                            + " " + where
                            + " LIMIT 1;";
            Debug.WriteLine(query);
            try
            {
                if(Conn.IsConnect())
                {
                    Cmd = new MySqlCommand(query, this.Conn.Connection);
                    Reader = Cmd.ExecuteReader();
                    while (Reader.Read())
                    {
                        model = new ProductModel() {
                            UId = DbHelper.TryGet(Reader, "uid"),
                            Description = DbHelper.TryGet(Reader, "description"),
                            SecondDescription = DbHelper.TryGet(Reader, "second_description"),
                            ThirdDescription = DbHelper.TryGet(Reader, "third_description"),
                            ItemCode = DbHelper.TryGet(Reader, "rel_item_code"),
                            Cost = double.Parse(DbHelper.TryGet(Reader, "cost")),
                            Price = double.Parse(DbHelper.TryGet(Reader, "price")),
                            AddedDateTime = DbHelper.TryGet(Reader, "added_datetime"),
                            UpdatedDateTime = DbHelper.TryGet(Reader, "updated_datetime"),
                            AddedBy = DbHelper.TryGet(Reader, "added_by"),
                            UpdatedBy = DbHelper.TryGet(Reader, "updated_by")
                        };
                    }
                    Conn.Close();
                } else
                {
                    throw DbConnException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name));
                }
            } catch(Exception e)
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, e.ToString()));
            }

            if (model != null)
            {
                List<string> itemCodeList = GetProductItemCode(locationId, model.UId);
                model.ItemCodeList = itemCodeList;
                return model;
            }
            else
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "No Record Found"));
            }
        }

        private List<string> GetProductItemCode(string locationId, string productId)
        {
            List<string> listItemCode = new List<string>();
            Conn = new DBConnection();
            string query = "SELECT RLPI.item_code FROM REF_LOCATION_PRODUCT_ITEMCODE AS RLPI "
                          + " WHERE product_uid = " + DbHelper.SetDBValue(productId, true) 
                          + " AND location_uid = " + DbHelper.SetDBValue(locationId, true) 
                          +  " ;";
            Debug.WriteLine(query);
            try
            {
                if (Conn.IsConnect())
                {
                    Cmd = new MySqlCommand(query, this.Conn.Connection);
                    Reader = Cmd.ExecuteReader();
                    while (Reader.Read())
                    {
                        string itemCode = DbHelper.TryGet(Reader, "item_code");
                        listItemCode.Add(itemCode);
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
            return listItemCode;
        }

        private bool VerifyUIdUnique(string uid)
        {
            this.Conn = new DBConnection();
            string id = null;
            string query = "SELECT uid FROM asset_product WHERE uid = " + DbHelper.SetDBValue(uid, true) + ";";
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

            return VerifyNotExist(id);
        }

        public bool VerifyUIdExist(string uid)
        {
            this.Conn = new DBConnection();
            string id = null;
            string query = "SELECT uid FROM asset_product WHERE uid = " + DbHelper.SetDBValue(uid, true) + ";";
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

        private bool AddProductExecution(ProductModel model)
        {
            int res = 0;
            Conn = new DBConnection();

            string query = "INSERT INTO asset_product"
                + " (`uid`,`description`,`second_description`, `third_description`, `upc`, `cost`, `price`,`added_by`)"
                + " VALUES ( "
                + DbHelper.SetDBValue(model.UId, false)
                + DbHelper.SetDBValue(model.Description, false)
                + DbHelper.SetDBValueNull(model.SecondDescription, false)
                + DbHelper.SetDBValueNull(model.ThirdDescription, false)
                + DbHelper.SetDBValueNull(model.Upc, false)
                + DbHelper.SetDBValueNull(model.Cost.ToString(), false)
                + DbHelper.SetDBValue(model.Price.ToString(), false)
                + DbHelper.SetDBValue(model.AddedBy, true)
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
                throw DbInsertException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, e.ToString()));
            }

            return CheckInsertionHelper(res);
        }
        
        private bool AddProductExecutionRelation(ProductModel model, string userId, string locationId)
        {
            int res = 0;
            Conn = new DBConnection();

            string query = "INSERT INTO ref_location_product"
                + " (`product_uid`,`location_uid`, `added_by`)"
                + " VALUES ( "
                + DbHelper.SetDBValue(model.UId, false)
                + DbHelper.SetDBValue(locationId, false)
                + DbHelper.SetDBValue(userId, true)
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
                throw DbInsertException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, e.ToString()));
            }


            return CheckInsertionHelper(res);
        }

        private bool UpdateProductExecution(ProductModel model)
        {
            int res = 0;
            Conn = new DBConnection();

            string query = "UPDATE asset_product"
                + " SET "
                + " `description` = " + DbHelper.SetDBValue(model.Description, false)
                + " `second_description` = " + DbHelper.SetDBValueNull(model.SecondDescription, false)
                + " `third_description` = " + DbHelper.SetDBValueNull(model.ThirdDescription, false)
                + " `upc` = " + DbHelper.SetDBValueNull(model.Upc, false)
                + " `cost` = " + DbHelper.SetDBValueNull(model.Cost, false)
                + " `price` = " + DbHelper.SetDBValue(model.Price.ToString(), false)
                + " `updated_by` = " + DbHelper.SetDBValue(model.UpdatedBy, true)
                + " WHERE uid = " + DbHelper.SetDBValue(model.UId, true);
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

        private bool UpdateProductExecutionRelation(ProductModel model, string locationId)
        {
            int res = 0;
            Conn = new DBConnection();
            string query = " UPDATE ref_location_product"
                + " SET "
                + " `updated_by` = " + DbHelper.SetDBValue(model.UpdatedBy, true)
                + " WHERE `product_uid` = " + DbHelper.SetDBValue(model.UId, true)
                + " AND `location_uid` = " + DbHelper.SetDBValue(locationId, true);
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


    }
}
