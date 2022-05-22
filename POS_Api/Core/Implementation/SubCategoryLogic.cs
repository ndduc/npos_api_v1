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
    public class SubCategoryLogic : BaseHelper, ISubCategoryLogic
    {
        private readonly ISubCategoryRepos _subCategoryRepos;
        private readonly IUserRepos _userRepos;
        private readonly ILocationRepos _locationRepos;
        public SubCategoryLogic()
        {
            _subCategoryRepos = new SubCategoryRepos();
            _locationRepos = new LocationRepos();
            _userRepos = new UserRepos();
        }

        public bool UpdateSubCategory(SubCategoryModel model, string userId, string locationId)
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
            return _subCategoryRepos.UpdateSubCategoryExecution(model);
        }


        public List<SubCategoryModel> GetSubCategoryByLocationId(string userId, string locationId)
        {
            if (_userRepos.VerifyUser(userId) && _locationRepos.VerifyUIdExist(locationId))
            {
                return _subCategoryRepos.GetSubCategoryByLocationIdExecution(locationId);
            }
            else
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "unauthorized access"));
            }
        }



        public bool AddSubCategory(SubCategoryModel model, string userId, string locationId)
        {
            string id = null;
            bool isUnqiue = false;
            while (!isUnqiue)
            {
                id = Guid.NewGuid().ToString();
                isUnqiue = _subCategoryRepos.VerifyUIdUnique(id);
            }
            if (_userRepos.VerifyUser(userId) && _locationRepos.VerifyUIdExist(locationId))
            {
                model.UId = id;
                model.AddedBy = userId;
                model.LocationUId = locationId;
                return _subCategoryRepos.AddSubCategoryExecution(model);
            }
            else
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "unauthorized access"));
            }
        }

        public bool AddSubCategoryProductRelation(string uid, string productId, string locationId, string userId)
        {
            if (_userRepos.VerifyUser(userId)
                && _locationRepos.VerifyUIdExist(locationId)
                && _subCategoryRepos.VerifyUIdExist(uid)
                && !_subCategoryRepos.VerifySubCategoryProductRelationExist(uid, productId, locationId))
            {

                return _subCategoryRepos.AddSubCategoryProductRelationExecution(uid, productId, locationId, userId);
            }
            else
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "unauthorized access"));
            }
        }

        public int GetSubCategoryPaginateCount(Dictionary<string, string> param)
        {
            param.TryGetValue("locationId", out string locationId);
            param.TryGetValue("searchType", out string searchType);
            try
            {
                return _subCategoryRepos.GetSubCategoryPaginateCount(locationId);
                // Search Type indicate search by default (locId) or itemId, etc ...
            }
            catch (Exception e)
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, e.ToString()));
            }
        }

        public IEnumerable<SubCategoryModel> GetSubCategoryPaginate(Dictionary<string, string> param)
        {

            param.TryGetValue("locationId", out string locationId);
            param.TryGetValue("itemCode", out string itemCode);
            param.TryGetValue("startIdx", out string startIdx);
            param.TryGetValue("endIdx", out string endIdx);

            try
            {
                if (locationId != null)
                {
                    return _subCategoryRepos.GetSubCategoryPaginateByDefault(locationId, int.Parse(startIdx), int.Parse(endIdx));
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

        public SubCategoryModel GetSubCategoryById(string userId, string locId, string SubCategoryId)
        {
            if (_userRepos.VerifyUser(userId))
            {
                return _subCategoryRepos.GetSubCategoryById(locId, SubCategoryId);
            }
            else
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Unauthorized Access"));
            }

        }

        public IEnumerable<SubCategoryModel> GetSubCategoryByDescription(string userId, string locId, string description)
        {
            if (_userRepos.VerifyUser(userId))
            {
                return _subCategoryRepos.GetSubCategoryByDescription(locId, description);
            }
            else
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Unauthorized Access"));
            }
        }

        public IEnumerable<SubCategoryModel> GetSubCategoryByDepartmentId(string userId, string locId, string deptId)
        {
            if (_userRepos.VerifyUser(userId))
            {
                return _subCategoryRepos.GetSubCategoryByDepartmentId(locId, deptId);
            }
            else
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Unauthorized Access"));
            }
        }

    }
}
