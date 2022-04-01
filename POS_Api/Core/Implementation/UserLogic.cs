using POS_Api.Core.Interface;
using POS_Api.Model;
using POS_Api.Model.ReponseViewModel;
using POS_Api.Repository.Implementation;
using POS_Api.Repository.Interface;
using POS_Api.Shared.ExceptionHelper;
using POS_Api.Shared.Security;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace POS_Api.Core.Implementation
{
    public class UserLogic : BaseHelper, IUserLogic
    {

        private readonly IUserRepos _userRepos;
        private readonly ILocationRepos _locationRepos;
        public UserLogic()
        {
            _userRepos = new UserRepos();
            _locationRepos = new LocationRepos();
        }
        public bool AddUser(UserModel userModel)
        {
            string id = null;
            bool isIdUnqiue = false;
            bool isSucess = false;
            // Verify whether this id is unique

            while (!isIdUnqiue)
            {
                id = Guid.NewGuid().ToString();
                isIdUnqiue = _userRepos.VerifyUIdUnique(id);
            }

            bool isUserNameUnique = _userRepos.VerifyUserName(userModel.UserName);

            if (isIdUnqiue && isUserNameUnique)
            {
                userModel.UId = id;
                //Hash Password
                userModel.Password = HashingPassword.EncryptPlainTextToCipherText(userModel.Password);
                // Insert Record, return false if the insertion false
                isSucess = _userRepos.AddUserExecution(userModel);
                return isSucess;
            }
            else
            {
                return isSucess;
            }
        }

        public bool AddUserWithParent(UserModel userModel, string parentId, string locationId, string type)
        {
            string id = null;
            bool isIdUnqiue = false;
            bool isSucess = false;
            // Verify whether this id is unique

            while (!isIdUnqiue)
            {
                id = Guid.NewGuid().ToString();
                isIdUnqiue = _userRepos.VerifyUIdUnique(id);
            }

            bool isUserNameUnique = _userRepos.VerifyUserName(userModel.UserName);

            if (isIdUnqiue && isUserNameUnique)
            {
                userModel.UId = id;
                //Hash Password
                userModel.Password = HashingPassword.EncryptPlainTextToCipherText(userModel.Password);
                // Insert Record, return false if the insertion false
                isSucess = _userRepos.AddUserExecution(userModel);
                if (isSucess)
                {
                    return _userRepos.AddRelationLocationUser(parentId, id, locationId, type);
                }
                else
                {
                    return isSucess;
                }

            }
            else
            {
                return isSucess;
            }
        }

        // Return true indicate the id is unique


        public UserModel GetUserByPassAndUserName(string userName, string pass)
        {
            return _userRepos.GetUserByPassAndUserNameExecution(userName, pass);
        }

        public bool UpdateUser(UserModel userModel)
        {
            return _userRepos.UpdateUserExecution(userModel);
        }


        public bool UpdatePassword(UserModel userModel)
        {
            return _userRepos.UpdatePasswordExecution(userModel);
        }

        public bool AddRelationLocationUser(string muserId, string userId, string locationId, string reason)
        {
            return _userRepos.AddRelationLocationUser(muserId, userId, locationId, reason);
        }

        public GenericPaginationModelVm<UserLocationModel> GetUserPagination(string userId, string locId, Dictionary<string, string> param)
        {
            bool isUserValid = _userRepos.VerifyUser(userId);
            if (isUserValid)
            {
                param.TryGetValue("startIdx", out string startIdx);
                param.TryGetValue("endIdx", out string endIdx);
                param.TryGetValue("userFullName", out string userFullName);

                string whereClause = "";
                if (!string.IsNullOrWhiteSpace(userFullName))
                {
                    whereClause = " WHERE AU.firstname LIKE '%" + userFullName  + "%' OR AU.lastname LIKE '%" + userFullName + "%' ";
                }

                if (!string.IsNullOrWhiteSpace(locId))
                {
                    int count = _userRepos.GetUserPaginationCount(locId, whereClause);
                    GenericPaginationModelVm<UserLocationModel> paginationModel = new GenericPaginationModelVm<UserLocationModel>
                    {
                        Count = count,
                        StartIndex = int.Parse(startIdx),
                        EndIndex = int.Parse(endIdx)
                    };
                    if (count != 0)
                    {
                        IEnumerable<UserLocationModel> userLocationModel = _userRepos.GetUserPagination(locId, whereClause);
                        paginationModel.DataObject = userLocationModel;

                        foreach(var item in paginationModel.DataObject)
                        {
                            item.UserLocations = new List<LocationModel>();
                            foreach (var location in item.LocationIds)
                            {
                                LocationModel loc = _locationRepos.GetLocationRelation(item.UId, location);
                                item.UserLocations.Add(loc);
                            }
                        }
                    }
                    
                    return paginationModel;
                } else
                {
                    throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name));
                }

            } else
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "unauthorized access"));
            }
        }
    }
}
