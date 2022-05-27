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
            model.LocationUId = locationId;
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

        public int GetCategoryPaginateCount(Dictionary<string, string> param)
        {
            param.TryGetValue("locationId", out string locationId);
            param.TryGetValue("searchType", out string searchType);
            try
            {
                return _categoryRepos.GetCategoryPaginateCount(locationId);
                // Search Type indicate search by default (locId) or itemId, etc ...
            }
            catch (Exception e)
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, e.ToString()));
            }
        }

        public IEnumerable<CategoryModel> GetCategoryPaginate(Dictionary<string, string> param)
        {

            param.TryGetValue("locationId", out string locationId);
            param.TryGetValue("itemCode", out string itemCode);
            param.TryGetValue("startIdx", out string startIdx);
            param.TryGetValue("endIdx", out string endIdx);

            try
            {
                if (locationId != null)
                {
                    return _categoryRepos.GetCategoryPaginateByDefault(locationId, int.Parse(startIdx), int.Parse(endIdx));
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

        public CategoryModel GetCategoryById(string userId, string locId, string CategoryId)
        {
            if (_userRepos.VerifyUser(userId))
            {
                return _categoryRepos.GetCategoryById(locId, CategoryId);
            }
            else
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Unauthorized Access"));
            }

        }

        public IEnumerable<CategoryModel> GetCategoryByDescription(string userId, string locId, string description)
        {
            if (_userRepos.VerifyUser(userId))
            {
                return _categoryRepos.GetCategoryByDescription(locId, description);
            }
            else
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Unauthorized Access"));
            }
        }

        public IEnumerable<CategoryModel> GetCategoryByDepartmentId(string userId, string locId, string deptId)
        {
            if (_userRepos.VerifyUser(userId))
            {
                return _categoryRepos.GetCategoryByDepartmentId(locId, deptId);
            }
            else
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Unauthorized Access"));
            }
        }

    }
}
