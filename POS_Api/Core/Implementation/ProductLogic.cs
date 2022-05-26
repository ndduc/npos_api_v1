using POS_Api.Core.Interface;
using POS_Api.Model;
using POS_Api.Model.ReponseViewModel;
using POS_Api.Model.ViewModel;
using POS_Api.Repository.Implementation;
using POS_Api.Repository.Interface;
using POS_Api.Shared.ExceptionHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


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

            if (isUserValid)
            {
                model.UId = id;
                model.AddedBy = userId;

                isSucess = _productRepos.AddProductExecution(model);

                if (isSucess)
                {
                    return _productRepos.AddProductExecutionRelation(model, userId, locationId);
                }
                else
                {
                    return isSucess;
                }
            }
            else
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "unauthorized access"));
            }

        }

        public ProductAddModelVm AddProduct(ProductModel model)
        {
            string id = null;
            bool isUnqiue = false;


            // Verify User
            bool isUserValid = _userRepos.VerifyUser(model.UserUId);
            // Verify Location
            bool isLocationValid = _locationRepos.VerifyUIdExist(model.LocationUId);

            if (!isUserValid)
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Invalid User"));
            }

            if (!isLocationValid)
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

            List<string> deptList = model.DepartmentList;
            List<string> cateList = model.CategoryList;
            List<string> venList = model.VendorList;
            List<string> secList = model.SectionList;
            List<string> itemCodeList = model.ItemCodeList;
            List<string> upcList = model.UpcList;
            List<string> taxList = model.TaxList;
            List<string> discountList = model.DiscountList;

            if (itemCodeList.Count > 0)
            {
                model.ItemCode = 1;
            }
            else
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
            model.AddedBy = model.UserUId;

            bool isSucess = _productRepos.AddProductExecution(model);

            ProductAddModelVm responseModel = new ProductAddModelVm();

            if (isSucess)
            {
                bool isProductLocationRelationSucess = _productRepos.AddProductExecutionRelation(model, model.UserUId, model.LocationUId);

                bool isDepartmentSucess = AddProductFunctionHelper(deptList, model.UId, model.UserUId, model.LocationUId, "DEPARTMENT");
                bool isCategorySucess = AddProductFunctionHelper(cateList, model.UId, model.UserUId, model.LocationUId, "CATEGORY");
                bool isVendorSucess = AddProductFunctionHelper(venList, model.UId, model.UserUId, model.LocationUId, "VENDOR");
                bool isSectionSucess = AddProductFunctionHelper(secList, model.UId, model.UserUId, model.LocationUId, "SECTION");
                bool isTaxSucess = AddProductFunctionHelper(taxList, model.UId, model.UserUId, model.LocationUId, "TAX");
                bool isDiscountSucess = AddProductFunctionHelper(discountList, model.UId, model.UserUId, model.LocationUId, "DISCOUNT");

                bool isItemCodeSucess = AddProductFunctionHelper(itemCodeList, model.UId, model.UserUId, model.LocationUId, "ITEMCODE");
                bool isUpcSucess = AddProductFunctionHelper(upcList, model.UId, model.UserUId, model.LocationUId, "UPC");

                // Still Missing Upc Logic

                #region SET RESPONSE MODEL
                if (isProductLocationRelationSucess)
                {
                    responseModel.Product_Location_Status = "OK";
                }
                else
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

        private bool AddProductFunctionHelper(List<string> valueList, string productId, string userId, string locationId, string option)
        {
            bool checker = false;

            if (valueList.Count > 0)
            {
                switch (option)
                {
                    case "CATEGORY":
                        checker = _categoryRepos.AddCategoryExecutionFromList(valueList, productId, locationId, userId);
                        break;
                    case "DEPARTMENT":
                        checker = _departmentRepos.AddDepartmentExecutionFromList(valueList, productId, locationId, userId);
                        break;
                    case "SECTION":
                        checker = _sectionRepos.AddSectionExecutionFromList(valueList, productId, locationId, userId);
                        break;
                    case "VENDOR":
                        checker = _vendorRepos.AddVendorExecutionFromList(valueList, productId, locationId, userId);
                        break;
                    case "TAX":
                        checker = _taxRepos.AddTaxExecutionFromList(valueList, productId, locationId, userId);
                        break;
                    case "DISCOUNT":
                        checker = _discountRepos.AddDiscountExecutionFromList(valueList, productId, locationId, userId);
                        break;
                    case "ITEMCODE":
                        checker = AddItemCodeExecutionFromList(valueList, productId, locationId, userId);
                        break;
                    case "UPC":
                        checker = AddUpcExecutionFromList(valueList, productId, locationId, userId);
                        break;
                    default:
                        break;
                }
            }

            return checker;
        }

        private bool UpsertProductFunctionHelper(List<string> valueList, string productId, string userId, string locationId, string option)
        {
            // note: logically speaking, each product should only have attribute per option
            bool checker = false;

            if (valueList.Count > 0)
            {
                switch (option)
                {
                    case "CATEGORY":
                        checker = _categoryRepos.UpsertCategoryExecutionFromList(valueList, productId, locationId, userId);
                        break;
                    case "DEPARTMENT":
                        checker = _departmentRepos.UpsertDepartmentExecutionFromList(valueList, productId, locationId, userId);
                        break;
                    case "SECTION":
                        checker = _sectionRepos.UpsertSectionExecutionFromList(valueList, productId, locationId, userId);
                        break;
                    case "VENDOR":
                        checker = _vendorRepos.UpsertVendorExecutionFromList(valueList, productId, locationId, userId);
                        break;
                    case "TAX":
                        checker = _taxRepos.UpsertTaxExecutionFromList(valueList, productId, locationId, userId);
                        break;
                    case "DISCOUNT":
                        checker = _discountRepos.UpsertDiscountExecutionFromList(valueList, productId, locationId, userId);
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

        public ProductAddModelVm UpdateProduct(ProductModel model) {
            // Verify User
            bool isUserValid = _userRepos.VerifyUser(model.UserUId);
            // Verify Location
            bool isLocationValid = _locationRepos.VerifyUIdExist(model.LocationUId);

            bool isProductExist = _productRepos.VerifyUIdExist(model.UId);

            bool isSucess = false;
            if (!isUserValid)
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Invalid User"));
            }

            if (!isLocationValid)
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Invalid Location"));
            }

            if (!isProductExist)
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Selected Product Not Exist"));
            }

            List<string> deptList = model.DepartmentList;
            List<string> cateList = model.CategoryList;
            List<string> venList = model.VendorList;
            List<string> secList = model.SectionList;
            List<string> itemCodeList = model.ItemCodeList == null ? new List<string>() : model.ItemCodeList;
            List<string> upcList = model.UpcList == null ? new List<string>() : model.UpcList;
            List<string> taxList = model.TaxList;
            List<string> discountList = model.DiscountList;
            ProductAddModelVm responseModel = new ProductAddModelVm();

            if (itemCodeList.Count > 0)
            {
                model.ItemCode = 1;
                responseModel.Product_ItemCode = "OK";
            }
            else
            {
                model.ItemCode = 0;
                responseModel.Product_ItemCode = "Product Does Not Have Item Code";
            }

            if (upcList.Count > 0)
            {
                model.Upc = 1;
                responseModel.Product_Upc = "OK";
            }
            else
            {
                model.Upc = 0;
                responseModel.Product_Upc = "Product Does Not Have Upc";
            }

            model.UpdatedBy = model.UserUId;

            isSucess = _productRepos.UpdateProductExecution(model);

            if (isSucess)
            {
                bool isProductLocationRelationSucess = _productRepos.UpdateProductExecutionRelation(model, model.LocationUId);
                bool isDepartmentSucess = UpsertProductFunctionHelper(deptList, model.UId, model.UserUId, model.LocationUId, "DEPARTMENT");
                bool isCategorySucess = UpsertProductFunctionHelper(cateList, model.UId, model.UserUId, model.LocationUId, "CATEGORY");
                bool isVendorSucess = UpsertProductFunctionHelper(venList, model.UId, model.UserUId, model.LocationUId, "VENDOR");
                bool isSectionSucess = UpsertProductFunctionHelper(secList, model.UId, model.UserUId, model.LocationUId, "SECTION");
                bool isTaxSucess = UpsertProductFunctionHelper(taxList, model.UId, model.UserUId, model.LocationUId, "TAX");
                bool isDiscountSucess = UpsertProductFunctionHelper(discountList, model.UId, model.UserUId, model.LocationUId, "DISCOUNT");

                #region SET RESPONSE MODEL
                if (isProductLocationRelationSucess)
                {
                    responseModel.Product_Location_Status = "OK";
                }
                else
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
                #endregion
                return responseModel;
            }
            else
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Product Is Not Sucessfully Updated"));
            }

            // Do UPDATE HERE
            // CHECK relation, if different then do update;

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


        public ProductModelVm GetProductById(string userId, string locationId, Dictionary<string, string> param, bool isCheckout)
        {
            return _productRepos.GetProductByIdWithMapExecution(userId, locationId, param, isCheckout);
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
            param.TryGetValue("searchText", out string searchText);
            param.TryGetValue("uid", out string uid);
            param.TryGetValue("upc", out string upc);
            param.TryGetValue("itemCode", out string itemCode);
            try
            {
                string whereClause = "";
                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    whereClause = " WHERE AL.description like '%" + searchText + "%'";
                } else if (!string.IsNullOrWhiteSpace(uid))
                {
                    whereClause = " WHERE AL.uid like '%" + uid + "%'";
                }
                else if (!string.IsNullOrWhiteSpace(itemCode))
                {
                    whereClause = " WHERE ITEM_CODE.item_code like '%" + itemCode + "%'";
                }
                else if (!string.IsNullOrWhiteSpace(upc))
                {
                    whereClause = " WHERE UPC.upc like '%" + upc + "%'";
                }
                return _productRepos.GetProductPaginateCount(locationId, whereClause);
                // Search Type indicate search by default (locId) or itemId, etc ...
            }
            catch (Exception e)
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, e.ToString()));
            }
        }

        public IEnumerable<ProductModel> GetProductPaginate(Dictionary<string, string> param)
        {

            param.TryGetValue("locationId", out string locationId);
            param.TryGetValue("startIdx", out string startIdx);
            param.TryGetValue("endIdx", out string endIdx);
            param.TryGetValue("searchText", out var searchText);
            param.TryGetValue("uid", out string uid);
            param.TryGetValue("upc", out string upc);
            param.TryGetValue("itemCode", out string itemCode);
            try
            {

                string whereClause = "";
                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    whereClause = " WHERE AL.description like '%" + searchText + "%'";
                }
                else if (!string.IsNullOrWhiteSpace(uid))
                {
                    whereClause = " WHERE AL.uid like '%" + uid + "%'";
                }
                else if (!string.IsNullOrWhiteSpace(itemCode))
                {
                    whereClause = " WHERE ITEM_CODE.item_code like '%" + itemCode + "%'";
                }
                else if (!string.IsNullOrWhiteSpace(upc))
                {
                    whereClause = " WHERE UPC.upc like '%" + upc + "%'";
                }


                if (locationId != null)
                {
                    var list = _productRepos.GetProductPaginateByDefault(locationId, int.Parse(startIdx), int.Parse(endIdx), whereClause);
                    return list;
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
