using MySql.Data.MySqlClient;
using POS_Api.Database.MySql.Configuration;
using POS_Api.Model;
using POS_Api.Model.ViewModel;
using POS_Api.Repository.Interface;
using POS_Api.Shared.DbHelper;
using POS_Api.Shared.ExceptionHelper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;


namespace POS_Api.Repository.Implementation
{
    public class ProductRepos : BaseHelper, IProductRepos
    {
        private readonly IUserRepos _userRepos;

        public ProductRepos()
        {
            _userRepos = new UserRepos();
        }

        public IEnumerable<ProductModel> GetProductByLocationExecution(string locId)
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
                        DbHelper.TryGet(Reader, "updated_by"),
                        DbHelper.TryGetBoolean(Reader, "apply_to_ui")
                    );
                    productList.Add(model);
                }
                this.Conn.Close();
                if (productList.Count > 0)
                {
                    return productList;
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

        public int GetProductPaginateCount(string locId, string where)
        {
            this.Conn = new DBConnection();
            string query = "SELECT count(*) as count FROM asset_product AS AL"
                + " INNER JOIN ref_location_product AS RLP"
                + " ON AL.uid = RLP.product_uid"
                + " AND RLP.location_uid = "
                + DbHelper.SetDBValue(locId, true) + " "
                + " LEFT JOIN "
                + " ( "
                + " SELECT RLPI.product_uid, GROUP_CONCAT(RLPI.item_code) as item_code FROM ref_location_product_itemcode as RLPI "
                + " GROUP BY RLPI.product_uid "
                + " ) AS ITEM_CODE "
                + " ON AL.uid = ITEM_CODE.product_uid "
                + " LEFT JOIN "
                + " ( "
                + " SELECT RLPU.product_uid, GROUP_CONCAT(RLPU.upc) as upc FROM ref_location_product_upc as RLPU "
                + " GROUP BY RLPU.product_uid "
                + " ) AS UPC "
                + " ON AL.uid = UPC.product_uid "
                + where
                + " ORDER BY AL.updated_datetime DESC, AL.added_datetime DESC;";
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
                throw DbConnException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name));
            }
            return count;
        }

        public IEnumerable<ProductModel> GetProductPaginateByDefault(string locId, int startIdx, int endIdx, string where)
        {
            List<ProductModel> productList = new List<ProductModel>();
            this.Conn = new DBConnection();
            string query = "SELECT AL.* FROM asset_product AS AL"
                        + " INNER JOIN ref_location_product AS RLP"
                        + " ON AL.uid = RLP.product_uid"
                        + " AND RLP.location_uid = "
                        + DbHelper.SetDBValue(locId, true) + " "
                        + " LEFT JOIN "
                        + " ( "
                        + " SELECT RLPI.product_uid, GROUP_CONCAT(RLPI.item_code) as item_code FROM ref_location_product_itemcode as RLPI "
                        + " GROUP BY RLPI.product_uid "
                        + " ) AS ITEM_CODE "
                        + " ON AL.uid = ITEM_CODE.product_uid "
                        + " LEFT JOIN "
                        + " ( "
                        + " SELECT RLPU.product_uid, GROUP_CONCAT(RLPU.upc) as upc FROM ref_location_product_upc as RLPU "
                        + " GROUP BY RLPU.product_uid "
                        + " ) AS UPC "
                        + " ON AL.uid = UPC.product_uid "
                        + where
                        + " ORDER BY AL.updated_datetime DESC, AL.added_datetime DESC"
                        + "; ";
            if (Conn.IsConnect())
            {
                Cmd = new MySqlCommand(query, this.Conn.Connection);
                Reader = Cmd.ExecuteReader();
                while (Reader.Read())
                {
                    ProductModel model = new ProductModel()
                    {
                        UId = DbHelper.TryGet(Reader, "uid"),
                        Description = DbHelper.TryGet(Reader, "description"),
                        SecondDescription = DbHelper.TryGet(Reader, "second_description"),
                        ThirdDescription = DbHelper.TryGet(Reader, "third_description"),
                        Upc = int.Parse(DbHelper.TryGet(Reader, "upc")),
                        ItemCode = int.Parse(DbHelper.TryGet(Reader, "item_code")),
                        Cost = double.Parse(DbHelper.TryGet(Reader, "cost")),
                        Price = double.Parse(DbHelper.TryGet(Reader, "price")),
                        AddedDateTime = DbHelper.TryGet(Reader, "added_datetime"),
                        UpdatedDateTime = DbHelper.TryGet(Reader, "updated_datetime"),
                        AddedBy = DbHelper.TryGet(Reader, "added_by"),
                        UpdatedBy = DbHelper.TryGet(Reader, "updated_by"),
                        ApplyToUI = DbHelper.TryGetBoolean(Reader, "apply_to_ui"),
                    };
                    productList.Add(model);
                }
                Conn.Close();
                return productList;
            }
            else
            {
                Conn.Close();
                throw DbConnException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name));
            }
        }


        public ProductModel GetProductByIdExecution(string locationId, string where)
        {
            ProductModel model = null;
            Conn = new DBConnection();
            string query = "SELECT AP.* FROM ASSET_PRODUCT AS AP "
                            + " INNER JOIN REF_LOCATION_PRODUCT AS RLP "
                            + " ON AP.uid = RLP.product_uid AND RLP.location_uid = " + DbHelper.SetDBValue(locationId, true)
                            + " " + where
                            + " LIMIT 1;";
            try
            {
                if (Conn.IsConnect())
                {
                    Cmd = new MySqlCommand(query, this.Conn.Connection);
                    Reader = Cmd.ExecuteReader();
                    while (Reader.Read())
                    {
                        model = new ProductModel()
                        {
                            UId = DbHelper.TryGet(Reader, "uid"),
                            Description = DbHelper.TryGet(Reader, "description"),
                            SecondDescription = DbHelper.TryGet(Reader, "second_description"),
                            ThirdDescription = DbHelper.TryGet(Reader, "third_description"),
                            Cost = double.Parse(DbHelper.TryGet(Reader, "cost")),
                            Price = double.Parse(DbHelper.TryGet(Reader, "price")),
                            AddedDateTime = DbHelper.TryGet(Reader, "added_datetime"),
                            UpdatedDateTime = DbHelper.TryGet(Reader, "updated_datetime"),
                            AddedBy = DbHelper.TryGet(Reader, "added_by"),
                            UpdatedBy = DbHelper.TryGet(Reader, "updated_by"),
                            ApplyToUI = DbHelper.TryGetBoolean(Reader, "apply_to_ui"),
                        };
                    }
                }
                else
                {
                    Conn.Close();
                    throw DbConnException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name));
                }
            }
            catch (Exception e)
            {
                Conn.Close();
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, e.ToString()));
            }
            Conn.Close();
            if (model != null)
            {
                List<string> itemCodeList = GetProductItemCode(locationId, model.UId);
                model.ItemCodeList = itemCodeList;
                List<string> upcList = GetProductUpc(locationId, model.UId);
                model.UpcList = upcList;
                return model;
            }
            else
            {
                return null;
            }
        }

        public List<string> GetProductItemCode(string locationId, string productId)
        {
            List<string> listItemCode = new List<string>();
            Conn = new DBConnection();
            string query = "SELECT RLPI.item_code FROM REF_LOCATION_PRODUCT_ITEMCODE AS RLPI "
                          + " WHERE product_uid = " + DbHelper.SetDBValue(productId, true)
                          + " AND location_uid = " + DbHelper.SetDBValue(locationId, true)
                          + " ;";
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

        public List<string> GetProductUpc(string locationId, string productId)
        {
            List<string> listUpc= new List<string>();
            Conn = new DBConnection();
            string query = "SELECT RLPI.upc FROM REF_LOCATION_PRODUCT_UPC AS RLPI "
                          + " WHERE product_uid = " + DbHelper.SetDBValue(productId, true)
                          + " AND location_uid = " + DbHelper.SetDBValue(locationId, true)
                          + " ;";
            try
            {
                if (Conn.IsConnect())
                {
                    Cmd = new MySqlCommand(query, this.Conn.Connection);
                    Reader = Cmd.ExecuteReader();
                    while (Reader.Read())
                    {
                        string upc = DbHelper.TryGet(Reader, "upc");
                        listUpc.Add(upc);
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
            return listUpc;
        }

        public bool VerifyUIdUnique(string uid)
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

        public bool AddProductExecution(ProductModel model)
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

        public bool AddProductExecutionRelation(ProductModel model, string userId, string locationId)
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

        public bool UpdateProductExecution(ProductModel model)
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

        public bool UpdateProductExecutionRelation(ProductModel model, string locationId)
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


        public ProductModelVm GetProductByIdWithMapExecution(string userId, string locationId, Dictionary<string, string> param, bool isCheckout)
        {
            Console.WriteLine(isCheckout);
            param.TryGetValue("uid", out string uid);
            param.TryGetValue("itemCode", out string itemCode);
            param.TryGetValue("upc", out string upc);
            param.TryGetValue("searchText", out string searchText);
            string whereClause = "";
            if (isCheckout)
            {
                if (!string.IsNullOrWhiteSpace(upc))
                {
                    whereClause = " INNER JOIN REF_LOCATION_PRODUCT_UPC AS RLPI "
                                     + " ON RLP.location_uid = RLPI.location_uid AND AP.uid = RLPI.product_uid AND RLPI.upc = " + DbHelper.SetDBValue(upc, true) + " ";
                }
            }
            else {
                if (!string.IsNullOrWhiteSpace(uid))
                {
                    whereClause = " WHERE AP.uid = " + DbHelper.SetDBValue(uid, true) + " ";
                }
                else if (!string.IsNullOrWhiteSpace(itemCode))
                {
                    whereClause = " INNER JOIN REF_LOCATION_PRODUCT_ITEMCODE AS RLPI "
                                    + " ON RLP.location_uid = RLPI.location_uid AND AP.uid = RLPI.product_uid AND RLPI.item_code = " + DbHelper.SetDBValue(itemCode, true) + " ";
                }
                else if (!string.IsNullOrWhiteSpace(searchText))
                {
                    //whereClause = " WHERE AP.description = '" + searchText + "'";
                    whereClause = " WHERE 1 = 0";

                }
                else if (!string.IsNullOrWhiteSpace(upc))
                {
                    whereClause = " INNER JOIN REF_LOCATION_PRODUCT_UPC AS RLPI "
                                     + " ON RLP.location_uid = RLPI.location_uid AND AP.uid = RLPI.product_uid AND RLPI.upc = " + DbHelper.SetDBValue(upc, true) + " ";
                }
                else
                {
                    throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Invalid Argument"));
                }
            }
            

            if (_userRepos.VerifyUser(userId))
            {
                ProductModel model = GetProductByIdExecution(locationId, whereClause);

                if (model == null)
                {
                    return null;
                }

                List<CategoryModel> cateList = GetProductCategoryList(model.UId, locationId);
                List<DepartmentModel> deptList = GetProductDepartmentList(model.UId, locationId);
                List<SectionModel> secList = GetProductSectionList(model.UId, locationId);
                List<VendorModel> venList = GetProductVendorList(model.UId, locationId);
                List<TaxModel> taxList = GetProductTaxList(model.UId, locationId);
                List<DiscountModel> discList = GetProductDiscountList(model.UId, locationId);

                ProductModelVm viewModel = new ProductModelVm(model)
                {
                    CategoryList = cateList,
                    DepartmentList = deptList,
                    SectionList = secList,
                    VendorList = venList,
                    TaxList = taxList,
                    DiscountList = discList
                };

                return viewModel;
            }
            else
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Authorized User"));
            }
        }

        #region INVOLVED CODE FROM OTHER REPOS -- MUST MOVE IN FUTURE
        private List<CategoryModel> GetProductCategoryList(string productId, string locationId)
        {
            List<CategoryModel> lst = new List<CategoryModel>();
            Conn = new DBConnection();
            string query = " SELECT AC.* FROM ref_location_product_category as RLPC "
                            + " INNER JOIN asset_category as AC "
                            + " ON RLPC.category_uid = AC.uid AND RLPC.location_uid = " + DbHelper.SetDBValue(locationId, true)
                            + " WHERE RLPC.product_uid = " + DbHelper.SetDBValue(productId, true) + "; ";
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

            return lst;
        }

        private List<DepartmentModel> GetProductDepartmentList(string productId, string locationId)
        {
            List<DepartmentModel> lst = new List<DepartmentModel>();
            Conn = new DBConnection();
            string query = " SELECT AC.* FROM ref_location_product_department as RLPC "
                            + " INNER JOIN asset_department as AC "
                            + " ON RLPC.department_uid = AC.uid AND RLPC.location_uid = " + DbHelper.SetDBValue(locationId, true)
                            + " WHERE RLPC.product_uid = " + DbHelper.SetDBValue(productId, true) + "; ";
            try
            {
                if (Conn.IsConnect())
                {
                    Cmd = new MySqlCommand(query, this.Conn.Connection);
                    Reader = Cmd.ExecuteReader();
                    while (Reader.Read())
                    {
                        DepartmentModel model = new DepartmentModel()
                        {
                            UId = DbHelper.TryGet(Reader, "uid"),
                            Description = DbHelper.TryGet(Reader, "description"),
                            LocationUId = DbHelper.TryGet(Reader, "location_uid"),
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

            return lst;
        }

        private List<SectionModel> GetProductSectionList(string productId, string locationId)
        {
            List<SectionModel> lst = new List<SectionModel>();
            Conn = new DBConnection();
            string query = " SELECT AC.* FROM ref_location_product_section as RLPC "
                            + " INNER JOIN asset_section as AC "
                            + " ON RLPC.section_uid = AC.uid AND RLPC.location_uid = " + DbHelper.SetDBValue(locationId, true)
                            + " WHERE RLPC.product_uid = " + DbHelper.SetDBValue(productId, true) + "; ";
            try
            {
                if (Conn.IsConnect())
                {
                    Cmd = new MySqlCommand(query, this.Conn.Connection);
                    Reader = Cmd.ExecuteReader();
                    while (Reader.Read())
                    {
                        SectionModel model = new SectionModel()
                        {
                            UId = DbHelper.TryGet(Reader, "uid"),
                            Description = DbHelper.TryGet(Reader, "description"),
                            LocationUId = DbHelper.TryGet(Reader, "location_uid"),
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

            return lst;
        }

        private List<VendorModel> GetProductVendorList(string productId, string locationId)
        {
            List<VendorModel> lst = new List<VendorModel>();
            Conn = new DBConnection();
            string query = " SELECT AC.* FROM ref_location_product_vendor as RLPC "
                            + " INNER JOIN asset_vendor as AC "
                            + " ON RLPC.vendor_uid = AC.uid AND RLPC.location_uid = " + DbHelper.SetDBValue(locationId, true)
                            + " WHERE RLPC.product_uid = " + DbHelper.SetDBValue(productId, true) + "; ";
            try
            {
                if (Conn.IsConnect())
                {
                    Cmd = new MySqlCommand(query, this.Conn.Connection);
                    Reader = Cmd.ExecuteReader();
                    while (Reader.Read())
                    {
                        VendorModel model = new VendorModel()
                        {
                            UId = DbHelper.TryGet(Reader, "uid"),
                            Description = DbHelper.TryGet(Reader, "description"),
                            LocationUId = DbHelper.TryGet(Reader, "location_uid"),
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

            return lst;
        }

        private List<TaxModel> GetProductTaxList(string productId, string locationId)
        {
            List<TaxModel> lst = new List<TaxModel>();
            Conn = new DBConnection();
            string query = " SELECT AC.* FROM ref_product_tax as RLPC "
                            + " INNER JOIN asset_tax as AC "
                            + " ON RLPC.tax_uid = AC.uid AND RLPC.location_uid = " + DbHelper.SetDBValue(locationId, true)
                            + " WHERE RLPC.product_uid = " + DbHelper.SetDBValue(productId, true) + "; ";
            try
            {
                if (Conn.IsConnect())
                {
                    Cmd = new MySqlCommand(query, this.Conn.Connection);
                    Reader = Cmd.ExecuteReader();
                    while (Reader.Read())
                    {
                        TaxModel model = new TaxModel()
                        {
                            UId = DbHelper.TryGet(Reader, "uid"),
                            Description = DbHelper.TryGet(Reader, "description"),
                            Rate = double.Parse(DbHelper.TryGet(Reader, "rate")),
                            LocationUId = DbHelper.TryGet(Reader, "location_uid"),
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

            return lst;
        }

        private List<DiscountModel> GetProductDiscountList(string productId, string locationId)
        {
            List<DiscountModel> lst = new List<DiscountModel>();
            Conn = new DBConnection();
            string query = " SELECT AC.* FROM ref_product_discount as RLPC "
                            + " INNER JOIN asset_discount as AC "
                            + " ON RLPC.discount_uid = AC.uid AND RLPC.location_uid = " + DbHelper.SetDBValue(locationId, true)
                            + " WHERE RLPC.product_uid = " + DbHelper.SetDBValue(productId, true) + "; ";
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
                            Rate = double.Parse(DbHelper.TryGet(Reader, "rate")),
                            LocationUId = DbHelper.TryGet(Reader, "location_uid"),
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

            return lst;
        }
        #endregion

        #region PRODUCT LOCATION RELATION
        public bool IsRelationLocationProductExist(string locationId, string productId)
        {
            Conn = new DBConnection();
            string id = null;
            string query = "SELECT id FROM ref_location_product"
                            + " WHERE PRODUCT_UID = " + DbHelper.SetDBValue(productId, true) + " AND"
                            + " LOCATION_UID = " + DbHelper.SetDBValue(locationId, true) + "; ";
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
                            + " WHERE "
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
                    return false;
                    //throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, e.ToString()));
                }


            }
            else
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Itemcode Already Existed"));
            }

            return CheckInsertionHelper(res);

        }

        public bool AddRelationUpc(string locationId, string productId, string userId, string upc)
        {
            bool isRelationExistItemCode = IsRelationItemCodeExist(locationId, productId, upc);
            bool isRelationLocationExist = IsRelationLocationProductExist(locationId, productId);
            int res;

            if (!isRelationExistItemCode && isRelationLocationExist)
            {

                Conn = new DBConnection();
                string query = "INSERT INTO ref_location_product_upc (`product_uid`, `location_uid`, `upc`, `added_by`) VALUES ( "
                    + DbHelper.SetDBValue(productId, false)
                    + DbHelper.SetDBValue(locationId, false)
                    + DbHelper.SetDBValue(upc, false)
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
                    return false;
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

        public bool IsProductLocationExist(string locationId, string productId)
        {
            Conn = new DBConnection();
            string id = null;
            string query = " SELECT id FROM ref_location_product WHERE "
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
        #endregion
    }
}
