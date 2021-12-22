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
    public class CategoryLogic : BaseHelper, ICategoryLogic
    {
        private readonly ICategoryRepos _categoryRepos;
        private readonly IUserRepos _userRepos;
        private readonly ILocationRepos _locationRepos;
        public CategoryLogic()
        {
            _categoryRepos = new CategoryRepos();
            _locationRepos = new LocationRepos();
            _userRepos = new UserRepos();
        }

        public bool UpdateCategory(CategoryModel model, string userId, string locationId)
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
            return _categoryRepos.UpdateCategoryExecution(model);
        }


        public List<CategoryModel> GetCategoryByLocationId(string userId, string locationId)
        {
            if (_userRepos.VerifyUser(userId) && _locationRepos.VerifyUIdExist(locationId))
            {
                return _categoryRepos.GetCategoryByLocationIdExecution(locationId);
            }
            else
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "unauthorized access"));
            }
        }



        public bool AddCategory(CategoryModel model, string userId, string locationId)
        {
            string id = null;
            bool isUnqiue = false;
            while (!isUnqiue)
            {
                id = Guid.NewGuid().ToString();
                isUnqiue = _categoryRepos.VerifyUIdUnique(id);
            }
            if (_userRepos.VerifyUser(userId) && _locationRepos.VerifyUIdExist(locationId))
            {
                model.UId = id;
                model.AddedBy = userId;
                model.LocationUId = locationId;
                return _categoryRepos.AddCategoryExecution(model);
            }
            else
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "unauthorized access"));
            }
        }

        public bool AddCategoryProductRelation(string uid, string productId, string locationId, string userId)
        {
            if (_userRepos.VerifyUser(userId)
                && _locationRepos.VerifyUIdExist(locationId)
                && _categoryRepos.VerifyUIdExist(uid)
                && !_categoryRepos.VerifyCategoryProductRelationExist(uid, productId, locationId))
            {

                return _categoryRepos.AddCategoryProductRelationExecution(uid, productId, locationId, userId);
            }
            else
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "unauthorized access"));
            }
        }


    }
}
