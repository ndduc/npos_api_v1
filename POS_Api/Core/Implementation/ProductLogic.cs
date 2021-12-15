using MySql.Data.MySqlClient;
using POS_Api.Core.Interface;
using POS_Api.Database.MySql.Configuration;
using POS_Api.Model;
using POS_Api.Model.EnumData;
using POS_Api.Model.ReponseViewModel;
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
        private readonly IProductRepos _productRepos;
        private readonly IUserRepos _userRepos;
        private readonly ILocationRepos _locationRepos;

        private readonly IDepartmentRepos _departmentRepos;
        private readonly ICategoryRepos _categoryRepos;
        private readonly IVendorRepos _vendorRepos;
        private readonly ISectionRepos _sectionRepos;
        private readonly IDiscountRepos _discountRepos;
        private readonly ITaxRepos _taxRepos;
        public ProductLogic()
        {
            _userRepos = new UserRepos();
            _productRepos = new ProductRepos();
            _locationRepos = new LocationRepos();
            _departmentRepos = new DepartmentRepos();
            _categoryRepos = new CategoryRepos();
            _vendorRepos = new VendorRepos();
            _sectionRepos = new SectionRepos();
            _discountRepos = new DiscountRepos();
            _taxRepos = new TaxRepos();
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

        public ProductAddModelVm AddProduct(ProductModel model, Dictionary<string, string> param)
        {
            string id = null;
            bool isUnqiue = false;

            param.TryGetValue("userid", out string userId);
            param.TryGetValue("locid", out string locId);
            param.TryGetValue("departmentList", out string dept);
            param.TryGetValue("categoryList", out string cate);
            param.TryGetValue("vendorList", out string ven);
            param.TryGetValue("sectionList", out string sec);
            param.TryGetValue("discount", out string discount);
            param.TryGetValue("tax", out string tax);
            param.TryGetValue("itemCodeList", out string itemCode);
            param.TryGetValue("upcList", out string upc);



            // Verify User
            bool isUserValid = _userRepos.VerifyUser(userId);   
            // Verify Location
            bool isLocationValid = _locationRepos.VerifyUIdExist(locId);

            if(!isUserValid)
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Invalid User"));
            }

            if(!isLocationValid)
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Invalid Location"));
            }


            while (!isUnqiue)
            {
                id = Guid.NewGuid().ToString();
                isUnqiue = _productRepos.VerifyUIdUnique(id);
            }

            // Do Check Relation Check
            // No Need To Check Product Location Relation as the product uid is always new

            List<string> deptList = GetListFromString(dept);
            List<string> cateList = GetListFromString(cate);
            List<string> venList = GetListFromString(ven);
            List<string> secList = GetListFromString(sec);
            List<string> itemCodeList = GetListFromString(itemCode);
            List<string> upcList = GetListFromString(upc);
            List<string> taxList = GetListFromString(tax);
            List<string> discountList = GetListFromString(discount);

            if(itemCodeList.Count > 0)
            {
                model.ItemCode = 1;
            } else
            {
                model.ItemCode = 0;
            }

            if (upcList.Count > 0)
            {
                model.Upc = 1;
            }
            else
            {
                model.Upc = 0;
            }

            model.UId = id;
            model.AddedBy = userId;

            bool isSucess = _productRepos.AddProductExecution(model);

            ProductAddModelVm responseModel = new ProductAddModelVm();

            if (isSucess)
            {
                bool isProductLocationRelationSucess = _productRepos.AddProductExecutionRelation(model, userId, locId);

                bool isDepartmentSucess = AddProductFunctionHelper(deptList, model.UId, userId, locId, "DEPARTMENT");
                bool isCategorySucess = AddProductFunctionHelper(cateList, model.UId, userId, locId, "CATEGORY");
                bool isVendorSucess = AddProductFunctionHelper(venList, model.UId, userId, locId, "VENDOR");
                bool isSectionSucess = AddProductFunctionHelper(secList, model.UId, userId, locId, "SECTION");
                bool isTaxSucess = AddProductFunctionHelper(taxList, model.UId, userId, locId, "TAX");
                bool isDiscountSucess = AddProductFunctionHelper(discountList, model.UId, userId, locId, "DISCOUNT");
                bool isItemCodeSucess = AddProductFunctionHelper(itemCodeList, model.UId, userId, locId, "ITEMCODE");
                bool isUpcSucess = AddProductFunctionHelper(upcList, model.UId, userId, locId, "UPC");

                // Still Missing Upc Logic

                #region SET RESPONSE MODEL
                if (isProductLocationRelationSucess)
                {
                    responseModel.Product_Location_Status = "OK";
                } else
                {
                    responseModel.Product_Location_Status = "FAILED";
                }

                if (isDepartmentSucess)
                {
                    responseModel.Product_Department = "OK";
                }
                else
                {
                    responseModel.Product_Department = "FAILED";
                }

                if (isCategorySucess)
                {
                    responseModel.Product_Category = "OK";
                }
                else
                {
                    responseModel.Product_Category = "FAILED";
                }

                if (isVendorSucess)
                {
                    responseModel.Product_Vendor = "OK";
                }
                else
                {
                    responseModel.Product_Vendor = "FAILED";
                }

                if (isSectionSucess)
                {

                    responseModel.Product_Section = "OK";
                }
                else
                {
                    responseModel.Product_Section = "FAILED";
                }

                if (isTaxSucess)
                {
                    responseModel.Product_Tax = "OK";
                }
                else
                {
                    responseModel.Product_Tax = "FAILED";
                }

                if (isDiscountSucess)
                {
                    responseModel.Product_Discount = "OK";
                }
                else
                {
                    responseModel.Product_Discount = "FAILED";
                }

                if (isItemCodeSucess)
                {
                    responseModel.Product_ItemCode = "OK";
                }
                else
                {
                    responseModel.Product_ItemCode = "FAILED";
                }

                if (isUpcSucess)
                {
                    responseModel.Product_Upc = "OK";
                }
                else
                {
                    responseModel.Product_Upc = "FAILED";
                }
                #endregion

                return responseModel;
            }
            else
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Product Is Not Sucessfully Added"));
            }
        }

        private bool AddProductFunctionHelper(List<string> itemList, string productId, string userId, string locationId, string option)
        {
            bool checker = false;

            if(itemList.Count > 0)
            {
                switch (option)
                {
                    case "CATEGORY":
                        checker = _categoryRepos.AddCategoryExecutionFromList(itemList, productId, locationId, userId);
                        break;
                    case "DEPARTMENT":
                        checker = _departmentRepos.AddDepartmentExecutionFromList(itemList, productId, locationId, userId);
                        break;
                    case "SECTION":
                        checker = _sectionRepos.AddSectionExecutionFromList(itemList, productId, locationId, userId);
                        break;
                    case "VENDOR":
                        checker = _vendorRepos.AddVendorExecutionFromList(itemList, productId, locationId, userId);
                        break;
                    case "TAX":
                        checker = _taxRepos.AddTaxExecutionFromList(itemList, productId, locationId, userId);
                        break;
                    case "DISCOUNT":
                        checker = _discountRepos.AddDiscountExecutionFromList(itemList, productId, locationId, userId);
                        break;
                    case "ITEMCODE":
                        checker = AddItemCodeExecutionFromList(itemList, productId, locationId, userId);
                        break;
                    case "UPC":
                        checker = AddUpcExecutionFromList(itemList, productId, locationId, userId);
                        break;
                    default:
                        break;
                }
            }
            
            return checker;
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
            return _productRepos.GetProductByIdWithMapExecution(userId, locationId, param);
        }

        public bool AddRelationItemCode(string locationId, string productId, string userId, string itemCode)
        {
            return _productRepos.AddRelationItemCode(locationId, productId, userId, itemCode);
        }

        public bool AddRelationUpc(string locationId, string productId, string userId, string upc)
        {
            return _productRepos.AddRelationUpc(locationId, productId, userId, upc);
        }

        private bool AddItemCodeExecutionFromList(List<string> itemCodeList, string productId, string locationId, string userId)
        {
            List<bool> exectutedList = new List<bool>();
            foreach (string item in itemCodeList)
            {
                exectutedList.Add(AddRelationItemCode(locationId, productId, userId, item));
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

        private bool AddUpcExecutionFromList(List<string> upcList, string productId, string locationId, string userId)
        {
            List<bool> exectutedList = new List<bool>();
            foreach (string item in upcList)
            {
                exectutedList.Add(AddRelationUpc(locationId, productId, userId, item));
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


        public int GetProductPaginateCount(Dictionary<string, string> param)
        {
            param.TryGetValue("locationId", out string locationId);
            param.TryGetValue("searchType", out string searchType);
            try
            {
                return _productRepos.GetProductPaginateCount(locationId);
                // Search Type indicate search by default (locId) or itemId, etc ...
            } catch (Exception e)
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, e.ToString()));
            }   
        }

        public IEnumerable<ProductModel> GetProductPaginate(Dictionary<string, string> param)
        {

            param.TryGetValue("locationId", out string locationId);
            param.TryGetValue("itemCode", out string itemCode);
            param.TryGetValue("startIdx", out string startIdx);
            param.TryGetValue("endIdx", out string endIdx);

            try
            {
                if ( locationId != null)
                {
                    return _productRepos.GetProductPaginateByDefault(locationId, int.Parse(startIdx), int.Parse(endIdx));
                } else if (itemCode != null)
                {
                    // add repos call the get product by item here
                    throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "To Be Implemented"));
                } else
                {
                    throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name));
                }
            }
            catch (Exception e)
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, e.ToString()));
            }
        }

    }
}
