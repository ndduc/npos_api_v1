using POS_Api.Core.Interface;
using POS_Api.Model;
using POS_Api.Repository.Implementation;
using POS_Api.Repository.Interface;
using POS_Api.Shared.ExceptionHelper;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace POS_Api.Core.Implementation
{
    public class TaxLogic : BaseHelper, ITaxLogic
    {
        private readonly ILocationRepos _locationRepos;
        private readonly IProductRepos _productRepos;
        private readonly ITaxRepos _taxRepos;
        private readonly IUserRepos _userRepos;
        public TaxLogic()
        {
            _userRepos = new UserRepos();
            _locationRepos = new LocationRepos();
            _productRepos = new ProductRepos();
            _taxRepos = new TaxRepos();
        }

        // Update Tax Rate and Desc
        public bool UpdateTax(TaxModel model, string userId, string locationId)
        {
            bool isUserValid = _userRepos.VerifyUser(userId);
            bool isLocationValid = _locationRepos.VerifyUIdExist(locationId);

            if (!isUserValid)
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Invalid User"));
            }

            if (!isLocationValid)
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Invalid Location"));
            }

            model.UpdatedBy = userId;
            return _taxRepos.UpdateTaxExecution(model);
        }


        public bool AddTax(TaxModel model, string userId, string locationId)
        {
            string id = null;
            bool isUnqiue = false;
            while (!isUnqiue)
            {
                id = Guid.NewGuid().ToString();
                isUnqiue = _taxRepos.VerifyUIdUnique(id);
            }
            if (_userRepos.VerifyUser(userId) && _locationRepos.VerifyUIdExist(locationId))
            {
                model.UId = id;
                model.AddedBy = userId;
                model.LocationUId = locationId;
                return _taxRepos.AddTaxExecution(model);
            }
            else
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "unauthorized access"));
            }
        }

        public bool AddTaxProductRelation(string productId, string locationId, string taxId, string userId)
        {
            bool isUserValid = _userRepos.VerifyUser(userId);
            bool isLocationValid = _locationRepos.VerifyUIdExist(locationId);
            bool isTaxValid = _taxRepos.VerifyUIdExist(taxId);
            bool isProductValid = _productRepos.VerifyUIdExist(productId);
            bool isTaxRelationExist = _taxRepos.VerifyTaxProductRelation(productId, locationId);
            bool isProductLocationExist = _productRepos.IsProductLocationExist(locationId, productId);  // Verify If Product and Location are sync

            if (!isUserValid)
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Invalid User"));
            }

            if (!isLocationValid)
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Invalid Location"));
            }

            if (!isProductValid)
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Invalid Product"));
            }

            if (!isTaxValid)
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Invalid Tax"));
            }

            if (isTaxRelationExist)
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "This Tax Relation Already Existed"));
            }

            if (!isProductLocationExist)
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Product Is Not Exist In This Location"));
            }

            return _taxRepos.AddTaxProductRelationExecution(productId, locationId, taxId, userId);
        }


        public List<TaxModel> GetTaxByLocationId(string userId, string locationId)
        {
            if (_userRepos.VerifyUser(userId) && _locationRepos.VerifyUIdExist(locationId))
            {
                return _taxRepos.GetTaxByLocationIdExecution(locationId);
            }
            else
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "unauthorized access"));
            }
        }

        public int GetTaxPaginateCount(Dictionary<string, string> param)
        {
            param.TryGetValue("locationId", out string locationId);
            param.TryGetValue("searchType", out string searchType);
            try
            {
                return _taxRepos.GetTaxPaginateCount(locationId);
                // Search Type indicate search by default (locId) or itemId, etc ...
            }
            catch (Exception e)
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, e.ToString()));
            }
        }

        public IEnumerable<TaxModel> GetTaxPaginate(Dictionary<string, string> param)
        {

            param.TryGetValue("locationId", out string locationId);
            param.TryGetValue("itemCode", out string itemCode);
            param.TryGetValue("startIdx", out string startIdx);
            param.TryGetValue("endIdx", out string endIdx);

            try
            {
                if (locationId != null)
                {
                    return _taxRepos.GetTaxPaginateByDefault(locationId, int.Parse(startIdx), int.Parse(endIdx));
                }
                else if (itemCode != null)
                {
                    // add repos call the get product by item here
                    throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "To Be Implemented"));
                }
                else
                {
                    throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name));
                }
            }
            catch (Exception e)
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, e.ToString()));
            }
        }

        public TaxModel GetTaxById(string userId, string locId, string TaxId)
        {
            if (_userRepos.VerifyUser(userId))
            {
                return _taxRepos.GetTaxById(locId, TaxId);
            }
            else
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Unauthorized Access"));
            }

        }

        public IEnumerable<TaxModel> GetTaxByDescription(string userId, string locId, string description)
        {
            if (_userRepos.VerifyUser(userId))
            {
                return _taxRepos.GetTaxByDescription(locId, description);
            }
            else
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Unauthorized Access"));
            }
        }

    }
}
