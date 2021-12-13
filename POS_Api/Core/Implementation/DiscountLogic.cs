using MySql.Data.MySqlClient;
using POS_Api.Core.Interface;
using POS_Api.Database.MySql.Configuration;
using POS_Api.Model;
using POS_Api.Repository.Implementation;
using POS_Api.Repository.Interface;
using POS_Api.Shared.DbHelper;
using POS_Api.Shared.ExceptionHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace POS_Api.Core.Implementation
{
    public class DiscountLogic : BaseHelper, IDiscountLogic
    {
        private readonly IUserLogic _userLogic;
        private readonly ILocationLogic _locationLogic;
        private readonly IProductLogic _productLogic;
        private readonly ILocationProductRelationLogic _productLocationRelationLogic;

        private readonly IDiscountRepos _discountRepos;
        public DiscountLogic(IUserLogic userLogic, ILocationLogic locationLogic, IProductLogic productLogic, ILocationProductRelationLogic productLocationRelationLogic)
        {
            _userLogic = userLogic;
            _locationLogic = locationLogic;
            _productLogic = productLogic;
            _productLocationRelationLogic = productLocationRelationLogic;
            _discountRepos = new DiscountRepos();
        }

        // Update Discount Rate and Desc
        public bool UpdateDiscount(DiscountModel model, string userId, string locationId)
        {
            bool isUserValid = _userLogic.VerifyUser(userId);
            bool isLocationValid = _locationLogic.VerifyUIdExist(locationId);

            if (!isUserValid)
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Invalid User"));
            }

            if (!isLocationValid)
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Invalid User"));
            }

            model.UpdatedBy = userId;
            return _discountRepos.UpdateDiscountExecution(model);
        }


        public bool AddDiscount(DiscountModel model, string userId, string locationId)
        {
            string id = null;
            bool isUnqiue = false;
            while (!isUnqiue)
            {
                id = Guid.NewGuid().ToString();
                isUnqiue = _discountRepos.VerifyUIdUnique(id);
            }
            if (_userLogic.VerifyUser(userId) && _locationLogic.VerifyUIdExist(locationId))
            {
                model.UId = id;
                model.AddedBy = userId;
                model.LocationUId = locationId;
                return _discountRepos.AddDiscountExecution(model);
            }
            else
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "unauthorized access"));
            }
        }

        public List<DiscountModel> GetDiscountByLocationId(string userId, string locationId)
        {
            if (_userLogic.VerifyUser(userId) && _locationLogic.VerifyUIdExist(locationId))
            {
                return _discountRepos.GetDiscountByLocationIdExecution(locationId);
            }
            else
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "unauthorized access"));
            }
        }


        public bool AddDiscountProductRelation(string productId, string locationId, string discountId, string userId)
        {
            bool isUserValid = _userLogic.VerifyUser(userId);
            bool isLocationValid = _locationLogic.VerifyUIdExist(locationId);
            bool isTaxValid = _discountRepos.VerifyUIdExist(discountId);
            bool isProductValid = _productLogic.VerifyUIdExist(productId);
            bool isTaxRelationExist = _discountRepos.VerifyDiscountProductRelation(productId, locationId);
            bool isProductLocationExist = _productLocationRelationLogic.IsProductLocationExist(locationId, productId);  // Verify If Product and Location are sync

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
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Invalid Discount"));
            }

            if (isTaxRelationExist)
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "This Discount Relation Already Existed"));
            }

            if (!isProductLocationExist)
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Product Is Not Exist In This Location"));
            }

            return _discountRepos.AddDiscountProductRelationExecution(productId, locationId, discountId, userId);
        }


    }
}
