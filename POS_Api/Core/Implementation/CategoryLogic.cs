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
    public class CategoryLogic: BaseHelper, ICategoryLogic
    {
        private readonly IUserLogic _userLogic;
        private readonly ILocationLogic _locationLogic;
        private readonly ICategoryRepos _categoryRepos;
        public CategoryLogic(IUserLogic userLogic, ILocationLogic locationLogic)
        {
            _userLogic = userLogic;
            _locationLogic = locationLogic;
            _categoryRepos = new CategoryRepos();
        }

        public bool UpdateCategory(CategoryModel model, string userId, string locationId)
        {
            bool isUserValid = _userLogic.VerifyUser(userId);
            bool isLocationValid = _locationLogic.VerifyUIdExist(locationId);

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
            if (_userLogic.VerifyUser(userId) && _locationLogic.VerifyUIdExist(locationId))
            {
                return _categoryRepos.GetCategoryByLocationIdExecution(locationId);
            }
            else {
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
            if(_userLogic.VerifyUser(userId) && _locationLogic.VerifyUIdExist(locationId))
            {
                model.UId = id;
                model.AddedBy = userId;
                model.LocationUId = locationId;
                return _categoryRepos.AddCategoryExecution(model);
            } else
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "unauthorized access"));
            }
        }

        public bool AddCategoryProductRelation(string uid, string productId, string locationId, string userId)
        {
            if (_userLogic.VerifyUser(userId) 
                && _locationLogic.VerifyUIdExist(locationId)
                && _categoryRepos.VerifyUIdExist(uid)
                && !_categoryRepos.VerifyCategoryProductRelationExist(uid, productId, locationId)) {

                return _categoryRepos.AddCategoryProductRelationExecution(uid, productId, locationId, userId);
            } else
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "unauthorized access"));
            }
        }


    }
}
