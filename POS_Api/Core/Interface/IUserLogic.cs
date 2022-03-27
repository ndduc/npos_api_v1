using POS_Api.Model;
using POS_Api.Model.ReponseViewModel;
using System.Collections.Generic;

namespace POS_Api.Core.Interface
{
    public interface IUserLogic
    {
        public UserModel GetUserByPassAndUserName(string userName, string pass);

        public bool AddUser(UserModel userModel);

        public bool UpdateUser(UserModel userModel);

        public bool UpdatePassword(UserModel userModel);


        public bool AddUserWithParent(UserModel userModel, string parentId, string locationId, string type);

        public bool AddRelationLocationUser(string muserId, string userId, string locationId, string reason);

        public GenericPaginationModelVm<UserLocationModel> GetUserPagination(string userId, string locId, Dictionary<string, string> param);


    }
}
