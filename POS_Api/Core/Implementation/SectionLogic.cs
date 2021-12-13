﻿using MySql.Data.MySqlClient;
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
    public class SectionLogic : BaseHelper, ISectionLogic
    {
        private readonly IUserLogic _userLogic;
        private readonly ILocationRepos _locationRepos;
        private readonly ISectionRepos _sectionRepos;
        public SectionLogic(IUserLogic userLogic)
        {
            _userLogic = userLogic;
            _locationRepos = new LocationRepos();
            _sectionRepos = new SectionRepos();
        }

        public bool UpdateSection(SectionModel model, string userId, string locationId)
        {
            bool isUserValid = _userLogic.VerifyUser(userId);
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
            if (_userLogic.VerifyUser(userId) && _locationRepos.VerifyUIdExist(locationId))
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
            if (_userLogic.VerifyUser(userId) && _locationRepos.VerifyUIdExist(locationId))
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
            if (_userLogic.VerifyUser(userId)
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


    }
}
