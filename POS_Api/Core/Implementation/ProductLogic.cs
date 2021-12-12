using MySql.Data.MySqlClient;
using POS_Api.Core.Interface;
using POS_Api.Database.MySql.Configuration;
using POS_Api.Model;
using POS_Api.Model.EnumData;
using POS_Api.Model.ViewModel;
using POS_Api.Repository.Implementation;
using POS_Api.Repository.Interface;
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

        private readonly IUserRepos _userRepos;
        private readonly IProductRepos _productRepos;
        public ProductLogic()
        {
            _userRepos = new UserRepos();
            _productRepos = new ProductRepos();
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
                isUnqiue = _productRepos.VerifyUIdUnique(id);
            }

            isUserValid = _userRepos.VerifyUser(userId);

            if(isUserValid)
            {
                model.UId = id;
                model.AddedBy = userId;

                isSucess = _productRepos.AddProductExecution(model);

                if (isSucess)
                {
                    return _productRepos.AddProductExecutionRelation(model, userId, locationId);
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
            isUserValid = _userRepos.VerifyUser(userId);

            if (isUserValid)
            {
                model.UpdatedBy = userId;

                isSucess = _productRepos.UpdateProductExecution(model);

                if (isSucess)
                {
                    int retry = 0;
                    while (!isRelationSucess && retry > 5)
                    {
                        retry++;
                        isRelationSucess = _productRepos.UpdateProductExecutionRelation(model, locationId);
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
            if (_userRepos.VerifyUser(userId))
            {
                return _productRepos.GetProductByLocationExecution(locationId);
            }
            else
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "unauthorized access"));
            }
        }


        public ProductModelVm GetProductById(string userId, string locationId, Dictionary<string, string> param)
        {
            param.TryGetValue("uid", out string uid);
            param.TryGetValue("itemCode", out string itemCode);
            param.TryGetValue("upc", out string upc);
            string whereClause;
            if (uid != null)
            {
                whereClause = " WHERE AP.uid = " + DbHelper.SetDBValue(uid, true) + " ";
            }
            else if (itemCode != null)
            {
                whereClause = " INNER JOIN REF_LOCATION_PRODUCT_ITEMCODE AS RLPI "
                                + " ON RLP.location_uid = RLPI.location_uid AND AP.uid = RLPI.product_uid AND RLPI.item_code = " + DbHelper.SetDBValue(itemCode, true) + " ";
            }
            else if (upc != null)
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Not Implemented"));
            }
            else
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Invalid Argument"));
            }

            if (_userRepos.VerifyUser(userId))
            {
                ProductModel model = _productRepos.GetProductByIdExecution(locationId, whereClause);
                
                if (model == null)
                {
                    throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "No Product Record Found"));
                }

                List<CategoryModel> cateList = GetProductCategoryList(model.UId, locationId);
                List<DepartmentModel> deptList = GetProductDepartmentList(model.UId, locationId);
                List<SectionModel> secList = GetProductSectionList(model.UId, locationId);
                List<VendorModel> venList = GetProductVendorList(model.UId, locationId);
                List<TaxModel> taxList = GetProductTaxList(model.UId, locationId);
                List<DiscountModel> discList = GetProductDiscountList(model.UId, locationId);

                ProductModelVm viewModel = new ProductModelVm(model);
                viewModel.CategoryList = cateList;
                viewModel.DepartmentList = deptList;
                viewModel.SectionList = secList;
                viewModel.VendorList = venList;
                viewModel.TaxList = taxList;
                viewModel.DiscountList = discList;

                return viewModel;
            }
            else
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Authorized User"));
            }
        }








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


    }
}
