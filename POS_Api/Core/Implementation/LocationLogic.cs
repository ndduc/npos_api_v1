using MySql.Data.MySqlClient;
using POS_Api.Core.Interface;
using POS_Api.Database.MySql.Configuration;
using POS_Api.Model;
using POS_Api.Model.EnumData;
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
    public class LocationLogic : BaseHelper, ILocationLogic
    {
        private readonly IUserRepos _userRepos;

        private readonly ILocationRepos _locationRepos;
        public LocationLogic()
        {
            _userRepos = new UserRepos();
            _locationRepos = new LocationRepos();
        }

        public bool AddLocation(LocationModel model, string userId)
        {
            string id = null;
            bool isUnqiue = false;
            // Verify whether this id is unique
            while (!isUnqiue)
            {
                id = Guid.NewGuid().ToString();
                isUnqiue = _locationRepos.VerifyUIdUnique(id);
            }
            model.UId = id;
            bool isInserted = _locationRepos.AddLocationExecution(model);
            bool isRelation = _userRepos.AddRelationLocationUser(null, userId, id, GenericEnumType.UserLocationType.CREATED.ToString());

            // Insert Record, return false if the insertion false
            if (isInserted && isRelation)
            {
                return true;
            } else
            {
                return false;
            }
        }

        public IEnumerable<LocationModel> GetLocationByUserId(string userId) {
            return _locationRepos.GetLocationByUserIdExecution(userId);
        }
    }
}
