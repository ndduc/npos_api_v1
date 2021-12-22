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
    public class SectionLogic : BaseHelper, ISectionLogic
    {
        private readonly ILocationRepos _locationRepos;
        private readonly ISectionRepos _sectionRepos;
        private readonly IUserRepos _userRepos;
        public SectionLogic()
        {
            _userRepos = new UserRepos();
            _locationRepos = new LocationRepos();
            _sectionRepos = new SectionRepos();
        }

        public bool UpdateSection(SectionModel model, string userId, string locationId)
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
            return _sectionRepos.UpdateSectionExecution(model);
        }


        public List<SectionModel> GetSectionByLocationId(string userId, string locationId)
        {
            if (_userRepos.VerifyUser(userId) && _locationRepos.VerifyUIdExist(locationId))
            {
                return _sectionRepos.GetSectionByLocationIdExecution(locationId);
            }
            else
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "unauthorized access"));
            }
        }



        public bool AddSection(SectionModel model, string userId, string locationId)
        {
            string id = null;
            bool isUnqiue = false;
            while (!isUnqiue)
            {
                id = Guid.NewGuid().ToString();
                isUnqiue = _sectionRepos.VerifyUIdUnique(id);
            }
            if (_userRepos.VerifyUser(userId) && _locationRepos.VerifyUIdExist(locationId))
            {
                model.UId = id;
                model.AddedBy = userId;
                model.LocationUId = locationId;
                return _sectionRepos.AddSectionExecution(model);
            }
            else
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "unauthorized access"));
            }
        }



        public bool AddSectionProductRelation(string uid, string productId, string locationId, string userId)
        {
            if (_userRepos.VerifyUser(userId)
                && _locationRepos.VerifyUIdExist(locationId)
                && _sectionRepos.VerifyUIdExist(uid)
                && !_sectionRepos.VerifySectionProductRelationExist(uid, productId, locationId))
            {

                return _sectionRepos.AddSectionProductRelationExecution(uid, productId, locationId, userId);
            }
            else
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "unauthorized access"));
            }
        }

        public int GetSectionPaginateCount(Dictionary<string, string> param)
        {
            param.TryGetValue("locationId", out string locationId);
            param.TryGetValue("searchType", out string searchType);
            try
            {
                return _sectionRepos.GetSectionPaginateCount(locationId);
                // Search Type indicate search by default (locId) or itemId, etc ...
            }
            catch (Exception e)
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, e.ToString()));
            }
        }

        public IEnumerable<SectionModel> GetSectionPaginate(Dictionary<string, string> param)
        {

            param.TryGetValue("locationId", out string locationId);
            param.TryGetValue("itemCode", out string itemCode);
            param.TryGetValue("startIdx", out string startIdx);
            param.TryGetValue("endIdx", out string endIdx);

            try
            {
                if (locationId != null)
                {
                    return _sectionRepos.GetSectionPaginateByDefault(locationId, int.Parse(startIdx), int.Parse(endIdx));
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

        public SectionModel GetSectionById(string userId, string locId, string SectionId)
        {
            if (_userRepos.VerifyUser(userId))
            {
                return _sectionRepos.GetSectionById(locId, SectionId);
            }
            else
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Unauthorized Access"));
            }

        }

        public IEnumerable<SectionModel> GetSectionByDescription(string userId, string locId, string description)
        {
            if (_userRepos.VerifyUser(userId))
            {
                return _sectionRepos.GetSectionByDescription(locId, description);
            }
            else
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Unauthorized Access"));
            }
        }


    }
}
